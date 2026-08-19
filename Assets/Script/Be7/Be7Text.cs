using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Be7Text : MonoBehaviour
{
    public Manager DeadList, UseList, language;
    public string Laiwen, Aimi, Bo, Luo;
    public string LaiwenEn, AimiEn, BoEn, LuoEn;
    private bool LaiwenDead,AimiDead,BoDead,LuoDead;
    public string cn, en;

    public void Start()
    {
        if (language.isEn)
        {
            GetComponent<TextMeshProUGUI>().text = en;
        }
        else
        {
            GetComponent<TextMeshProUGUI>().text = cn;
        }
        
        
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
            if (language.isEn)
            {
                Text.text = Text.text.Replace("{Laiwen}", LaiwenEn);
            }
            else
            {
                Text.text = Text.text.Replace("{Laiwen}", Laiwen);
            }
            
        }
        else Text.text = Text.text.Replace("{Laiwen}", "");

        if (!AimiDead)
        {
            if (language.isEn)
            {
                Text.text = Text.text.Replace("{Aimi}", AimiEn);
            }
            else
            {
                Text.text = Text.text.Replace("{Aimi}", Aimi);
            }
        }
        else Text.text = Text.text.Replace("{Aimi}", "");

        if (!BoDead)
        {
            if (language.isEn)
            {
                Text.text = Text.text.Replace("{Bo}", BoEn);
            }
            else
            {
                Text.text = Text.text.Replace("{Bo}", Bo);
            }
        }
        else Text.text = Text.text.Replace("{Bo}", "");

        if (!LuoDead)
        {
            if (language.isEn)
            {
                Text.text = Text.text.Replace("{Luo}", LuoEn);
            }
            else
            {
                Text.text = Text.text.Replace("{Luo}", Luo);
            }
        }
        else Text.text = Text.text.Replace("{Luo}", "");


    }



}
