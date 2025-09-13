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

    [Header("兑换尸体管理")]
    public Manager Body,Food,DeadName;

    private void OnEnable()
    {
        int day_num = DayTime.GetComponent<Progress>().day_num;

        if (day_num == 2) Talksys.GetComponent<TalkSystem>().Talklines[day_num] = TextLine[0];
        else if (day_num == 5) Talksys.GetComponent<TalkSystem>().Talklines[day_num] = TextLine[1];
        else if (day_num == 7) Talksys.GetComponent<TalkSystem>().Talklines[day_num] = TextLine[2];
        Talksys.GetComponent<TalkSystem>()._inshop = true;
        Talksys.GetComponent<TalkSystem>().line = 0;
        _ = Talksys.GetComponent<TalkSystem>().ShowText();
    }

    public bool ExchangeFood()
    {

        if(Body.Weight == 1)
        {
            Body.Weight -= 1;
            Food.Weight += 6 - DeadName.TxtLine.Count;
            return true;

        }
        else if(Body.Weight > 1)
        {
            return false;
        }
        else
        {
            return false ;
        }



    }
    





}
