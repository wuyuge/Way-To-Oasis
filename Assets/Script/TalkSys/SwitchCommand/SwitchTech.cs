using System;
using TMPro;
using UnityEngine;

public class SwitchTech : SwitchCommand
{
    private TalkSystem _talkSys;
    [SerializeField]
    private Manager showTech;
    private GameObject _techMask;
    private TechTextList _textList;
    private TextMeshProUGUI _maskText;

    /// <summary>
    /// 初始化SwitchTech类的实例，设置TalkSystem以及相关UI组件。
    /// </summary>
    /// <param name="talkSys">TalkSystem的实例，用于对话系统的管理和控制。</param>
    public override void Init(TalkSystem talkSys)
    {
        _talkSys = talkSys;
        _textList = talkSys.TechTextList;
    }

    /// <summary>
    /// 根据传入的功能代码执行相应的操作，包括显示不同的UI遮罩层以引导用户进行特定的游戏内活动。
    /// </summary>
    /// <param name="function">指定要执行的功能，如打开教学菜单、点击进行对话等。</param>
    public override void Execute(FunctionCode.Function function)
    {
        if (!showTech.GeneralBool)
        {
            //自动下一行
            _talkSys.line++;
            _talkSys.showText.ShowText();
            return;
        }
        
        switch (function)
        {
            case FunctionCode.Function.A:
                //教学菜单
				return;
                /*EnableMask("Menu",_talkSys.Menu);
                _talkSys.showText.StopNextCommend();
                break;*/
            case FunctionCode.Function.B:
                //点击进行对话
				return;
                /*EnableMask("Click",_talkSys.transform.Find("MaskLayer").gameObject);
                _talkSys.showText.StopNextCommend();
                break;*/
            case FunctionCode.Function.C:
                //点击人物头像
                TutorialManager.Controller.ShowTutorial(0);
				return;
                /*EnableMask("Talk",_talkSys.DownBar.transform.Find("MaskLayer").gameObject);
                _talkSys.showText.StopNextCommend();
                _talkSys.showText.CanShowText = true;
                break;*/
            case FunctionCode.Function.D:
                //结束教学
				return;
                /*_techMask.transform.parent.gameObject.GetComponent<MaskManager>().SetClik(true);
                _talkSys.showText.StopNextCommend();
                break;*/
            case FunctionCode.Function.E:
                //右侧栏
				return;
                /*EnableMask("Right",_talkSys.DaytimeOBJ.transform.parent.gameObject);
                _talkSys.showText.StopNextCommend();
                break;*/
            case FunctionCode.Function.F:
                //切换下阶段
                TutorialManager.Controller.ShowTutorial(1);
				return;
                /*EnableMask("Close",_talkSys.DaytimeOBJ);
                break;*/
            case FunctionCode.Function.G:
                //分配食物
                TutorialManager.Controller.ShowTutorial(2);
				return;
                /*nableMask("Food",_talkSys.DownBar.transform.Find("MaskLayer").gameObject);
                break;*/
            case FunctionCode.Function.H:
                //安抚操作
				return;
                /*EnableMask("Comfort",SelectComfortChara());
                break;*/
            case FunctionCode.Function.I:
                //负重分配
                TutorialManager.Controller.ShowTutorial(3);
				return;
                /*EnableMask("Weight",_talkSys.DownBar.transform.Find("MaskLayer").gameObject);
                break;*/
            case FunctionCode.Function.J:
                //使遮罩可以被鼠标点击关闭
                
                break;
            case FunctionCode.Function.K:
                //使遮罩不可以被鼠标点击关闭
                break;
                
            default:
                Debug.LogError("命令对应枚举值错误",this.gameObject);
                break;
        }
    }
    

    /// <summary>
    /// 从TalkSystem的CharacterList中选择一个满足条件的角色作为安抚对象。
    /// 该方法遍历所有角色，查找未死亡且具有Special1或Special2属性的角色。
    /// </summary>
    /// <returns>返回第一个符合条件的角色GameObject。如果没有找到符合条件的角色，则输出错误信息并返回null。</returns>
    private GameObject SelectComfortChara()
    {
        try
        {
            foreach (GameObject character in _talkSys.CharacterList)
            {
                var chara = character.GetComponent<Character>();
                if (!chara.Dead && (chara.Special1 || chara.Special2))
                {
                    return character;
                }
            }
            Debug.LogError("没有安抚对象");
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError($"安抚查找角色发生错误{e}");
            return null;
        }
        
        
    }
    
}
