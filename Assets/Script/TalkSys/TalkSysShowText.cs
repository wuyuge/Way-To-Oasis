using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Serialization;

// 新增：角色名映射配置（替代硬编码）
[Serializable]
public class CharacterNameMapping
{
    public string EnglishName;
    public string ChineseName;
}

public class TalkSysShowText : MonoBehaviour, ITalkSysCore
{
    private TalkSystem _talkSys;
    private TalkSysSwitch _switchManager;
    private List<Manager> TalkLines => _talkSys.Talklines;
    private int LineIndex => _talkSys.line;
    private int DayNum => _talkSys.Daytime;
    private float IntervalTime => _talkSys.TextSpeedI;
    private bool _isPlayerTalking;
    private bool InShop => _talkSys._inshop;
    private bool MiniMode => _talkSys.MiniMode;
    private MiniCharacterTalkSys _miniCharacterTalkSys;
    private TextMeshProUGUI _currentTextUI;
    private TextMeshProUGUI TextUI => _currentTextUI;
    private TextMeshProUGUI _playerName, _characterName, _shopGeneralName;
    private Coroutine _currentCoroutine;
    private Coroutine _autoPlayCoroutine; // 新增：管理自动播放协程
    private string PlayerNameBox => _talkSys.PlayerName.TxtLine[0];
    private int _stopCommend = 0;
    private const string CgPattern = @"CG\d+"; //CG标签
    private const string AsidePattern = @"A_";//旁白标签
    private const string ExpressionPattern =  @"@\{(\d+)\}"; //表情解析标签
    private const string MiniPattern = @"^MINI\d{1}$";//迷你游戏标签
    public bool CanShowText { get; set; }

    [Header("调试配置")]
    public bool showDebug;

    [Header("功能依赖")]
    public HistoryManager historyManager;
    public Manager autoplay;
    public float autoPlayDelay;
    public CgManager cgManager;
    public Manager skipManager;
    public GameObject aimiGame, amandeGame, boGame, luoGame;
    public GameObject laiWenGame;

    [Header("角色名映射（替代硬编码）")] // 新增：可视化配置角色名
    public List<CharacterNameMapping> characterNameMappings;

    #region 初始化注入

    private void Awake()
    {
        CanShowText = true;
        GlobalData.ShowText = this;
        if (GlobalData.History == null)
        {
            GlobalData.History = historyManager;
        }
    }

    public void Init(TalkSystem talkSys)
    {
        if (talkSys == null)
        {
            Debug.LogError("传入的 talkSys 参数为 null");
            return;
        }
    
        _talkSys = talkSys;
        _switchManager = talkSys.switchManager;
        _miniCharacterTalkSys = talkSys.MiniCharacterManager;
        _playerName = talkSys.PlayerNameText;
        _shopGeneralName = talkSys.ShopName;
        _characterName = talkSys.Chara_Name;
    
        // 添加验证日志
        Debug.Log($"TalkSysShowText 初始化成功: _talkSys={_talkSys != null}, " +
                  $"playerName={_playerName != null}, characterName={_characterName != null}");
    }
    
    private void OnEnable()
    {
        TalkSysStaticData.TalkSysShowText = this;
    }
    #endregion
    #region 主逻辑循环

    public void ShowText()
    {
        if (!CanShowText || FullModeState.GetValue(isFullMode: true))
            return;

        while (true)
        {
            // 指令阻断逻辑
            if (_stopCommend > 0)
            {
                _stopCommend--;
                Debug.Log("停止执行下条命令");
                return;
            }

            string curText;
            try
            {
                // 核心优化：增加多层空值+长度检查，避免索引越界
                if (TalkLines == null || DayNum < 0 || DayNum >= TalkLines.Count ||
                    TalkLines[DayNum]?.TxtLine == null || LineIndex < 0 || LineIndex >= TalkLines[DayNum].TxtLine.Count)
                {
                    Debug.Log("对话数据已遍历完毕");
                    return;
                }

                curText = TalkLines[DayNum].TxtLine[LineIndex];
                // 玩家名替换（仅修改临时变量）
                curText = curText.Replace("{PlayerName}", PlayerNameBox);
            }
            catch (Exception e)
            {
                Debug.LogError($"读取对话文本失败：{e.Message}");
                return;
            }

            
            // 停止当前协程（快速跳过逻辑）
            if (_currentCoroutine != null)
            {
                StopOutputText();
                _talkSys.line++;
                return;
            }

            // 说话人切换指令
            if (curText == "->p")
            {
                _isPlayerTalking = true;
                _talkSys.line++;
                continue;
            }
            if (curText == "->c")
            {
                _isPlayerTalking = false;
                _talkSys.line++;
                continue;
            }

            // 通用命令处理
            if (curText.Contains("$"))
            {
                if (curText.Contains("DownTalkBox"))
                {
                    Debug.Log("重置对话历史");
                    historyManager?.Refresh(); // 空值保护
                }
                _switchManager?.DoSwitchCode(); // 空值保护
                _talkSys.line++;
                continue;
            }

            // 安抚标记处理
            if (curText.Contains("#"))
            {
                SetUnComfort(curText);
                _talkSys.line++;
                continue;
            }

            // CG指令处理
            if (Regex.IsMatch(curText, CgPattern))
            {
                ShowCg(curText);
                _talkSys.line++;
                ShowText();
                return;
            }
            if (curText == "HideCg")
            {
                CloseCg();
                _talkSys.line++;
                continue;
            }
            
            //迷你游戏指令处理
            if (Regex.IsMatch(curText,MiniPattern))
            {
                _talkSys.line++;
                CanShowText = false;
                if (SwitchMiniGame(curText))
                {
                    return;
                }
                continue;
            }

            
            

            // 显示文本逻辑
            CheckTextUI();
            _currentTextUI.text = string.Empty;
            _currentCoroutine = StartCoroutine(OutPutText(MiniMode));
            break;
        }
    }

    #endregion
    #region UI依赖

    private void CheckTextUI()
    {
        if (InShop)
        {
            _currentTextUI = _talkSys.ShopTextBar?.GetComponent<TextMeshProUGUI>();
        }
        else if (_isPlayerTalking)
        {
            _currentTextUI = _talkSys.Player;
        }
        else
        {
            _currentTextUI = _talkSys.Character;
        }

        // 空值保护：防止UI未赋值导致空引用
        if (_currentTextUI == null)
        {
            Debug.LogError("当前文本UI组件未找到！");
            _currentTextUI = GetComponent<TextMeshProUGUI>(); // 降级处理
        }
    }

    #endregion
    #region 对话管理

    /// <summary>
    /// 核心优化：
    /// 1. 自动播放改用协程管理，避免Invoke的多次调用问题
    /// 2. 所有外部调用增加空值检查
    /// 3. 简化文本处理逻辑
    /// </summary>
    private IEnumerator OutPutText(bool onMiniMode = false)
    {
        // 前置检查：确保对话数据有效
        if (!IsTalkDataValid())
        {
            _currentCoroutine = null;
            yield break;
        }
        
        
        string originalText = TalkLines[DayNum].TxtLine[LineIndex];
        var tempHistory = new TextHistory();
        string charaName = string.Empty;
        string displayText = originalText;

        if (originalText.Contains("$"))
        {
            ShowText();
            yield break;
        }

        if (displayText == "->p" || displayText == "->c")
        {
            yield break;
        }
        // 说话人校正
        if (displayText.Contains("："))
        {
            _isPlayerTalking = false;
        }

        CheckTextUI();
        
        //替换已死亡角色名字
        if (displayText.Contains("{DeadName}"))
        {
            var addedText = "";
            var deadName = new List<string>();
            foreach (var value in _talkSys.CharacterList)
            {
                var component = value.GetComponent<Character>();
                if(!component.Dead) continue;
                deadName.Add(component.CharacterName);
            }

            for (int i = 0; i < deadName.Count; i++)
            {
                addedText += deadName[i];
                if (i < deadName.Count)
                {
                    addedText += ',';
                }
            }
            displayText = displayText.Replace("{DeadName}", addedText);
        }
        
        // 旁白处理
        if (Regex.IsMatch(displayText, AsidePattern))
        {
            tempHistory.IsASide = true;
            displayText = displayText.Replace("A_", string.Empty);
        }
        else
        {
            tempHistory.IsASide = false;
        }

        // 角色说话逻辑
        if (!_isPlayerTalking)
        {
            var nameAndText = HandleCharacterName(displayText);
            charaName = nameAndText.Item1;
            displayText = nameAndText.Item2;

            tempHistory.IsPlayer = false;
            tempHistory.CharacterName = charaName;

            // 表情解析（改用正则，简化逻辑）
            if (displayText.Contains("@"))
            {
                displayText = SetExpression(displayText, charaName);
            }

            // 商店模式处理
            if (InShop)
            {
                _shopGeneralName.text = "商人";
                tempHistory.CharacterName = "商人";
            }
            else
            {
                _characterName.text = charaName;
                if (!onMiniMode)
                {
                    _talkSys.CharacterImageManager?.SetImage(charaName); // 空值保护
                }
            }
        }
        // 玩家说话逻辑
        else
        {
            tempHistory.IsPlayer = true;
            tempHistory.CharacterName = string.Empty;

            if (InShop)
            {
                _shopGeneralName.text = PlayerNameBox;
            }
            else
            {
                _playerName.text = PlayerNameBox;
            }
        }

        if (MiniMode)
        {
            _miniCharacterTalkSys?.ShowAllText(charaName, string.Empty);
        }

        

        // 历史记录存储
        if (displayText.Contains("{PlayerName}"))
        {
            displayText = displayText.Replace("{PlayerName}", PlayerNameBox);
        }
        tempHistory.Text = displayText;
        if (!string.IsNullOrWhiteSpace(tempHistory.Text))
        {
            historyManager?.SetHistory(tempHistory); // 空值保护
        }

        // 打字机效果
        float actualInterval = IntervalTime - (IntervalTime * ((autoplay.Weight - 1) * 0.1f));
        actualInterval = Mathf.Max(0.01f, actualInterval); // 防止间隔为0

        
        foreach (var c in displayText)
        {
            _talkSys.Type?.Play(); // 空值保护

            if (!onMiniMode)
            {
                _currentTextUI.text += c;
            }
            else
            {
                if (_isPlayerTalking)
                {
                    _currentTextUI.text += c;
                }
                _miniCharacterTalkSys?.ShowText(charaName, c); // 空值保护
            }

            yield return new WaitForSeconds(actualInterval);
        }

        // 播放结束清理
        _talkSys.Type?.Stop(); // 空值保护
        _talkSys.line++;
        _currentCoroutine = null;

        // 核心优化：自动播放改用协程，可取消
        if (autoplay.GeneralBool && IsTalkDataValid() && !GlobalData.TalkSystem.useNewSys)
        {
            if (_autoPlayCoroutine != null)
            {
                StopCoroutine(_autoPlayCoroutine);
            }
            _autoPlayCoroutine = StartCoroutine(AutoPlayNextLine(autoPlayDelay / autoplay.Weight));
        }
    }

    /// <summary>
    /// 新增：自动播放协程（替代Invoke）
    /// </summary>
    private IEnumerator AutoPlayNextLine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowText();
        _autoPlayCoroutine = null;
    }

    /// <summary>
    /// 核心优化：
    /// 1. 改用Tuple返回值，更清晰
    /// 2. 支持全角/半角冒号
    /// 3. 简化拆分逻辑
    /// </summary>
    private (string Name, string Text) HandleCharacterName(string initialText)
    {
        string charaName = string.Empty;
        string text = initialText;

        // 支持全角：和半角:
        int colonIndex = initialText.IndexOf('：');
        if (colonIndex == -1)
        {
            colonIndex = initialText.IndexOf(':');
        }

        if (colonIndex > 0)
        {
            charaName = initialText.Substring(0, colonIndex);
            text = initialText.Substring(colonIndex + 1);
        }

        return (charaName, text);
    }

    public void StopOutputText()
    {
        
        
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }

        // 停止自动播放协程
        if (_autoPlayCoroutine != null)
        {
            StopCoroutine(_autoPlayCoroutine);
            _autoPlayCoroutine = null;
        }

        ShowAllText();
    }

    /// <summary>
    /// 核心优化：
    /// 1. 增加数据有效性检查
    /// 2. 复用HandleCharacterName方法，减少冗余
    /// </summary>
    private void ShowAllText()
    {
        if (!IsTalkDataValid() || _currentTextUI == null)
            return;

        _currentTextUI.text = string.Empty;
        string originalText = TalkLines[DayNum].TxtLine[LineIndex];
        string displayText = originalText;
        
        if (originalText.Contains("$"))
        {
            ShowText();
            return;
        }

        if (!_isPlayerTalking)
        {
            var nameAndText = HandleCharacterName(originalText);
            displayText = nameAndText.Text;

            // 表情解析
            if (displayText.Contains("@"))
            {
                displayText = SetExpression(displayText, nameAndText.Name);
            }
            
            if (displayText.Contains("{PlayerName}"))
            {
                displayText = displayText.Replace("{PlayerName}", PlayerNameBox);
            }
            
            // 旁白处理
            if (displayText.Contains("A_"))
            {
                displayText = displayText.Replace("A_", string.Empty);
            }
            
            //替换已死亡角色名字
            if (displayText.Contains("{DeadName}"))
            {
                var addedText = "";
                var deadName = new List<string>();
                foreach (var value in _talkSys.CharacterList)
                {
                    var component = value.GetComponent<Character>();
                    if(!component.Dead) continue;
                    deadName.Add(component.CharacterName);
                }

                for (int i = 0; i < deadName.Count; i++)
                {
                    addedText += deadName[i];
                    if (i < deadName.Count)
                    {
                        addedText += ',';
                    }
                }
                displayText = displayText.Replace("{DeadName}", addedText);
            }
            
            // 迷你模式处理
            if (MiniMode)
            {
                _miniCharacterTalkSys?.ShowAllText(nameAndText.Name, string.Empty);
                _miniCharacterTalkSys?.ShowAllText(nameAndText.Name, displayText);
                return;
            }
        }
        
        // 旁白处理
        if (displayText.Contains("A_"))
        {
            displayText = displayText.Replace("A_", string.Empty);
        }
        
        //替换已死亡角色名字
        if (displayText.Contains("{DeadName}"))
        {
            var addedText = "";
            var deadName = new List<string>();
            foreach (var value in _talkSys.CharacterList)
            {
                var component = value.GetComponent<Character>();
                if(!component.Dead) continue;
                deadName.Add(component.CharacterName);
            }

            for (int i = 0; i < deadName.Count; i++)
            {
                addedText += deadName[i];
                if (i < deadName.Count)
                {
                    addedText += ',';
                }
            }
            displayText = displayText.Replace("{DeadName}", addedText);
        }

        _currentTextUI.text = displayText;
    }

    public void SetEmptyText()
    {
        if (_talkSys != null)
        {
            _talkSys.Character.text = string.Empty;
            _talkSys.Player.text = string.Empty;
            if (_talkSys.ShopTextBar != null)
            {
                _talkSys.ShopTextBar.GetComponent<TextMeshProUGUI>().text = string.Empty;
            }
        }
    }
    
    public void Skip()
    {
        // 空值保护
        if (_talkSys == null)
        {
            Debug.LogWarning("Skip 被调用时 _talkSys 已为空，已安全拦截", this);
            return;
        }

        // 安全检查对话数据
        if (_talkSys.Talklines == null || DayNum < 0 || DayNum >= _talkSys.Talklines.Count)
        {
            Debug.LogError("跳过对话失败：对话数据无效", this);
            return;
        }

        _talkSys.Talklines[DayNum] = skipManager;
        _talkSys.line = 0;

        // 停止所有协程，避免冲突
        StopOutputText();
        ShowText();
    }
    /// <summary>
    /// 新增：通用对话数据有效性检查
    /// 避免重复写检查逻辑
    /// </summary>
    private bool IsTalkDataValid()
    {
        return TalkLines != null && DayNum >= 0 && DayNum < TalkLines.Count &&
               TalkLines[DayNum]?.TxtLine != null && LineIndex >= 0 && LineIndex < TalkLines[DayNum].TxtLine.Count;
    }
    #endregion
    #region 人物状态管理
    /// <summary>
    /// 核心优化：
    /// 1. 改用正则解析表情标记，逻辑简化90%
    /// 2. 增加异常处理
    /// </summary>
    private string SetExpression(string text, string characterName)
    {
        
        try
        {
            var match = Regex.Match(text, ExpressionPattern);
            if (match.Success)
            {
                string expression = match.Groups[1].Value;
                text = Regex.Replace(text, ExpressionPattern, string.Empty);
                Debug.Log($"设置表情{expression} {characterName}");
                if (showDebug)
                {
                    Debug.Log($"表情:{expression} , 输出文本:{text}");
                }

                _talkSys?.SwitchExpression(characterName, expression); // 空值保护
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"解析表情标记失败：{e.Message}");
        }

        return text;
    }

    /// <summary>
    /// 核心优化：
    /// 1. 移除硬编码，改用配置表映射角色名
    /// 2. 优化查找逻辑，提升效率
    /// </summary>
    private void SetUnComfort(string text)
    {
        try
        {
            string englishName = text.Replace("#", "").Trim();
            if (string.IsNullOrEmpty(englishName))
            {
                Debug.LogError("安抚角色名不能为空");
                return;
            }

            // 从配置表查找中文名
            var mapping = characterNameMappings.FirstOrDefault(m => m.EnglishName == englishName);
            if (mapping == null)
            {
                Debug.LogError($"未找到角色名映射：{englishName}，请在Inspector中配置");
                return;
            }

            string chineseName = mapping.ChineseName;

            // 查找角色并设置状态
            if (_talkSys?.CharacterList != null)
            {
                foreach (var obj in _talkSys.CharacterList)
                {
                    Character character = obj?.GetComponent<Character>();
                    if (character != null && character.CharacterName == chineseName)
                    {
                        character.NotComfort = true;
                        return;
                    }
                }
            }

            Debug.LogError($"未找到角色：{chineseName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"设置角色不安状态失败：{e.Message}");
        }
    }
    #endregion
    #region Cg管理
    /// <summary>
    /// 核心优化：
    /// 1. 移除throw，改为优雅的错误处理
    /// 2. 增加参数校验
    /// </summary>
    private void ShowCg(string text)
    {
        try
        {
            FullModeState.SetValue(true);
            string indexStr = Regex.Replace(text, "CG", string.Empty);

            if (int.TryParse(indexStr, out int index))
            {
                if (cgManager != null)
                {
                    cgManager.gameObject.SetActive(true);
                    cgManager.ShowCg(index);
                }
                else
                {
                    Debug.LogError("CG管理器未赋值！");
                }
            }
            else
            {
                Debug.LogError($"CG索引格式错误：{text}，正确格式应为CG+数字（如CG1）");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"显示CG失败：{e.Message}");
        }
    }

    public void CloseCg()
    {
        cgManager?.HideCg(); 
        FullModeState.SetValue(false); // 关闭CG时退出全屏模式
    }
    #endregion
    #region 迷你游戏管理
    private bool SwitchMiniGame(string curText)
    {
        
        if (!int.TryParse(curText.Last().ToString(),out var result) || result == 0 || result > 5)
        {
            Debug.LogError($"迷你游戏转换文本错误{curText}",this);
            CanShowText = true;
            return false;
        }
        switch (result)
        {
            case 1:
                aimiGame.SetActive(true);
                break;
            case 2:
                luoGame.SetActive(true);
                break;
            case 3:
                boGame.SetActive(true);
                break;
            case 4:
                amandeGame.SetActive(true);
                break;
            case 5:
                laiWenGame.SetActive(true);
                break;
            
        }
        return true;
    }

    public void CompleteMiniGame()
    {
        CanShowText = true;
        ShowText();
    }
    
    #endregion
    private void OnDestroy()
    {
        if (GlobalData.Day == 0)
        {
            return;
        }
        StopOutputText();
        if (_autoPlayCoroutine != null)
        {
            StopCoroutine(_autoPlayCoroutine);
        }
    }
    
    public void StopNextCommend()
    {
        Debug.Log("开始禁止下条指令");
        _stopCommend++;
    }
    
}