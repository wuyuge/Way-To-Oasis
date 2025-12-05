using UnityEngine;

public class SwitchCheck : SwitchCommand
{
    private TalkSystem _talkSys;
    [SerializeField]
    private Manager shopEventBox;

    private int ShopEvent => shopEventBox.Weight;

    public override void Init(TalkSystem talkSys)
    {
        _talkSys = talkSys;
    }

    public override void Execute(FunctionCode.Function code)
    {
        switch (code)
        {
            case FunctionCode.Function.A:
                //TODO:检查商店事件
                break;
            case FunctionCode.Function.B://原命令:/CheckEveryOneLive,dead,twicedeadchoice
                //TODO:是否有人死亡
                break;
            case FunctionCode.Function.C:
                //TODO:检查艾米莉在前一天是否获得食物
                break;
            case FunctionCode.Function.D:
                //TODO:检查博金森是否死亡
                break;
            case FunctionCode.Function.E:
                //TODO:检查艾米莉自杀事件
                break;
            case FunctionCode.Function.F:
                //TODO:判断死亡者性别
                break;
            case FunctionCode.Function.G:
                //TODO:Day0检查是否与所有人对话
                break;
            case FunctionCode.Function.H:
                //TODO:检查是否持有博金森尸体
                break;
        }
    }
    
    
    
    
    
    
    
}
