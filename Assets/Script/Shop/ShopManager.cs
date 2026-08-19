using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("文本对象")]
    
    public List<Manager> TextLine;
    public GameObject DayTime;
    public GameObject TextBar;
    public GameObject Talksys;
    private TalkSystem _talkSysScript;
    public ObjectManager ObjManager;
    [Header("兑换尸体管理")]
    public Manager Body,Food,DeadName,UesdBody;
    public Manager Kill, Exchange;

    private void Awake()
    {
        _talkSysScript =  Talksys.GetComponent<TalkSystem>();
        
    }


    private void OnEnable()
    {
        
        Kill.GeneralBool = false;
        Exchange.GeneralBool = false;

        int day_num = DayTime.GetComponent<Progress>().day_num;

        if (day_num == 2) _talkSysScript.Talklines[day_num] = TextLine[0];
        else if (day_num == 5) _talkSysScript.Talklines[day_num] = TextLine[1];
        else if (day_num == 7) _talkSysScript.Talklines[day_num] = TextLine[2];
        _talkSysScript._inshop = true;
        _talkSysScript.line = 0;
        _ = _talkSysScript.ShowText();
    }

    public bool ExchangeFood()
    {
        Debug.Log("交换尸体");
        int canUseBody = 0;
        foreach(string s in DeadName.TxtLine)
        {
            if (!s.Contains("Uesd"))
            {
                canUseBody += 1;
            }
        }



        if(canUseBody == 1)
        {
            Body.Weight -= 1;
            Food.Weight += 7 - DeadName.TxtLine.Count;
            UesdBody.TxtLine.Add( DeadName.TxtLine[0] += "Uesd");
            DeadName.TxtLine.RemoveAt(0);
            ObjManager.Food_Text.text = Food.Weight.ToString();
            ObjManager.Body_Text.text = Body.Weight.ToString();
            return true;

        }
        else if(canUseBody > 1)
        {
            return false;
        }
        else
        {
            return false ;
        }



    }
    





}
