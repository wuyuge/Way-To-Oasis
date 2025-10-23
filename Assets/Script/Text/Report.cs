using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Report : MonoBehaviour
{
    public Manager Food,Body,CurrentDead;
    public Progress Day;
    private TextMeshProUGUI Text;
    public string DefultText;
    public Manager AmandeKillSelf;
    private void Start()
    {
        Text = GetComponent<TextMeshProUGUI>();
        DefultText = Text.text;
    }


    public void ShowText()
    {
        Text.text = DefultText;
        string showdead = string.Empty;
        Text.text = Text.text.Replace("{day}", Day.day_num.ToString());
        
        if (CurrentDead.TxtLine.Count != 0)
        {
            showdead += "<color=#ff0000ff>";
            foreach (string s in CurrentDead.TxtLine)
            {
                
                if (s.Contains("博金森"))
                {
                    if(s != CurrentDead.TxtLine[CurrentDead.TxtLine.Count - 1])
                    showdead += "你杀死了" + s + "\n";
                    else
                        showdead += "你杀死了" + s;
                }
                else if (s.Contains("阿曼德"))
                {
                    if (AmandeKillSelf.GeneralBool)
                    {
                        if (s != CurrentDead.TxtLine[CurrentDead.TxtLine.Count - 1])
                            showdead +=  s + "死亡了";
                        else showdead += s + "死亡了";
                    }
                    else
                    {
                        if (s != CurrentDead.TxtLine[CurrentDead.TxtLine.Count - 1])
                            showdead += s + "被你饿死了\n";
                        else showdead += s + "被你饿死了";
                    }
                }
                else
                {
                    if (s != CurrentDead.TxtLine[CurrentDead.TxtLine.Count - 1])
                        showdead += s + "被你饿死了\n"; 
                    else showdead += s + "被你饿死了";


                }

            }

            showdead += "</color>";
            Text.text = Text.text.Replace("{dead}", showdead + "\n");
            Debug.Log("有人死亡报告" + $"{showdead}死亡");
        }
        else
        {
            Debug.Log("无人死亡报告");
            Text.text = Text.text.Replace("{dead}", string.Empty);
            
        }
        Text.text = Text.text.Replace("{food}", "<color=#00ff00ff>" + Food.Weight.ToString() + "</color>");
        if(Food.Weight > 9)
        {
            Text.text = Text.text.Replace("{body}", " " + Body.Weight.ToString());
        }
        else Text.text = Text.text.Replace("{body}", Body.Weight.ToString());





    }



}
