using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SwitchCheck : SwitchCommand
{
    private TalkSystem _talkSys;
    private Manager ShopEventBox => _talkSys.shopEvent;
    private bool ShopEvent => ShopEventBox.GeneralBool;
    [SerializeField]
    private List<Character> _characters = new List<Character>();
    public Manager aimi;
    public List<FunctionCode.Function> doNotResetLineList;

    public override void Init(TalkSystem talkSys)
    {
        _talkSys = talkSys;
        _characters.Clear(); // 清空旧数据，必加

        if (talkSys.CharacterList == null) return;

        foreach (var g in talkSys.CharacterList)
        {
            // 跳过空对象，这是核心修复！
            if (g == null) continue;

            // 只有物体上真的有 Character 脚本才添加
            Character charComp = g.GetComponent<Character>();
            if (charComp != null)
            {
                _characters.Add(charComp);
            }

            // 够6个立刻停止
            if (_characters.Count == 6)
            {
                break;
            }
        }
    }

    public override void Execute(FunctionCode.Function code)
{
    if (!doNotResetLineList.Contains(code)) _talkSys.ResetLine();
    
    switch (code)
    {
        case FunctionCode.Function.A:
            if (ShopEvent)
            {
                _talkSys.SwitchLine(TalkLine.Line1);
                
                return;
            }
            _talkSys.SwitchLine(TalkLine.Line2);
            
            return;
            
        case FunctionCode.Function.B:
            Debug.Log("检测是否有人死亡");
            if (_talkSys.useNewSys)
            {
                GlobalData.NewTalkSysShowText.SetChoiceLine(0,false);
            }
            foreach (var c in _characters)
            {
                if (c.Dead)
                {
                    _talkSys.SwitchLine(TalkLine.Line2);
                    
                    return;
                }
            }
            _talkSys.SwitchLine(TalkLine.Line1);
            
            return;
            
        case FunctionCode.Function.C:
            Debug.Log("检查艾米莉在前一天是否获得食物");
            if (aimi.Eat)
            {
                _talkSys.SwitchLine(TalkLine.Line1);
                
                return;
            }
            _talkSys.SwitchLine(TalkLine.Line2);
            
            return;
            
        case FunctionCode.Function.D:
            Debug.Log("检查博金森是否死亡");
            foreach (var c in _characters)
            {
                if (c.CharacterName == "博金森")
                {
                    if (c.Dead)
                    {
                        _talkSys.SwitchLine(TalkLine.Line2);
                        
                        return;
                    }
                    Debug.Log("博金森未死亡");
                    break;
                }
            }
            _talkSys.SwitchLine(TalkLine.Line1);
            
            return;
            
        case FunctionCode.Function.E:
            if (_talkSys.Daytime - _talkSys.BoDeadTime.Weight > 1)
            {
                _talkSys.SwitchLine(TalkLine.Line2);
                
                return;
            }
            _talkSys.SwitchLine(TalkLine.Line1);
            
            return;
            
        case FunctionCode.Function.F:
            var deadNum = 0;
            string charaName = null;
            foreach (var c in _characters)
            {
                if (c.Dead)
                {
                    deadNum++;
                    charaName = c.CharacterName;
                }
                if (deadNum > 1)
                {
                    _talkSys.SwitchLine(TalkLine.Line3);
                    return;
                }
            }
            if (charaName is "莱文" or "博金森")
            {
                _talkSys.SwitchLine(TalkLine.Line1);
                return;
            }
            else if (charaName is "艾米莉" or "阿曼德")
            {
                _talkSys.SwitchLine(TalkLine.Line2);
                return;
            }
            return;  // 保底return
            
        case FunctionCode.Function.G:
            int curTalk = 0;
            foreach (var value in _characters)
            {
                if (value.have_talk) curTalk++;
            }
            Debug.Log(curTalk.ToString());
            if (curTalk < 5)
            {
                _talkSys.SwitchLine(TalkLine.Line3);
                if (_talkSys.useNewSys)
                {
                    GlobalData.NewTalkSysShowText.SetChoiceLine(0,false);
                }
                _talkSys.line = 0;
                return;
            }
            _talkSys.line = 5;
            return;
            
        case FunctionCode.Function.H:
            Manager tempDeadBox = _talkSys.DeadName;
            if (tempDeadBox.TxtLine != null)
            {
                foreach (string s in tempDeadBox.TxtLine)
                {
                    if (s.Contains("博金森"))
                    {
                        _talkSys.buttonFunc.SetBoBodySpecialChoice(true);
                        return;
                    }
                }
                _talkSys.buttonFunc.SetBoBodySpecialChoice(false);
            }
            _talkSys.showText.CanShowText = false;
            return;
            
        case FunctionCode.Function.I:
            if (_talkSys.amandeKillself.GeneralBool)
            {
                _talkSys.SwitchLine(TalkLine.Line1);
                
                return;
            }
            _talkSys.SwitchLine(TalkLine.Line2);
            
            return;
            
        case FunctionCode.Function.J:
            foreach (var c in _characters)
            {
                if (c.CharacterName == "艾米莉")
                {
                    if (c.Dead)
                    {
                        _talkSys.SwitchLine(TalkLine.Line3);
                        return;
                    }
                    break;
                }
            }
            _talkSys.line++;
            return;  // 注意：这里没有SwitchLine，只是检查后返回
            
        case FunctionCode.Function.K:
            // 迷你游戏切换标志
            return;
            
        case FunctionCode.Function.Ka:
            foreach (var value in GlobalData.MiniGameManager.miniGameData)
            {
                if (value.infos[GlobalData.Day].canPlay && value.name == "阿曼德")
                {
                    _talkSys.ResetLine();
                    if (_talkSys.useNewSys) GlobalData.NewTalkSysShowText.SetChoiceLine(0,false);
                    _talkSys.SwitchLine(TalkLine.Line3);
                    return;
                }
            }
            return;
            
        case FunctionCode.Function.Kb:
            foreach (var value in GlobalData.MiniGameManager.miniGameData)
            {
                if (value.infos[GlobalData.Day].canPlay && value.name == "艾米莉")
                {
                    _talkSys.ResetLine();
                    if (_talkSys.useNewSys) GlobalData.NewTalkSysShowText.SetChoiceLine(0,false);
                    _talkSys.SwitchLine(TalkLine.Line3);
                    return;
                }
            }
            return;
            
        case FunctionCode.Function.Kc:
            foreach (var value in GlobalData.MiniGameManager.miniGameData)
            {
                if (value.infos[GlobalData.Day].canPlay && value.name == "博金森")
                {
                    _talkSys.ResetLine();
                    if (_talkSys.useNewSys) GlobalData.NewTalkSysShowText.SetChoiceLine(0,false);
                    _talkSys.SwitchLine(TalkLine.Line3);
                    return;
                }
            }
            return;
            
        case FunctionCode.Function.Kd:
            foreach (var value in GlobalData.MiniGameManager.miniGameData)
            {
                if (value.infos[GlobalData.Day].canPlay && value.name == "洛尔坎")
                {
                    _talkSys.ResetLine();
                    if (_talkSys.useNewSys) GlobalData.NewTalkSysShowText.SetChoiceLine(0,false);
                    _talkSys.SwitchLine(TalkLine.Line3);
                    return;
                }
            }
            return;
            
        case FunctionCode.Function.Ke:
            foreach (var value in GlobalData.MiniGameManager.miniGameData)
            {
                if (value.infos[GlobalData.Day].canPlay && value.name == "莱文")
                {
                    _talkSys.ResetLine();
                    if (_talkSys.useNewSys) GlobalData.NewTalkSysShowText.SetChoiceLine(0,false);
                    _talkSys.SwitchLine(TalkLine.Line3);
                    return;
                }
            }
            return;
            
        case FunctionCode.Function.L:
            foreach (var value in _characters)
            {
                if (value.Dead && value.CharacterName != "阿曼德")
                {
                    if (_talkSys.useNewSys) GlobalData.NewTalkSysShowText.SetChoiceLine(0,false);
                    _talkSys.SwitchLine(TalkLine.Line2);
                    _talkSys.line = 0;
                    return;
                }
            }
            if (_talkSys.useNewSys) GlobalData.NewTalkSysShowText.SetChoiceLine(0,false);
            _talkSys.SwitchLine(TalkLine.Line1);
            _talkSys.line = 0;
            return;
            
        case FunctionCode.Function.M:
            var tempLive = 0;
            foreach (var value in _characters)
            {
                if (value.CharacterName == "主角") continue;
                if (!value.Dead)
                {
                    tempLive++;
                    if (tempLive > 1) break;
                }
            }
            if (tempLive == 1)
            {
                _talkSys.SwitchLine(TalkLine.Line3);
                _talkSys.line = 0;
                if (_talkSys.useNewSys) GlobalData.NewTalkSysShowText.SetChoiceLine(0,false);
                return;
            }
            
            if (!_talkSys.useNewSys)_talkSys.line++;
            return;
            
        case FunctionCode.Function.N:
            tempLive = 0;
            foreach (var value in _characters)
            {
                if (value.CharacterName == "主角") continue;
                if (!value.Dead)
                {
                    tempLive++;
                    if (tempLive > 2) break;
                }
            }
            if (tempLive <= 2)
            {
                _talkSys.SwitchLine(TalkLine.Line3);
                if (_talkSys.useNewSys) GlobalData.NewTalkSysShowText.SetChoiceLine(0,false);
                _talkSys.line = 0;
                return;
            }
            if (!_talkSys.useNewSys)_talkSys.line++;
            return;
    }
}









}
