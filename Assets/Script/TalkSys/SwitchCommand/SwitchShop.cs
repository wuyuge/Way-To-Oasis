using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchShop : SwitchCommand
{
    private TalkSystem _talkSys;

    private bool InShop
    {
        get => _talkSys._inshop;
        set => _talkSys._inshop = value;
    }
    
    public override void Init(TalkSystem talkSys)
    {
        _talkSys = talkSys;
    }

    public override void Execute(FunctionCode.Function function)
    {
        switch (function)
        {
            case FunctionCode.Function.A:
                //开启商店场景
                InShop = true;
                break;
            case FunctionCode.Function.B:
                //关闭商店场景
                InShop = false;
                _talkSys.ShopManager.SetActive(false);
                break;
            case FunctionCode.Function.C:
                //杀人接口
                _talkSys.Day2_Shop_KillSomeOne.GeneralBool = true;
                _talkSys.ShopCharaBar.SetActive(true);
                _talkSys.ShopCharaBar.GetComponent<Animator>().SetTrigger("Up");
                _talkSys.ShopCharaBar.GetComponent<ShopCharacterManager>().KillSB();
                _talkSys.on = false;
                break;
            case FunctionCode.Function.D:
                //换尸体接口
                _talkSys.Day2_Shop_Exchange.GeneralBool = true;
                InShop = true;

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
