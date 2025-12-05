using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTech : SwitchCommand
{
    private TalkSystem _talkSys;
    
    
    public override void Init(TalkSystem talkSys)
    {
        _talkSys = talkSys;
    }

    public override void Execute(FunctionCode.Function function)
    {
        switch (function)
        {
            case FunctionCode.Function.A:
                //TODO:教学菜单
                break;
            case FunctionCode.Function.B:
                //TODO:点击进行对话
                break;
            case FunctionCode.Function.C:
                //TODO:点击人物头像
                break;
            case FunctionCode.Function.D:
                //TODO:结束教学
                break;
            case FunctionCode.Function.E:
                //TODO:右侧栏
                break;
            case FunctionCode.Function.F:
                //TODO:切换下阶段
                break;
            case FunctionCode.Function.G:
                //TODO:分配食物
                break;
            case FunctionCode.Function.H:
                //TODO:负重分配
                break;
            case FunctionCode.Function.I:
                //TODO:安抚操作
                break;
        }
    }
}
