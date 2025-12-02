using System;
using System.Collections.Generic;
using UnityEngine;

public enum SwitchCode
{
    NormalChoice,
    ShopChoice,
    ShopMiddleChoice,

}



public class TalkSysSwitch : MonoBehaviour,ITalkSysCore
{
    
    private TalkSystem _talkSys;
    private List<Manager> _talkLines;
    private int DayNum => _talkSys?.Daytime ?? 0;
    private int _line;

    public TalkSysUIButtonFunc buttonManager;
    

    /// <summary>
    /// 初始化TalkSysSwitch对象，设置对话系统和对话行列表，并重置当前行索引。
    /// </summary>
    /// <param name="talkSys">对话系统的实例，包含所有对话数据和配置。</param>
    public void Init(TalkSystem talkSys)
    {
        _talkSys = talkSys;
       _talkLines = talkSys.Talklines;
       buttonManager = talkSys.buttonFunc;
       _line = 0;

    }

    /// <summary>
    /// 执行特定操作，根据当前对话行中的命令来切换场景中的选择按钮状态。
    /// 该方法解析当前对话行文本以确定要执行的命令，并基于此命令调用相应的场景切换逻辑。
    /// </summary>
    /// <remarks>此方法内部处理异常情况，确保即使在遇到错误时也能继续执行。</remarks>
    public void DoSwitchCode()
    {
        //命令命名规范  ${命令}
        try
        {
            string curText = _talkLines[DayNum].TxtLine[_line];
            curText = curText.Replace("$", "");
            curText = curText.Replace("{", "");
            curText = curText.Replace("}", "");
            bool isSuccess = Enum.TryParse<SwitchCode>(curText, ignoreCase: true, out var tempCode);
            if (!isSuccess)
            {
                Debug.LogError($"文本内置标记错误:{curText}");
                return;
            }
            switch (tempCode)
            {
                //选项逻辑处理
                case SwitchCode.NormalChoice:
                    SwitchChoiceScence();
                    break;
                case SwitchCode.ShopChoice:
                    SwitchChoiceScence(true);
                    break;
                case SwitchCode.ShopMiddleChoice:
                    SwitchChoiceScence(true, true);
                    break;
                
            }

            _line++;

        }
        catch (Exception e)
        {
            Debug.LogError(e);
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
        if (!inShop)
        {
            buttonManager.SwitchButtonState(ButtonName.Left,ButtonAction.Enable,_talkLines[DayNum].Option1);
            buttonManager.SwitchButtonState(ButtonName.Right,ButtonAction.Enable,_talkLines[DayNum].Option2);
            return;
        }

        buttonManager.SwitchButtonState(ButtonName.ShopLeft, ButtonAction.Enable, _talkLines[DayNum].Option1);
        if (middleOn)
        {
            buttonManager.SwitchButtonState(ButtonName.ShopRight, ButtonAction.Enable, _talkLines[DayNum].Option3);
            buttonManager.SwitchButtonState(ButtonName.ShopMiddle,ButtonAction.Enable,_talkLines[DayNum].Option2);
            return;
        }
        buttonManager.SwitchButtonState(ButtonName.ShopRight, ButtonAction.Enable, _talkLines[DayNum].Option2);

    }

    private void SetChoice(Manager lineBox)
    {
        _line = 0;
        _talkLines[DayNum] = lineBox;
    }

    /// <summary>
    /// 根据提供的选项来切换当前对话线路。
    /// </summary>
    /// <param name="option">一个整数，代表用户选择的对话分支选项。支持1, 2, 或3作为有效输入。</param>
    void TurnOption(int option)
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
