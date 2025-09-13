using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSelect : MonoBehaviour
{
    public TalkSystem TalkSystem;
    public Manager textbox;


    public void Clik()
    {
        TalkSystem.on = true;
        TalkSystem.line = 0;
        TalkSystem.Setchoice(textbox);
        
    }





}
