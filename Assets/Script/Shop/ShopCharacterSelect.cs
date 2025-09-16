using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ShopCharacterSelect : MonoBehaviour
{
    public GameObject ShopManager,ShopTextBar;
    public string Name;
    public Manager DeadName,UsedBody;
    public GameObject TalkSys;
    public Character Character;
    public Manager SpecialTalk,FinalFood,FinalBody;

    private void Start()
    {
        
        ShopTextBar = gameObject.transform.parent.parent.Find("TextBar").gameObject;
    }



    public void Clik()
    {
        TalkSystem ts = TalkSys.GetComponent<TalkSystem>();
        //如果是要杀人
        if (ShopManager.GetComponent<ShopCharacterManager>().kill && Name != "Leader")
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
            FinalBody.Weight += 1;
            TalkSys.GetComponent<TalkSystem>();
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
            FinalFood.Weight += 7 - DeadName.TxtLine.Count;
            UsedBody.TxtLine.Add(DeadName.TxtLine[index] += "Uesd");
            DeadName.TxtLine.RemoveAt(index);

            
            TalkSys.GetComponent<TalkSystem>().DownBar.GetComponent<ObjectManager>().Food_Text.text = FinalFood.Weight.ToString();
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
