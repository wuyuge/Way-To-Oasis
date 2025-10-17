using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Report : MonoBehaviour
{
    public Manager Food,Body,CurrentDead;
    public Progress Day;
    private TextMeshProUGUI Text;
    public string DeadText;
    public string DefultText;

    private void Start()
    {
        Text = GetComponent<TextMeshProUGUI>();
    }


    public void ShowText()
    {
        Text.text = DefultText;
        string showdead = DeadText + " ";
        Text.text = Text.text.Replace("{food}",Food.Weight.ToString() + "\n");
        Text.text = Text.text.Replace("{body}",Body.Weight.ToString());
        Text.text = Text.text.Replace("{day}",Day.day_num.ToString() + "\n");
        if(CurrentDead.TxtLine.Count != 0)
        {

            foreach (string s in CurrentDead.TxtLine)
            {
                showdead += s + " ";

            }
            showdead += "\n";

            Text.text = Text.text.Replace("{dead}", showdead);
            Debug.Log("有人死亡报告" + $"{showdead}死亡");
        }
        else
        {
            Debug.Log("无人死亡报告");
            Text.text = Text.text.Replace("{dead}", "");
        }


    }



}
