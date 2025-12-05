using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchShop : SwitchCommand
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
                //TODO:开启商店场景
                break;
            case FunctionCode.Function.B:
                //TODO:关闭商店场景
                break;
            case FunctionCode.Function.C:
                //TODO:杀人接口
                break;
            case FunctionCode.Function.D:
                //TODO:换尸体接口
                break;
        }
    }
    
    
    
    
}
