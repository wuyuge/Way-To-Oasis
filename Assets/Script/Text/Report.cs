using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Report : MonoBehaviour
{
    public Manager Food,Body,Food2,Body2,CurrentDead;
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
        if(Text == null)
        {
            Text = GetComponent<TextMeshProUGUI>();
            DefultText = Text.text;
        }
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
            
        }
        else
        {
            
            Text.text = Text.text.Replace("{dead}", string.Empty);
            
        }

        string FoodText = Mathf.Max(Food.Weight, Food2.Weight).ToString();
        string BodyText = Mathf.Max(Body.Weight, Body2.Weight).ToString();

        Text.text = Text.text.Replace("{food}", "<color=#00ff00ff>" + FoodText + "</color>");
        if(Mathf.Max(Food.Weight, Food2.Weight) > 9)
        {
            Text.text = Text.text.Replace("{body}", " " + BodyText);
        }
        else Text.text = Text.text.Replace("{body}", BodyText);





    }



}
