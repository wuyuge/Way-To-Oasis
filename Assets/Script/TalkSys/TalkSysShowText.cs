using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TalkSysShowText : MonoBehaviour,ITalkSysCore
{
    private TalkSystem _talkSys;
    private TalkSysSwitch _switchManager;
    private List<Manager> TalkLines => _talkSys.Talklines;
    private int LineIndex => _talkSys.line;
    private int DayNum => _talkSys.Daytime;
    private float _intervalTime;
    private bool _isPlayerTalking;
    private bool InShop => _talkSys._inshop;
    private bool MiniMode => _talkSys.MiniMode;
    private MiniCharacterTalkSys _miniCharacterTalkSys;
    private TextMeshProUGUI _currentTextUI;
    private TextMeshProUGUI TextUI => _currentTextUI;
    private TextMeshProUGUI _playerName,_characterName,_shopGeneralName;
    private Coroutine _currentCoroutine;
    private string PlayerNameBox => _talkSys.PlayerName.TxtLine[0];
    //一次性开关用于阻止指令执行完毕后继续读取下一条
    private int _stopCommend = 0;
    public bool CanShowText { get; set; }
    [Header("是否显示Debug")] public bool showDebug;
    [Header("历史对话")] public HistoryManager historyManager;
    [Header("自动播放")] public Manager autoplay;
    [Header("自动播放延迟")] public float autoPlayDelay;
    private void Awake()
    {
        CanShowText = true;
    }

    public void Init(TalkSystem talkSys)
    {
        _talkSys = talkSys;
        _intervalTime = talkSys.TextSpeedI;
        _switchManager = talkSys.switchManager;
        _miniCharacterTalkSys = talkSys.MiniCharacterManager;
        _playerName = talkSys.PlayerNameText;
        _shopGeneralName = talkSys.ShopName;
        _characterName = talkSys.Chara_Name;

    }


    /// <summary>
    /// 显示当前对话行的文本到UI。该方法首先检查当前文本是否包含特殊命令（以"$"开头），如果包含则调用开关管理器执行相应的代码切换。
    /// 对于指示说话者变化的特定字符串（如"->p"表示玩家开始说话，"->c"表示角色开始说话），会更新说话者状态并递归调用自身以显示下一个对话行。
    /// 最后，清空当前文本UI的内容，并根据当前场景和说话者状态选择合适的UI组件后，通过协程逐字符输出文本。
    /// </summary>
    public void ShowText()
    {
        
        if(!CanShowText) return;
        
        while (true)
        {
            if (_stopCommend > 0)
            {
                _stopCommend--;
                Debug.Log("停止执行下条命令");
                return;
            }
            string curText;
            try
            {
                curText = TalkLines?[DayNum]?.TxtLine?[LineIndex] ?? string.Empty;
                if (TalkLines[DayNum].TxtLine[LineIndex].Contains("{PlayerName}"))
                {
                    TalkLines[DayNum].TxtLine[LineIndex] =
                        TalkLines[DayNum].TxtLine[LineIndex].Replace("{PlayerName}", PlayerNameBox);
                    Debug.Log("替换玩家姓名");
                }

            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine($"索引越界:{e}");
                return;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }
            
            if (_currentCoroutine!=null)
            {
                StopOutputText();
                _talkSys.line++;
                return;
            }
            
            //固定转换说话人标识
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
            
            if (curText.Contains("$"))//通用命令
            {
                if (curText.Contains("DownTalkBox"))
                {
                    Debug.Log("重置对话历史");
                    historyManager.Refresh();
                }
                _switchManager.DoSwitchCode();
                _talkSys.line++;
                continue;
            }

            if (curText.Contains("#"))//安抚标记
            {
                SetUnComfort(curText);
                _talkSys.line++;
                continue;
            }

            
        
            
            CheckTextUI();
            _currentTextUI.text = string.Empty;
            _currentCoroutine = StartCoroutine(OutPutText(MiniMode));
            break;
        }
        
    }

    /// <summary>
    /// 确定当前应使用的文本UI元素。根据是否在商店场景中，或者当前说话者是玩家还是角色来选择正确的UI组件。
    /// </summary>
    private void CheckTextUI()
    {
        if (InShop)
        {
            _currentTextUI = _talkSys.ShopTextBar.GetComponent<TextMeshProUGUI>();
            return;
        }

        if (_isPlayerTalking)
        {
            _currentTextUI = _talkSys.Player;
            return;
        }

        _currentTextUI = _talkSys.Character;
    }


    /// <summary>
    /// 逐字符输出文本到UI，支持迷你模式显示。
    /// </summary>
    /// <param name="onMiniMode">是否启用迷你模式，默认为false。</param>
    /// <returns>返回一个IEnumerator，用于Unity的协程处理。</returns>
    private IEnumerator OutPutText(bool onMiniMode = false)
    {
        
        var tempString = TalkLines[DayNum].TxtLine[LineIndex];
        var tempHistory = new TextHistory();
        var charaName = string.Empty;
        if (tempString.Contains("："))//如果文本分类错误即转换说话人
        {
            _isPlayerTalking = false;
            
        }
        CheckTextUI();
        if (!_isPlayerTalking)
        {
            string[] tempTextBox = HandleCharacterName(tempString);
            charaName = tempTextBox[0];
            tempString = tempTextBox[1];
            tempHistory.IsPlayer = false;
            tempHistory.CharacterName = charaName;
            if (tempString.Contains("@"))
            {
                tempString = SetExpression(tempString,charaName);
            }
            if (InShop)
            {
                _shopGeneralName.text = "商人";
            }
            else
            {
                _characterName.text = charaName;
                if (!onMiniMode)_talkSys.CharacterImageManager.SetImage(charaName);
            }
        }
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

        if (onMiniMode)_miniCharacterTalkSys.ShowText(charaName, string.Empty);

        tempHistory.Text = tempString;
        historyManager.SetHistory(tempHistory);
        foreach (var stringValue in tempString)
        {
            
            _talkSys.Type.Play();
            if (!onMiniMode)
            {
                _currentTextUI.text += stringValue;
            }
            else
            {
                if (_isPlayerTalking)
                {
                    _currentTextUI.text += stringValue;
                }
                _miniCharacterTalkSys.ShowText(charaName, stringValue);
            }
            
            yield return new WaitForSeconds(_intervalTime);
            
        }
        
        _talkSys.Type.Stop();
        _talkSys.line++;
        _currentCoroutine = null;
        if (autoplay.GeneralBool && TalkLines[DayNum].TxtLine[LineIndex] is not null)
        {
            Invoke(nameof(ShowText),autoPlayDelay);
        }

    }

    /// <summary>
    /// 处理初始文本以分离角色名称和实际对话内容。
    /// </summary>
    /// <param name="initialText">包含角色名称和对话内容的原始字符串。</param>
    /// <returns>一个字符串数组，其中第一个元素是角色名称，第二个元素是去除角色名称后的对话内容。</returns>
    string[] HandleCharacterName(string initialText)
    {
        string[] textBox = new string[2];
        textBox[1] = initialText;
        foreach (var nameString in initialText)
        {
            if (nameString == '：')
            {
                break;
            }
            textBox[0] += nameString;
        }

        textBox[1] = textBox[1].Replace($"{textBox[0]}：", "");
        
        return textBox;
    }


    /// <summary>
    /// 停止当前正在输出的文本。如果存在正在进行的文本输出协程，则停止该协程，并将当前协程引用设置为null。
    /// </summary>
    public void StopOutputText()
    {
        if (_currentCoroutine != null)
        {
            
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
            ShowAllText();
        }
    }

    /// <summary>
    /// 显示当前对话行的全部文本到UI。根据是否处于迷你模式以及当前说话者状态，该方法会处理并显示相应的文本。
    /// 如果当前不是玩家在说话且处于迷你模式，则调用迷你角色对话系统显示经过处理后的文本。
    /// 否则，直接将处理后的文本（包括可能的表情设置）或原始文本显示到当前文本UI上。
    /// </summary>
    private void ShowAllText()
    {
        if (!_isPlayerTalking)
        {
            if (MiniMode)
            {
                string[] text;
                text = HandleCharacterName(TalkLines[DayNum].TxtLine[LineIndex]);
                _miniCharacterTalkSys.ShowText(text[0], string.Empty);
                _miniCharacterTalkSys.ShowText(text[0], text[1]);
                return;
            }
            _currentTextUI.text = string.Empty;
            string[] tempString = HandleCharacterName(TalkLines[DayNum].TxtLine[LineIndex]);
            if (tempString[1].Contains("@"))
            {
                tempString[1] = SetExpression(tempString[1],tempString[0]);
            }
            _currentTextUI.text = tempString[1];
            
            return;
        }
        _currentTextUI.text = string.Empty;
        _currentTextUI.text = TalkLines[DayNum].TxtLine[LineIndex];
        
    }

    public void SetEmptyText()
    {
        _talkSys.Character.text = string.Empty;
        _talkSys.Player.text = string.Empty;
        _talkSys.ShopTextBar.GetComponent<TextMeshProUGUI>().text = string.Empty;
    }


    public void StopNextCommend()
    {
        Debug.Log("开始禁止下条指令");
        _stopCommend++;
    }

    private string SetExpression(string text,string characterName)
    {
        //固定表情标记格式 @{内容}
        int startIndex = 0, endIndex = 0;
        bool startFound = false;
        foreach (var value in text)
        {
            if (value == '@')
            {
                startFound = true;
            }

            if (value == '}')
            {
                break;
            }

            if (!startFound) startIndex++;
            endIndex++;
        }

        var expression = text.Substring(startIndex + 2, endIndex - startIndex - 2);
        text = text.Replace("@{" + $"{expression}" + "}", "");
        if (showDebug)Debug.Log($"表情{expression} , 输出文本:{text}");
        _talkSys.SwitchExpression(characterName,expression);
        return text;
    }

    /// <summary>
    /// 设置指定角色为不安状态。此方法首先移除文本中的特殊字符（"#")，然后根据提供的角色英文名转换为中文名。
    /// 如果角色名匹配成功，则在角色列表中查找该角色，并将其不安状态设置为true。
    /// 若未找到对应的角色或传入的角色名不正确，则记录错误信息。
    /// </summary>
    /// <param name="text">需要被安抚的角色的英文名，例如"Aimi"代表艾米莉。</param>
    private void SetUnComfort(string text)
    {
        text = text.Replace("#", "");
        string comfortCharaName;
        switch (text)
        {
            case "Aimi":
                comfortCharaName = "艾米莉";
                break;
            case "Laiwen":
                comfortCharaName = "莱文";
                break;
            case "Luo":
                comfortCharaName = "洛尔坎";
                break;
            case "Bo":
                comfortCharaName = "博金森";
                break;
            case "Amande":
                comfortCharaName = "阿曼德";
                break;
            default:
                Debug.LogError($"安抚角色名错误 错误字段:{text}");
                return;
        }

        foreach (var value in _talkSys.CharacterList)
        {
            Character temp = value.GetComponent<Character>();
            if (temp.CharacterName == comfortCharaName)
            {
                temp.NotComfort = true;
                return;
            }
        }
        
    }
    

}
