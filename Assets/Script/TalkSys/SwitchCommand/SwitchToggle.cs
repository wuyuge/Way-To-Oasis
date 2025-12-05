using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToggle : SwitchCommand
{
    private TalkSystem _talkSys;
    #region 对话栏与角色栏引用

    private GameObject TalkPanel => _talkSys.gameObject;
    private GameObject CharacterPanel => _talkSys.charabar;

    #endregion
    
    public override void Init(TalkSystem talkSys)
    {
        _talkSys = talkSys;
    }

    public override void Execute(FunctionCode.Function function)
    {
        switch (function)
        {
            case FunctionCode.Function.A:
                //TODO:迷你对话模式布尔值
                break;
            case FunctionCode.Function.B:
                //TODO:对话框动画控制
                break;
            case FunctionCode.Function.C:
                //TODO:角色框动画控制
                break;
            case FunctionCode.Function.D:
                //TODO:切换下一个对话数据
                break;
            case FunctionCode.Function.E:
                //TODO:开关角色分配食物按钮
                break;
            case FunctionCode.Function.F:
                //TODO:开关营火动画
                break;
            case FunctionCode.Function.G:
                //TODO:禁止点击继续对话
                break;
            case FunctionCode.Function.H:
                //TODO:开关迷你角色图像
                break;
            case FunctionCode.Function.I:
                //TODO:关闭遮罩
                break;
            case FunctionCode.Function.J:
                //TODO:全屏黑屏    
                break;
            case FunctionCode.Function.K:
                //TODO:Demo结束
                break;
            case FunctionCode.Function.L:
                //TODO:从教学场景切换到主场景
                break;
        }
    }
    
    
    
    #region UI移动逻辑

    private enum UIMovement
    {
        Up,
        Down
    }
    private enum UIElement
    {
        Talk,
        Character
    }


    private void MoveUI(UIElement element,UIMovement moveValue)
    {
        GameObject temp = element == UIElement.Talk ? TalkPanel : CharacterPanel;
        
        
        switch (moveValue)
        {
            case UIMovement.Up:
                temp.GetComponent<Animator>().SetTrigger(0);
                break;
            case UIMovement.Down:
                temp.GetComponent<Animator>().SetTrigger(1);
                break;
            
        }
        
        
    }
    
    

    #endregion
    
    
    
    
}
