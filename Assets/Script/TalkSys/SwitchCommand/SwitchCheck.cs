using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SwitchCheck : SwitchCommand
{
    private TalkSystem _talkSys;
    private Manager ShopEventBox => _talkSys.shopEvent;
    private bool ShopEvent => ShopEventBox.GeneralBool;
    private List<Character> _characters = new List<Character>();
    private Manager Aimi => _talkSys.aimi;
    public List<FunctionCode.Function> doNotResetLineList;

    public override void Init(TalkSystem talkSys)
    {
        _talkSys = talkSys;
        foreach (var g in talkSys.CharacterList)
        {
            try
            {
                _characters.Add(g.GetComponent<Character>());
            }
            catch (Exception e)
            {
                Debug.LogError($"错误{e}");
            }
            
        }

    }

    public override void Execute(FunctionCode.Function code)
    {
        if (!doNotResetLineList.Contains(code)) _talkSys.ResetLine();
        switch (code)
        {
            case FunctionCode.Function.A:
                //检查商店事件
                if (ShopEvent)
                {
                    _talkSys.SwitchLine(TalkLine.Line1);
                    break;
                }

                _talkSys.SwitchLine(TalkLine.Line2);
                break;
            case FunctionCode.Function.B: //原命令:/CheckEveryOneLive,dead,twicedeadchoice
                //是否有人死亡
                foreach (var c in _characters)
                {
                    if (c.Dead)
                    {
                        _talkSys.SwitchLine(TalkLine.Line2); //有人死亡转2号对话分支,否则1号
                        return;
                    }
                }

                _talkSys.SwitchLine(TalkLine.Line1);
                break;
            case FunctionCode.Function.C:
                //检查艾米莉在前一天是否获得食物
                if (Aimi.Day1Eat)
                {
                    _talkSys.SwitchLine(TalkLine.Line1);
                    return;
                }

                _talkSys.SwitchLine(TalkLine.Line2);
                break;
            case FunctionCode.Function.D:
                //检查博金森是否死亡
                foreach (var c in _characters)
                {
                    if (c.CharacterName == "博金森")
                    {
                        if (c.Dead)
                        {
                            _talkSys.SwitchLine(TalkLine.Line2);
                            return;
                        }

                        break;
                    }

                }

                _talkSys.SwitchLine(TalkLine.Line1);
                break;
            case FunctionCode.Function.E:
                //检查博金森死亡时间
                if (_talkSys.Daytime - _talkSys.BoDeadTime.Weight > 1)
                {
                    _talkSys.SwitchLine(TalkLine.Line2);
                    return;
                }

                _talkSys.SwitchLine(TalkLine.Line1);
                break;
            case FunctionCode.Function.F:
                //判断死亡者性别
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
                }
                else if (charaName is "艾米莉" or "阿曼德")
                {
                    _talkSys.SwitchLine(TalkLine.Line2);
                }

                break;
            case FunctionCode.Function.G:
                //Day0检查是否与所有人对话
                if (_talkSys.Day0_Talk.Weight != 3)
                {
                    _talkSys.SwitchLine(TalkLine.Line3);
                    _talkSys.line = 0;
                    break;
                }

                _talkSys.line = 5;
                break;
            case FunctionCode.Function.H:
                //检查是否持有博金森尸体
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

                break;
            case FunctionCode.Function.I:
                //检查艾米莉自杀事件
                if (_talkSys.amandeKillself.GeneralBool)
                {
                    _talkSys.SwitchLine(TalkLine.Line1);
                    return;
                }

                _talkSys.SwitchLine(TalkLine.Line2);
                break;
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
                break;
            case FunctionCode.Function.K:
                //迷你游戏切换标志
                break;
            case FunctionCode.Function.Ka:
                //阿曼德迷你游戏切换标志
                foreach (var value in GlobalData.MiniGameManager.miniGameData)
                {
                    if (value.day == GlobalData.Day && value.characterName == "阿曼德")
                    {
                        if (value.canPlay)
                        {
                            _talkSys.ResetLine();
                            _talkSys.SwitchLine(TalkLine.Line3);
                        }
                        return;
                    }
                }
                return;
            case FunctionCode.Function.Kb:
                //艾米莉迷你游戏切换标志
                foreach (var value in GlobalData.MiniGameManager.miniGameData)
                {
                    if (value.day == GlobalData.Day && value.characterName == "艾米莉")
                    {
                        if (value.canPlay)
                        {
                            _talkSys.ResetLine();
                            _talkSys.SwitchLine(TalkLine.Line3);
                        }
                        return;
                    }
                }
                return;
            case FunctionCode.Function.Kc:
                //博金森迷你游戏切换标志
                foreach (var value in GlobalData.MiniGameManager.miniGameData)
                {
                    if (value.day == GlobalData.Day && value.characterName == "博金森")
                    {
                        if (value.canPlay)
                        {
                            _talkSys.ResetLine();
                            _talkSys.SwitchLine(TalkLine.Line3);
                        }
                        return;
                    }
                }
                return;
            case FunctionCode.Function.Kd:
                //洛尔坎迷你游戏切换标志
                foreach (var value in GlobalData.MiniGameManager.miniGameData)
                {
                    if (value.day == GlobalData.Day && value.characterName == "洛尔坎")
                    {
                        if (value.canPlay)
                        {
                            _talkSys.ResetLine();
                            _talkSys.SwitchLine(TalkLine.Line3);
                        }
                        return;
                    }
                }
                return;
            case FunctionCode.Function.Ke:
                //莱文迷你游戏切换标志
                foreach (var value in GlobalData.MiniGameManager.miniGameData)
                {
                    if (value.day == GlobalData.Day && value.characterName == "莱文")
                    {
                        if (value.canPlay)
                        {
                            _talkSys.ResetLine();
                            _talkSys.SwitchLine(TalkLine.Line3);
                        }
                        return;
                    }
                }
                return;

        }
    }









}
