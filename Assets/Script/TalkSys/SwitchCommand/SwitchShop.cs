using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchShop : SwitchCommand
{
    private TalkSystem _talkSys;
    private TalkSysShowText _showText;

    
    
    public override void Init(TalkSystem talkSys)
    {
        _talkSys = talkSys;
        _showText = talkSys.showText;
    }

    public override void Execute(FunctionCode.Function function)
    {
        switch (function)
        {
            case FunctionCode.Function.A:
                //开启商店场景
                _talkSys._inshop = true;
                _showText.CanShowText = true;
                GlobalData.InShop = true;
                break;
            case FunctionCode.Function.B:
                //关闭商店场景
                _talkSys._inshop = false;
                _showText.CanShowText = false;
                GlobalData.InShop = false;
                if (_talkSys.useNewSys)
                {
                    GlobalData.NewTalkSysShowText.LockOutPut();
                    GlobalData.NewTalkSysShowText.SetShopStatus(false);
                }
                _talkSys.ShopManager.SetActive(false);
                break;
            case FunctionCode.Function.C:
                //杀人接口
                Debug.Log("KillSomeOne");
                _talkSys.Day2_Shop_KillSomeOne.GeneralBool = true;
                _talkSys.ShopCharaBar.SetActive(true);
                _talkSys.ShopCharaBar.GetComponent<Animator>().SetTrigger("Up");
                _talkSys.ShopCharaBar.GetComponent<ShopCharacterManager>().KillSB();
                _talkSys.on = false;
                break;
            case FunctionCode.Function.D:
                //换尸体接口
                _talkSys.Day2_Shop_Exchange.GeneralBool = true;
                _talkSys._inshop = true;

                if (!_talkSys.ShopManager.GetComponent<ShopManager>().ExchangeFood())
                {
                    _talkSys.ShowExchangeTalk();
                    _talkSys.ShopCharaBar.SetActive(true);
                    _talkSys.ShopCharaBar.GetComponent<Animator>().SetTrigger("Up");
                    _talkSys.ShopCharaBar.GetComponent<ShopCharacterManager>().SelectBody();
                    _talkSys.on = false;
                    return;


                }
                _talkSys.ShowExchangeTalk();
                break;
        }
    }
    
    
    
    
}
