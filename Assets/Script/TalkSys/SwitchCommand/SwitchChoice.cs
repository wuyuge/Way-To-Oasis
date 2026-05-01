using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;
using TMPro;

[System.Serializable]
public class SwitchChoice :SwitchCommand
{
    private TalkSystem _talkSys;
    private List<Manager> _talkLines;
    private int DayNum => _talkSys?.Daytime ?? 0;
    private TalkSysUIButtonFunc _buttonManager;

    public Manager haveBody, finalBody;
    public override void Init(TalkSystem talkSys)
    {
        _talkSys = talkSys;
        _talkLines = talkSys.Talklines;
        _buttonManager = talkSys.buttonFunc;
    }

    public override void Execute(FunctionCode.Function function)
    {
        switch (function)
        {
            default:
                Debug.LogError($"错误的函数选择{function}");
                return;
            case FunctionCode.Function.A:
                SwitchChoiceScence();
                break;
            case FunctionCode.Function.B:
                SwitchChoiceScence(true);
                break;
            case FunctionCode.Function.C:
                SwitchChoiceScence(true, true);
                break;
        }
        
    }

#region 选择分支逻辑

    /// <summary>
    /// 根据是否在商店中以及是否启用中间按钮来切换场景中的选择按钮状态。
    /// </summary>
    /// <param name="inShop">指示当前操作是否在商店场景中进行，默认为false。</param>
    /// <param name="middleOn">当处于商店场景时，指示是否启用中间的按钮，默认为false。</param>
    private void SwitchChoiceScence(bool inShop = false,bool middleOn = false)
    {
        _talkSys.Player.text = string.Empty;
        if (_talkSys.Daytime != 0)_talkSys.ShopTextBar.GetComponent<TextMeshProUGUI>().text = string.Empty;
        _talkSys.showText.CanShowText = false;
        if (!inShop)
        {
            _buttonManager.SwitchButtonState(ButtonName.Left,ButtonAction.Enable,_talkLines[DayNum].Option1);
            _buttonManager.SwitchButtonState(ButtonName.Right,ButtonAction.Enable,_talkLines[DayNum].Option2);
            return;
        }

        
        _buttonManager.SwitchButtonState(ButtonName.ShopLeft, ButtonAction.Enable, _talkLines[DayNum].Option1);
        if (middleOn)
        {
            if (haveBody.Weight == 0 && finalBody.Weight == 0)
            {
                _buttonManager.SwitchButtonState(ButtonName.ShopLeft, ButtonAction.Disable, _talkLines[DayNum].Option1);
            }
            _buttonManager.SwitchButtonState(ButtonName.ShopRight, ButtonAction.Enable, _talkLines[DayNum].Option3);
            _buttonManager.SwitchButtonState(ButtonName.ShopMiddle,ButtonAction.Enable,_talkLines[DayNum].Option2);
            
            return;
        }
        _buttonManager.SwitchButtonState(ButtonName.ShopRight, ButtonAction.Enable, _talkLines[DayNum].Option2);
        
    }
    /// <summary>
    ///用于外部按钮调用传入文本
    /// </summary>
    /// <param name="lineBox"></param>
    public void SetChoice(Manager lineBox)
    {
        _talkSys.line = 0;
        _talkLines[DayNum] = lineBox;
    }

    /// <summary>
    /// 用于内部自动根据条件切换分支
    /// </summary>
    /// <param name="option">一个整数，代表用户选择的对话分支选项。支持1, 2, 或3作为有效输入。</param>
    private void TurnOption(int option)
    {
        switch (option)
        {
            case 1:
                _talkLines[DayNum] = _talkLines[DayNum].Option1;
                break;
            case 2:
                _talkLines[DayNum] = _talkLines[DayNum].Option2;
                break;
            case 3:
                _talkLines[DayNum] = _talkLines[DayNum].Option3;
                break;
        }
    }
    
    
    #endregion
    
    
}

