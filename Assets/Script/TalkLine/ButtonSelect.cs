using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ButtonSelect : MonoBehaviour
{
    public TalkSystem TalkSystem;
    public Manager textbox { get; set; }
    public TextMeshProUGUI buttonText;
    private void Awake()
    {
        if (buttonText == null)
        {
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
        }
        
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
        try
        {
            textbox = textBox;
            buttonText.text = textbox.TxtLine[0];
        }
        catch (Exception e)
        {
            Debug.LogError($"按钮错误 错误类型{e}");
        }
        
    }



}
