using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Select : MonoBehaviour
{
    public GameObject obj;
    public GameObject progress,TalkBar;
    public Manager Textline;
    public bool _isSpecial;
    

    public void Switch(bool i)
    {
        
        obj.GetComponent<Toggle>().isOn = i;
        

    }

    public void Clik()
    {
        if (_isSpecial)
        {
            int day_num = progress.GetComponent<Progress>().day_num;
            if (day_num == 1)
            {
                TalkBar.GetComponent<TalkSystem>().Talklines[day_num] = Textline;
                TalkBar.GetComponent<TalkSystem>().line = 0;
                _ = TalkBar.GetComponent<TalkSystem>().ShowText();

                TalkBar.GetComponent<Animator>().SetTrigger("up");
                _isSpecial = false;

            }
        }
    }




}
