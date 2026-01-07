using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchSet : SwitchCommand
{
    private TalkSystem _talkSys;
    private MiniCharacterManager _miniCharacterManager;
    private Progress _progress;
    
    public override void Init(TalkSystem talkSys)
    {
        _talkSys = talkSys;
        _miniCharacterManager = _talkSys.MiniCharacterManager.gameObject.GetComponent<MiniCharacterManager>();
        _progress = _talkSys.DaytimeOBJ.GetComponent<Progress>();
    }

    public override void Execute(FunctionCode.Function function)
    {
        switch (function)
        {
            case FunctionCode.Function.A:
                //设定迷你角色动画为站立
                _miniCharacterManager.SetStand();
                break;
            case FunctionCode.Function.B:
                //设定迷你角色动画为坐
                _miniCharacterManager.SetSit();
                break;
            case FunctionCode.Function.C:
                //TODO:在角色对话中添加已经死亡角色的名称(可能要重载函数返回值)
                break;
            case FunctionCode.Function.D:
                //禁止/开启切换阶段
                break;
            case FunctionCode.Function.Da://开启
                _progress.CanSwitch = true;
                break;
            case FunctionCode.Function.Db://关闭
                _progress.CanSwitch = false;
                break;
            case FunctionCode.Function.E:
                //控制角色安抚状态 (已转换为其他特殊标记)
                break;
            case FunctionCode.Function.F:
                //开/关 显示角色名称
                break;
            case FunctionCode.Function.Fa://开启
                _talkSys.SetShowName();
                break;
            case FunctionCode.Function.Fb://关闭
                _talkSys.SetNoName();
                break;
            case FunctionCode.Function.G:
                //重置角色对话立绘状态
                _talkSys.CharacterImageManager.ResetTrigger();
                break;
            case FunctionCode.Function.H:
                //所有对象一起黑掉
                _talkSys.CharacterImageManager.CloseImage();
                _progress.skip.transform.Find("Report").gameObject.SetActive(false);
                _progress.skip.GetComponent<Animator>().SetTrigger("dark");
                break;
            case FunctionCode.Function.I:
                //单独用于阿曼德二次对话
                break;
            case FunctionCode.Function.Ia://开
                _talkSys.amande.GetComponent<Character>().have_talk = true;
                Execute(FunctionCode.Function.Db);
                break;
            case FunctionCode.Function.Ib://关
                _talkSys.amande.GetComponent<Character>().have_talk = false;
                Execute(FunctionCode.Function.Da);
                break;
            case FunctionCode.Function.J:
                //消耗博金森尸体并且设定艾米莉不可负重
                int index = -1;
                foreach(string s in _talkSys.DeadName.TxtLine)
                {
                    index++;
                    if(s == "博金森")
                    {
                        _talkSys.UsedBody.TxtLine.Add("博金森Used");
                        _talkSys.DeadName.TxtLine.RemoveAt(index);
                    }

                }
                _talkSys.CharacterList[1].GetComponent<Character>().CantWeight = true;
                break;
            case FunctionCode.Function.K:
                //Day0开启切换阶段
                _talkSys.DaytimeOBJ.GetComponent<Progress>().can_skip = true;
                break;
        }
    }
}
