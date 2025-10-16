using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Be7Text : MonoBehaviour
{
    public Manager DeadList, UseList;
    public string Laiwen, Aimi, Bo, Luo;
    private bool LaiwenDead,AimiDead,BoDead,LuoDead;

    public void Start()
    {
        
        foreach (string s in DeadList.TxtLine)
        {

            if (s.Contains("À³ÎÄ"))
            {
                LaiwenDead = true;
            }

            if (s.Contains("°¬Ã×Àò"))
            {
                AimiDead = true;
            }

            if (s.Contains("²©½ðÉ­"))
            {
                BoDead = true;
            }

            if (s.Contains("Âå¶û¿²"))
            {
                LuoDead = true;
            }

        }

        foreach (string s in UseList.TxtLine)
        {
            if (s.Contains("À³ÎÄ"))
            {
                LaiwenDead = true;
            }

            if (s.Contains("°¬Ã×Àò"))
            {
                AimiDead = true;
            }

            if (s.Contains("²©½ðÉ­"))
            {
                BoDead = true;
            }

            if (s.Contains("Âå¶û¿²"))
            {
                LuoDead = true;
            }

        }


        TextMeshProUGUI Text = GetComponent<TextMeshProUGUI>();

        if (!LaiwenDead)
        {
            Text.text = Text.text.Replace("{Laiwen}", Laiwen);
        }
        else Text.text = Text.text.Replace("{Laiwen}", "");

        if (!AimiDead)
        {
            Text.text = Text.text.Replace("{Aimi}", Aimi);
        }
        else Text.text = Text.text.Replace("{Aimi}", "");

        if (!BoDead)
        {
            Text.text = Text.text.Replace("{Bo}", Bo);
        }
        else Text.text = Text.text.Replace("{Bo}", "");

        if (!LuoDead)
        {
            Text.text = Text.text.Replace("{Luo}", Luo);
        }
        else Text.text = Text.text.Replace("{Luo}", "");


    }



}
