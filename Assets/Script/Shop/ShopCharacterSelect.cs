using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ShopCharacterSelect : MonoBehaviour
{
    public GameObject ShopManager,ShopTextBar;
    public string Name;
    public Manager DeadName;
    public GameObject TalkSys;
    public Character Character;
    public Manager SpecialTalk;

    private void Start()
    {
        
        ShopTextBar = gameObject.transform.parent.parent.Find("TextBar").gameObject;
    }



    public void Clik()
    {
        TalkSystem ts = TalkSys.GetComponent<TalkSystem>();
        //如果是要杀人
        if (ShopManager.GetComponent<ShopCharacterManager>().kill)
        {
            DeadName.TxtLine.Add(Name);
            TalkSys.GetComponent<TalkSystem>().ShowKillTalk();
            this.Character.Dead = true;
            if(ts.Daytime == 2)
            {
                ts.on = true;
                ts.Talklines[ts.Daytime] = SpecialTalk;
                ts._inshop = true;
                ts.line = 0;
                _ = ts.ShowText(true);
            }

            _ = SetAnimation();


        }
        else//拿尸体换食物
        {
            int index = -1;
            foreach(string s in DeadName.TxtLine)
            {
                index++;

                if (s == Name)
                {
                    break;
                }

            }
            DeadName.TxtLine[index] += "Uesd";


            _ = SetAnimation();

        }

    }

    async Task SetAnimation()
    {
        TalkSys.GetComponent<TalkSystem>().line++;
        
        ShopTextBar.GetComponent<Animator>().SetTrigger("Up");
        await Task.Delay(200);
        TalkSys.GetComponent<TalkSystem>().on = true;
        ShopManager.GetComponent<Animator>().SetTrigger("Down");
        await Task.Delay(200);
        ShopManager.SetActive(false);
    }








}
