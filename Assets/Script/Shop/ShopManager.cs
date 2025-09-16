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
    public ObjectManager ObjManager;
    [Header("兑换尸体管理")]
    public Manager Body,Food,DeadName,UesdBody;

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

        int CanUseBody = 0;
        foreach(string s in DeadName.TxtLine)
        {
            if (!s.Contains("Uesd"))
            {
                CanUseBody += 1;
            }
        }


        if(CanUseBody == 1)
        {
            Body.Weight -= 1;
            Food.Weight += 7 - DeadName.TxtLine.Count;
            UesdBody.TxtLine.Add( DeadName.TxtLine[0] += "Uesd");
            DeadName.TxtLine.RemoveAt(0);
            ObjManager.Food_Text.text = Food.Weight.ToString();
            ObjManager.Body_Text.text = Body.Weight.ToString();
            return true;

        }
        else if(CanUseBody > 1)
        {
            return false;
        }
        else
        {
            return false ;
        }



    }
    





}
