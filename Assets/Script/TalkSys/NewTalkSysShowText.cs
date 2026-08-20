using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class NewTalkSysShowText : MonoBehaviour, ITalkSysCore
{
    private TalkSystem _talkSys;
    private List<Manager> TextBox => _talkSys != null ? _talkSys.Talklines : null;
    
    // 修复：玩家名索引越界保护
    private string PlayerNameBox
    {
        get
        {
            if (_talkSys?.PlayerName?.TxtLine == null || _talkSys.PlayerName.TxtLine.Count == 0)
                return string.Empty;
            return _talkSys.PlayerName.TxtLine[0];
        }
    }

    private int DayNum => _talkSys != null ? _talkSys.Daytime : -1;
    private float IntervalTime => _talkSys != null ? _talkSys.TextSpeedI : 0.05f;
    private bool MiniMode => _talkSys != null && _talkSys.MiniMode;
    
    private Coroutine _outPutTextCoroutine;
    public int curLine;
    private bool _isShowingText = false;
    private bool _isPlayerTalk = false;
    private TalkSysSwitch _switchManager; 
    
    private Manager Language => GlobalData.Language;
    
    [SerializeField]
    private bool _lock;
    
    private Manager SkipManager => _talkSys?.showText?.skipManager;
    private MiniCharacterTalkSys MiniTalkSys => _talkSys?.MiniCharacterManager;
    private CgManager CgManager => _talkSys?.showText?.cgManager;
    
    public bool inBranch;
    public int branchLine = 0;
    private bool _inShop;
    private TextMeshProUGUI _shopText, _shopName;
    public bool OnHistory { get; set; }
    public Manager autoPlay;
    private Coroutine _autoCoroutine;

    // 辅助属性：统一校验当天数据是否有效
    private bool IsDayValid => TextBox != null && DayNum >= 0 && DayNum < TextBox.Count && TextBox[DayNum]?.data != null;
    
    // 辅助属性：统一校验当前行是否有效
    private bool IsCurrentLineValid => IsDayValid && curLine >= 0 && curLine < TextBox[DayNum].data.Count;

    public void Init(TalkSystem talkSys)
    {
        _talkSys = talkSys;
        if (talkSys != null)
        {
            _switchManager = talkSys.switchManager;
            _shopName = talkSys.ShopName;
            _shopText = talkSys.ShopTextBar != null ? talkSys.ShopTextBar.GetComponent<TextMeshProUGUI>() : null;
        }
        GlobalData.NewTalkSysShowText = this;
    }

    public void ShowText()
    {
        // 修复：入口统一空值与锁状态校验
        if (_talkSys == null || _lock || FullModeState.GetValue(true) || OnHistory)
        {
            _talkSys?.Type?.Stop();
            return;
        }

        // 修复：DayNum越界 + curLine越界双重校验
        if (!IsDayValid)
        {
            return;
        }

        if (TextBox[DayNum].data.Count <= curLine)
        {
            if (GlobalData.Progress != null && GlobalData.Progress.talk)
            {
                curLine = Mathf.Max(0, TextBox[DayNum].data.Count - 1);
            }
            return;
        }

        var lineData = TextBox[DayNum].data[curLine];
        
        // 修复：CG显示空引用保护
        if (lineData.showCg)
        {
            SetCg(lineData.cgNum);
        }

        // 修复：角色不适状态校验
        if (lineData.uncomfortType != CharacterType.Player)
        {
            CheckComfort();
        }
        
        // 修复：音效播放空引用保护
        GlobalData.AudioEffect?.Play(lineData.audioEffect);

        if (lineData.onlyCode 
            || string.IsNullOrEmpty(lineData.cn) 
            || string.IsNullOrEmpty(lineData.en))
        {
            RunCode();
            curLine++;
            // 修复：递归前再次校验边界，防止越界
            if (curLine < TextBox[DayNum]?.data.Count)
            {
                ShowText();
            }
        }
        else
        {
            CheckSpeaker();
            if (!_isShowingText)
            {
                try
                {
                    SetHistory();
                }
                catch (Exception e)
                {
                    Debug.LogError($"记录历史失败：{e.Message}");
                }
                finally
                {
                    _outPutTextCoroutine = StartCoroutine(OutPutText(GetText(TextBox[DayNum], inBranch)));
                    RunCode();
                }
            }
            else
            {
                _talkSys.Type?.Stop();
                
                // 修复：协程停止空引用保护
                if (_outPutTextCoroutine != null)
                {
                    StopCoroutine(_outPutTextCoroutine);
                    _outPutTextCoroutine = null;
                }
                
                ShowAllText(GetText(TextBox[DayNum], inBranch));
                RunCode();
                
                if (!CheckBranch())
                {
                    curLine++;
                }
            }
        }
    }

    private void CheckComfort()
    {
        if (!IsCurrentLineValid || _talkSys?.CharacterList == null) return;

        var targetType = TextBox[DayNum].data[curLine].uncomfortType.ToString();
        foreach (var obj in _talkSys.CharacterList)
        {
            // 修复：对象为空时跳过
            if (obj == null) continue;
            Character character = obj.GetComponent<Character>();
            if (character != null && character.CharacterName == targetType)
            {
                character.NotComfort = true;
                return;
            }
        }
    }

    private void SetCg(int i)
    {
        FullModeState.SetValue(true);
        // 修复：CG管理器空引用保护
        if (CgManager != null)
        {
            CgManager.gameObject.SetActive(true);
            CgManager.ShowCg(i);
        }
    }

    private void SetHistory()
    {
        if (!IsCurrentLineValid || GlobalData.History == null) return;
        
        var tempHistory = new TextHistory
        {
            Text = GetText(TextBox[DayNum]),
            CharacterName = TextBox[DayNum].data[curLine].speaker.ToString()
        };

        if (TextBox[DayNum].data[curLine].isAside)
        {
            tempHistory.IsASide = true;
        }
        else
        {
            tempHistory.IsASide = false;
            tempHistory.IsPlayer = TextBox[DayNum].data[curLine].isPlayerTalking;
        }
        GlobalData.History.SetHistory(tempHistory);
    }

    private void CheckSpeaker()
    {
        if (!IsCurrentLineValid) return;
        
        var lineData = TextBox[DayNum].data[curLine];
        _isPlayerTalk = lineData.isPlayerTalking || lineData.isAside;

        // 修复：UI控件空引用保护
        if (_talkSys?.Player == null || _talkSys?.PlayerNameText == null) return;
        
        if (!_isPlayerTalk)
        {
            _talkSys.Player.text = string.Empty;
            _talkSys.PlayerNameText.text = string.Empty;    
        }
        else
        {
            _talkSys.PlayerNameText.text = PlayerNameBox;
        }
    }

    private void RunCode()
    {
        if (!IsCurrentLineValid) return;
        RunCodeInternal(TextBox[DayNum].data[curLine].codes);
        CheckMiniGame();
    }
    
    private void RunCode(Manager manager, int index)
    {
        // 修复：参数空值与索引越界校验
        if (manager?.data == null || index < 0 || index >= manager.data.Count) return;
        RunCodeInternal(manager.data[index].codes);
    }

    // 抽取公共代码执行逻辑
    private void RunCodeInternal(List<Code> codes)
    {
        if (codes == null || codes.Count == 0) return;
        
        foreach (var value in codes)
        {
            if (value == Code.HideCg)
            {
                CgManager?.HideCg();
                FullModeState.SetValue(false);
            }
            else
            {
                // 修复：开关管理器空引用保护
                _switchManager?.DoSwitchCode(value.ToString());
            }
        }
    }

    #region 输出文本
    private IEnumerator OutPutText(string text)
    {
        _isShowingText = true;
        var textUI = GetTextUI();
        
        // 修复：自动播放协程空引用保护
        if (_autoCoroutine != null)
        {
            StopCoroutine(_autoCoroutine);
            _autoCoroutine = null;
        }

        // 修复：文本UI空引用保护
        if (textUI != null)
        {
            textUI.text = string.Empty;
        }
        
        if (MiniMode)
        {
            MiniTalkSys?.ShowAllText(GetCurrentCharacterName(), string.Empty);
        }

        if (string.IsNullOrEmpty(text))
        {
            _isShowingText = false;
            yield break;
        }
        
        foreach (var value in text)
        {
            if (_lock)
            {
                _isShowingText = false;
                yield break;
            }

            if (MiniMode && !_isPlayerTalk)
            {
                MiniTalkSys?.ShowText(GetCurrentCharacterName(), value);
            }
            else if (textUI != null)
            {
                textUI.text += value;
            }
            
            _talkSys?.Type?.Play();
            yield return new WaitForSeconds(IntervalTime);
        }

        if (!CheckBranch())
        {
            curLine++;
        }
        _isShowingText = false;
        
        // 修复：autoPlay空引用保护
        if (autoPlay != null && autoPlay.GeneralBool && !_lock && !OnHistory && !FullModeState.GetValue(true))
        {
            _autoCoroutine = StartCoroutine(AutoPlayNext(text.Length));
        }
    }

    private void ShowAllText(string text)
    {
        _isShowingText = false;
        var textUI = GetTextUI();
        
        if (_autoCoroutine != null)
        {
            StopCoroutine(_autoCoroutine);
            _autoCoroutine = null;
        }

        if (MiniMode && !_isPlayerTalk)
        {
            MiniTalkSys?.ShowAllText(GetCurrentCharacterName(), string.Empty);
            MiniTalkSys?.ShowAllText(GetCurrentCharacterName(), text);
        }
        else if (textUI != null)
        {
            textUI.text = text;
        }

        if (autoPlay != null && autoPlay.GeneralBool && !_lock && !OnHistory && !FullModeState.GetValue(true))
        {
            _autoCoroutine = StartCoroutine(AutoPlayNext(text.Length));
        }
    }
    
    private IEnumerator AutoPlayNext(int textLength)
    {
        float baseReadTime = Mathf.Max(1f, IntervalTime * textLength * 0.6f);
        float speedFactor = Mathf.Max(0.2f, 1f - ((autoPlay.Weight - 1) * 0.15f));
        float waitTime = baseReadTime * speedFactor;
    
        yield return new WaitForSeconds(waitTime);

        if (autoPlay.GeneralBool && !_lock && !_isShowingText && !OnHistory && !FullModeState.GetValue(true))
        {
            ShowText();
        }
    }
    #endregion

    // 封装获取当前行分支信息
    private (List<string> branchList, bool hasBranch) GetCurrentBranchData()
    {
        if (!IsCurrentLineValid)
        {
            return (new List<string>(), false);
        }
        
        var lineData = TextBox[DayNum].data[curLine];
        bool isEn = Language != null && Language.isEn;
        
        // 修复：分支列表空引用保护
        List<string> branchList = isEn ? lineData.enBranch : lineData.cnBranch;
        bool hasBranch = isEn ? lineData.enHaveBranch : lineData.cnHaveBranch;
        
        return (branchList ?? new List<string>(), hasBranch && branchList != null && branchList.Count > 0);
    }

    private bool CheckBranch()
    {
        var (branchList, hasBranch) = GetCurrentBranchData();
        
        if (inBranch)
        {
            branchLine++;
            // 修复：分支索引越界保护
            if (branchLine >= branchList.Count)
            {
                inBranch = false;
                branchLine = 0;
                return false;
            }
            return true;
        }

        inBranch = hasBranch;
        if (inBranch) branchLine = 0;
        return inBranch;
    }

    private void CheckMiniGame()
    {
        if (!IsCurrentLineValid || GlobalData.ShowText == null) return;
        
        var v = TextBox[DayNum].data[curLine].minigameType;
        if (v == CharacterType.Player) return;

        // 修复：小游戏对象空引用保护
        switch (v)
        {
            case CharacterType.艾米莉:
                GlobalData.ShowText.aimiGame?.SetActive(true);
                break;
            case CharacterType.阿曼德:
                GlobalData.ShowText.amandeGame?.SetActive(true);
                break;
            case CharacterType.博金森:
                GlobalData.ShowText.boGame?.SetActive(true);
                break;
            case CharacterType.洛尔坎:
                GlobalData.ShowText.luoGame?.SetActive(true);
                break;
            case CharacterType.莱文:
                GlobalData.ShowText.laiWenGame?.SetActive(true);
                break;
            case CharacterType.商人:
                Debug.LogError($"小游戏配置错误 Day: {DayNum}");
                break;
        }
    }

    private string GetText(Manager manager, bool inBranch = false)
    {
        if (manager?.data == null || curLine < 0 || curLine >= manager.data.Count)
            return string.Empty;

        string s = string.Empty;
        var lineData = manager.data[curLine];
        bool isEn = Language != null && Language.isEn;

        if (!inBranch)
        {
            s = isEn ? lineData.en : lineData.cn;
        }
        else
        {
            var branchList = isEn ? lineData.enBranch : lineData.cnBranch;
            // 修复：分支索引越界保护
            if (branchList != null && branchLine >= 0 && branchLine < branchList.Count)
            {
                s = branchList[branchLine];
            }
        }
        
        SetCharaName(manager);
        
        // 文本替换
        s = s.Replace("{PlayerName}", PlayerNameBox);
        s = s.Replace("{DeadName}", GetDeadName());
        s = s.Replace("{DeadNum}", GetDeadNumber().ToString());
        s = s.Replace("{Person}", GetPerson());
        return s ?? string.Empty;
    }

    private string GetPerson()
    {
        if (Language == null) return string.Empty;
        
        if (GetDeadNumber() == 1)
        {
            switch (GetDeadName(true))
            {
                case "博金森":
                case "莱文":
                case "洛尔坎":
                    return Language.isEn ? "His" : "他";
                default:
                    return Language.isEn ? "Her" : "她";
            }
        }
        return Language.isEn ? "Their" : "他们";
    }

    private int GetDeadNumber()
    {
        if (GlobalData.Characters == null) return 0;
        
        int v = 0;
        foreach (var value in GlobalData.Characters)
        {
            if (value != null && value.Dead)
            {
                v++;
            }
        }
        return v;
    }

    private string GetDeadName(bool returnCn = false)
    {
        if (GlobalData.Characters == null) return string.Empty;
        
        var v = string.Empty;
        foreach (var value in GlobalData.Characters)
        {
            if (value == null || !value.Dead) continue;
            
            string name = CharacterName.GetCharaName(value.CharacterName, returnCn);
            if (string.IsNullOrEmpty(v))
            {
                v = name;
            }
            else
            {
                v += ", " + name;
            }
        }
        return v;
    }

    private TextMeshProUGUI GetTextUI()
    {
        if (_inShop)
        {
            return _shopText;
        }
        return _isPlayerTalk ? _talkSys?.Player : _talkSys?.Character;
    }

    public void SetShopStatus(bool status)
    {
        _inShop = status;
    }

    private void SetCharaName(Manager container)
    {
        if (container?.data == null || curLine < 0 || curLine >= container.data.Count) return;
        
        string text = string.Empty;
        var lineData = container.data[curLine];

        switch (lineData.speaker)
        {
            case CharacterType.Player:
                return;
            case CharacterType.阿曼德:
                text = Language != null && Language.isEn ? "Amanda" : "阿曼德";
                break;
            case CharacterType.艾米莉:
                text = Language != null && Language.isEn ? "Emily" : "艾米莉";
                break;
            case CharacterType.博金森:
                text = Language != null && Language.isEn ? "Bokinson" : "博金森";
                break;
            case CharacterType.莱文:
                text = Language != null && Language.isEn ? "Levine" : "莱文";
                break;
            case CharacterType.洛尔坎:
                text = Language != null && Language.isEn ? "Lorquin" : "洛尔坎";
                break;
        }

        // 商店模式名字显示
        if (_inShop && _shopName != null)
        {
            if (lineData.isAside)
            {
                _shopName.text = "";
            }
            else
            {
                _shopName.text = lineData.isPlayerTalking 
                    ? PlayerNameBox 
                    : (Language != null && Language.isEn ? "Merchant" : "商人");
            }
        }
        
        if (!_isPlayerTalk && !MiniMode)
        {
            int expression = lineData.expression;
            if (expression != 0)
            {
                _talkSys?.SwitchExpression(lineData.speaker.ToString(), expression);
            }
            _talkSys?.CharacterImageManager?.SetImage(lineData.speaker.ToString());
            
            // 修复：角色名UI空引用保护
            if (_talkSys?.Chara_Name != null)
            {
                _talkSys.Chara_Name.text = text;
            }
        }
    }

    private string GetCurrentCharacterName()
    {
        if (!IsCurrentLineValid) return string.Empty;
        return TextBox[DayNum].data[curLine].speaker.ToString();
    }

    public void SetChoiceLine(int line, bool resetHistory)
    {
        curLine = line;
        if (resetHistory)
        {
            GlobalData.History?.Refresh();
        }
        _isShowingText = false;
        
        var textUI = GetTextUI();
        if (textUI != null)
        {
            textUI.text = string.Empty;
        }
    }

    public void LockOutPut()
    {
        _lock = true;
    }

    public void UnLockOutPut()
    {
        _lock = false;
    }

    public bool GetLockState()
    {
        return _lock;
    }

    public void Skip()
    {
        // 修复：SkipManager空引用与索引越界保护
        if (SkipManager?.data != null && SkipManager.data.Count > 0)
        {
            RunCode(SkipManager, 0);
        }
        
        StopAllCoroutines();
        _outPutTextCoroutine = null;
        _autoCoroutine = null;
        
        if (_talkSys?.Player != null)
        {
            _talkSys.Player.text = string.Empty;
        }
    }
}