using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TalkSysShowText : MonoBehaviour,ITalkSysCore
{
    private TalkSystem _talkSys;
    private TalkSysSwitch _switchManager;
    private List<Manager> _talkLines;
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
    
    public void Init(TalkSystem talkSys)
    {
        _talkSys = talkSys;
        _talkLines = talkSys.Talklines;
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
        if (_currentCoroutine!=null)
        {
            StopOutputText();
            _talkSys.line++;
            return;
        }
        while (true)
        {
            string curText = _talkLines[DayNum].TxtLine[LineIndex];
            
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
            
            if (curText.Contains("$"))
            {
                _switchManager.DoSwitchCode();
                _talkSys.line++;
                continue;
            }
        
            
            CheckTextUI();
            TextUI.text = string.Empty;
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
        
        var tempString = _talkLines[DayNum].TxtLine[LineIndex];
        var charaName = string.Empty;
        if (!_isPlayerTalking)
        {
            string[] tempTextBox;
            tempTextBox = HandleCharacterName(tempString);
            charaName = tempTextBox[0];
            tempString = tempTextBox[1];
            if (InShop)
            {
                _shopGeneralName.text = charaName;
            }
            else
            {
                _characterName.text = charaName;
            }
        }
        else
        {
            if (InShop)
            {
                _shopGeneralName.text = PlayerNameBox;
            }
            else
            {
                _playerName.text = PlayerNameBox;
            }
        }
        

        foreach (var stringValue in tempString)
        {
            if (!onMiniMode)
            {
                TextUI.text += stringValue;
            }
            else
            {
                _miniCharacterTalkSys.ShowText(charaName, stringValue);
            }
            
            yield return new WaitForSeconds(_intervalTime);
            
        }

        _talkSys.line++;
        _currentCoroutine = null;

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
            if (nameString == ':')
            {
                break;
            }
            textBox[0] += nameString;
        }

        textBox[1] = textBox[1].Replace($"{textBox[0]}:", "");

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

    private void ShowAllText()
    {
        if (!_isPlayerTalking)
        {
            _currentTextUI.text = HandleCharacterName(_talkLines[DayNum].TxtLine[LineIndex])[1];
        }
        _currentTextUI.text = _talkLines[DayNum].TxtLine[LineIndex];
        
    }
    
    
}
