using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class NewTalkSysShowText : MonoBehaviour,ITalkSysCore
{
    private TalkSystem _talkSys;
    private List<Manager> TextBox => _talkSys.Talklines;
    private string PlayerNameBox => _talkSys.PlayerName.TxtLine[0];
    private int DayNum => _talkSys.Daytime;
    private float IntervalTime => _talkSys.TextSpeedI;
    private bool InShop => _talkSys._inshop;
    private bool MiniMode => _talkSys.MiniMode;
    private Coroutine _outPutTextCoroutine;
    public int curLine;
    private bool _isShowingText = false;
    private bool _isPlayerTalk = false;
    private TalkSysSwitch _switchManager; 
    private Manager Language => GlobalData.Language;
    [SerializeField]
    private bool _lock;
    private Manager SkipManager => _talkSys.showText.skipManager;
    private MiniCharacterTalkSys MiniTalkSys => _talkSys.MiniCharacterManager;
    private CgManager CgManager => _talkSys.showText.cgManager;
    public bool inBranch;
    public int branchLine = 0;
    private bool _inShop;
    private TextMeshProUGUI _shopText, _shopName;
    public bool OnHistory { get; set; }
    public Manager autoPlay;
    private Coroutine _autoCoroutine;

    public void Init(TalkSystem talkSys)
    {
        _talkSys = talkSys;
        _switchManager = talkSys.switchManager;
        GlobalData.NewTalkSysShowText = this;
        _shopName = talkSys.ShopName;
        _shopText = talkSys.ShopTextBar.GetComponent<TextMeshProUGUI>();
        

    }

    public void ShowText()
    {
        /*Debug.Log($"输出文本：{TextBox[DayNum].data[curLine].cn}");*/
        if (_lock || FullModeState.GetValue(true) || OnHistory)
        {
            _talkSys.Type.Stop();
            return;
        }

        if (TextBox[DayNum]?.data.Count <= curLine)
        {
            if (GlobalData.Progress.talk)
            {
                curLine = TextBox[DayNum].data.Count - 1;
            }
            return;
        }

        if (TextBox[DayNum].data[curLine].showCg)
        {
            SetCg(TextBox[DayNum].data[curLine].cgNum);
        }

        if (TextBox[DayNum]?.data[curLine].uncomfortType != CharacterType.Player)
        {
            CheckComfort();
        }
        
        var lineData = TextBox[DayNum].data[curLine];
        GlobalData.AudioEffect.Play(lineData.audioEffect);
        if (lineData.onlyCode 
            || string.IsNullOrEmpty(lineData.cn) 
            || string.IsNullOrEmpty(lineData.en))
        {
            RunCode();
            curLine++;
            // 修复越界：小于Count
            if (curLine < TextBox[DayNum].data.Count)
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
                    Debug.LogError(e);
                }
                finally
                {
                    _outPutTextCoroutine = StartCoroutine(OutPutText(GetText(TextBox[DayNum],inBranch)));
                    RunCode();
                }
                
            }
            else
            {
                _talkSys.Type.Stop();
                StopCoroutine(_outPutTextCoroutine);
                _outPutTextCoroutine = null;
                ShowAllText(GetText(TextBox[DayNum],inBranch));
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
        foreach (var obj in _talkSys.CharacterList)
        {
            Character character = obj?.GetComponent<Character>();
            if (character != null && character.CharacterName == TextBox[DayNum].data[curLine].uncomfortType.ToString())
            {
                character.NotComfort = true;
                return;
            }
        }
    }
    

    private void SetCg(int i)
    {
        FullModeState.SetValue(true);
        CgManager.gameObject.SetActive(true);
        CgManager.ShowCg(i);
    }
    
    

    private void SetHistory()
    {
        var tempHistory = new TextHistory();
        tempHistory.Text = GetText(TextBox[DayNum]);
        tempHistory.CharacterName = TextBox[DayNum].data[curLine].speaker.ToString();
        if (TextBox[DayNum].data[curLine].isAside)
        {
            tempHistory.IsASide = true;
            GlobalData.History.SetHistory(tempHistory);
            return;
        }
        tempHistory.IsASide = false;
        tempHistory.IsPlayer = TextBox[DayNum].data[curLine].isPlayerTalking;
        GlobalData.History.SetHistory(tempHistory);
        
    }
    

    private void CheckSpeaker()
    {
        _isPlayerTalk = TextBox[DayNum].data[curLine].isPlayerTalking || TextBox[DayNum].data[curLine].isAside;
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
        foreach (var value in TextBox[DayNum].data[curLine].codes)
        {
            if (value == Code.HideCg)
            {
                CgManager?.HideCg(); // 空值保护
                FullModeState.SetValue(false); // 新增：关闭CG时退出全屏模式
            }
            else
            {
                var code = value.ToString();
                _switchManager.DoSwitchCode(code);
            }
        }
        CheckMiniGame();
        
    }
    
    private void RunCode(Manager manager,int index)
    {
        foreach (var value in manager.data[index].codes)
        {
            if (value == Code.HideCg)
            {
                CgManager?.HideCg(); // 空值保护
                FullModeState.SetValue(false); // 新增：关闭CG时退出全屏模式
            }
            else
            {
                var code = value.ToString();
                _switchManager.DoSwitchCode(code);
            }
            
        }
        
    }

    #region 输出文本
    private IEnumerator OutPutText(string text)
    {
        _isShowingText = true;
        var textUI = GetTextUI();
        if (_autoCoroutine != null)
        {
            StopCoroutine(_autoCoroutine);
        }
        textUI.text = string.Empty;
        
        if(MiniMode) MiniTalkSys?.ShowAllText(GetCurrentCharacterName(), string.Empty);
        foreach (var value in text)
        {
            if (MiniMode && !_isPlayerTalk)
            {
                MiniTalkSys?.ShowText(GetCurrentCharacterName(),value);
            }
            else
            {
                textUI.text += value;
            }
            _talkSys.Type.Play();
            if (_lock)
            {
                yield break;
            }
            yield return new WaitForSeconds(IntervalTime);
        }
        if (!CheckBranch())
        {
            curLine++;
        }
        _isShowingText = false;
        if (autoPlay.GeneralBool && !_lock && !OnHistory && !FullModeState.GetValue(true))
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
        }
        if (MiniMode && !_isPlayerTalk)
        {
            MiniTalkSys?.ShowAllText(GetCurrentCharacterName(), string.Empty);
            MiniTalkSys?.ShowAllText(GetCurrentCharacterName(),text);
        }
        else
        {
            textUI.text = text;
        }
        if (autoPlay.GeneralBool && !_lock && !OnHistory && !FullModeState.GetValue(true))
        {
            _autoCoroutine = StartCoroutine(AutoPlayNext(text.Length));
        }
    }
    
    private IEnumerator AutoPlayNext(int textLength)
    {
        // 根据文本长度计算基础阅读时间（至少1秒）
        float baseReadTime = Mathf.Max(1f, IntervalTime * textLength * 0.6f);
    
        // 用 Weight 调节速度：Weight越大，等待越短
        float speedFactor = Mathf.Max(0.2f, 1f - ((autoPlay.Weight - 1) * 0.15f));
        float waitTime = baseReadTime * speedFactor;
    
        yield return new WaitForSeconds(waitTime);

        // 再次确认状态，防止期间被手动操作打断
        if (autoPlay.GeneralBool && !_lock && !_isShowingText && !OnHistory && !FullModeState.GetValue(true))
        {
            ShowText();
        }
    }

    #endregion

    // 封装获取当前行分支信息
    private (List<string> branchList, bool hasBranch) GetCurrentBranchData()
    {
        var lineData = TextBox[DayNum].data[curLine];
        return Language.isEn? (lineData.enBranch, lineData.enHaveBranch) : (lineData.cnBranch, lineData.cnHaveBranch);
    }

    private bool CheckBranch()
    {
        var (branchList, hasBranch) = GetCurrentBranchData();
        if (inBranch)
        {
            branchLine++;
            if (branchLine >= branchList.Count)
            {
                inBranch = false;
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
        var v = TextBox[DayNum].data[curLine].minigameType;
        if ( v != CharacterType.Player)
        {
            switch (v)
            {
                case CharacterType.艾米莉:
                    GlobalData.ShowText.aimiGame.SetActive(true);
                    break;
                case CharacterType.阿曼德:
                    GlobalData.ShowText.amandeGame.SetActive(true);
                    break;
                case CharacterType.博金森:
                    GlobalData.ShowText.boGame.SetActive(true);
                    break;
                case CharacterType.洛尔坎:
                    GlobalData.ShowText.luoGame.SetActive(true);
                    break;
                case CharacterType.莱文:
                    GlobalData.ShowText.laiWenGame.SetActive(true);
                    break;
                case CharacterType.商人:
                    Debug.LogError($"小游戏配置错误 Day: {DayNum}");
                    break;
            }
        }
    }


    private string GetText(Manager manager , bool inBranch = false)
    {
        string s;
        if (!inBranch)
        {
            if (!GlobalData.Language.isEn)
            {
                s = manager.data[curLine].cn;
            }
            else
            {
                s = manager.data[curLine].en;
            }
            
        }
        else
        {
            if (!GlobalData.Language.isEn)
            {
                s = manager.data[curLine].cnBranch[branchLine];
            }
            else
            {
                s = manager.data[curLine].enBranch[branchLine];
            }
        }
        
        SetCharaName(manager);
        s = s.Replace("{PlayerName}", PlayerNameBox);
        s = s.Replace("{DeadName}", GetDeadName());
        s = s.Replace("{DeadNum}", GetDeadNumber().ToString());
        s = s.Replace("{Person}", GetPerson());
        return s;
    }

    private string GetPerson()
    {
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
        int v = 0;
        foreach (var value in GlobalData.Characters)
        {
            if (value.Dead)
            {
                v++;
            }
        }
        return v;
    }

    private string GetDeadName(bool returnCn = false)
    {
        var v = string.Empty;
        foreach (var value in GlobalData.Characters)
        {
            if (value.Dead)
            {
                if (v == string.Empty)
                {
                    v = CharacterName.GetCharaName(value.CharacterName,returnCn);
                }
                else
                {
                    v += ", " + CharacterName.GetCharaName(value.CharacterName,returnCn);
                }
            }
        }

        return v;
    }
    

    private string SwitchText(string text)
    {
        var s = text;
        var inTag = false;
        var code = string.Empty;
        foreach (var value in s)
        {
            if (value == '{')
            {
                inTag = true;
                continue;
            }

            if (value == '}')
            {
                inTag = false;
                break;
            }
            
            if (inTag)
            {
                code += value;
            }
        }

        switch (code)
        {
            case "PlayerName":
                s = s.Replace("{PlayerName}", PlayerNameBox);
                break;
            case "DeadName":
                break;
        }
        
        return s;
    }
    

    private TextMeshProUGUI GetTextUI()
    {
        if (_inShop)
        {
            return _shopText;
        }
        return _isPlayerTalk ? _talkSys.Player : _talkSys.Character;
    }

    public void SetShopStatus(bool status)
    {
        _inShop = status;
    }

    private void SetCharaName(Manager container)
    {
        string text = string.Empty;

        switch (container.data[curLine].speaker)
        {
            case CharacterType.Player:
                return;
            case CharacterType.阿曼德:
                text = Language.isEn ? "Amanda" : "阿曼德";
                break;
            case CharacterType.艾米莉:
                text = Language.isEn ? "Emily" : "艾米莉";
                break;
            case CharacterType.博金森:
                text = Language.isEn ? "Bokinson" : "博金森";
                break;
            case CharacterType.莱文:
                text = Language.isEn ? "Levine" : "莱文";
                break;
            case CharacterType.洛尔坎:
                text = Language.isEn ? "Lorquin" : "洛尔坎";
                break;
        }
        if (_inShop)
        {
            if (container.data[curLine].isAside)
            {
                _shopName.text = "";
            }
            else
            {
                _shopName.text = container.data[curLine].isPlayerTalking ? PlayerNameBox : (Language.isEn ? "Merchant" : "商人");
            }
            
        }
        
        if (!_isPlayerTalk)
        {
            if (MiniMode)
            {
                return;
            }
            int expression = container.data[curLine].expression;
            if (expression != 0)
            {
                _talkSys?.SwitchExpression(container.data[curLine].speaker.ToString(), container.data[curLine].expression);
            }
            _talkSys?.CharacterImageManager?.SetImage(container.data[curLine].speaker.ToString());
            _talkSys.Chara_Name.text = text;
            
        }
    }

    private string GetCurrentCharacterName()
    {
        return TextBox[DayNum].data[curLine].speaker.ToString();
    }

    public void  SetChoiceLine(int line,bool resetHistory)
    {
        curLine =  line;
        if (resetHistory)
        {
            GlobalData.History.Refresh();
        }
        _isShowingText = false;
        var textUI = GetTextUI();
        textUI.text = string.Empty;
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
        RunCode(SkipManager,0);
    }
    
}
