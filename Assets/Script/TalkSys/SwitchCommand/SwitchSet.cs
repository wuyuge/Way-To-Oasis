using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchSet : SwitchCommand
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
                //TODO:设定迷你角色动画为站立
                break;
            case FunctionCode.Function.B:
                //TODO:设定迷你角色动画为坐
                break;
            case FunctionCode.Function.C:
                //TODO:在角色对话中添加已经死亡角色的名称(可能要重载函数返回值)
                break;
            case FunctionCode.Function.D:
                //TODO:禁止/开启切换阶段
                break;
            case FunctionCode.Function.E:
                //TODO:控制角色安抚状态
                break;
            case FunctionCode.Function.F:
                //TODO:开/关 显示角色名称
                break;
            case FunctionCode.Function.G:
                //TODO:重置角色对话立绘状态
                break;
            case FunctionCode.Function.H:
                //TODO:所有对象一起黑掉
                break;
            case FunctionCode.Function.I:
                //TODO:单独用于阿曼德二次对话
                break;
            case FunctionCode.Function.J:
                //TODO:消耗博金森尸体并且设定艾米莉不可负重
                break;
        }
    }
}
