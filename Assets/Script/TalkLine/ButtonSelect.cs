using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonSelect : MonoBehaviour
{
    public TalkSystem TalkSystem;
    public Manager textbox { get; set; }
    private TextMeshProUGUI _buttonText;
    private void Awake()
    {
        _buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }



    public void Clik()
    {
        TalkSystem.on = true;
        TalkSystem.line = 1;
        TalkSystem.SetTextBox(textbox);
        TalkSystem.ShowText();
        for (int i = 0; i < gameObject.transform.parent.childCount; i++)
        {
            gameObject.transform.parent.GetChild(i).gameObject.SetActive(false);
        }
        
    }

    public void SetTextBox(Manager textBox)
    {
        textbox = textBox;
        _buttonText.text = textbox.TxtLine[0];
    }



}
