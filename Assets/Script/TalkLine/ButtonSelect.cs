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
    [SerializeField]
    private GameObject playerTalkBack;
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
        if (TalkSystem.useNewSys)
        {
            GlobalData.NewTalkSysShowText.SetChoiceLine(1,false);
            GlobalData.NewTalkSysShowText.UnLockOutPut();
        }
        try
        {
            if (playerTalkBack != null)
            {
                playerTalkBack.SetActive(true);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e,this);
        }
        finally
        {
            TalkSystem.SetTextBox(textbox);
            TalkSystem.ShowText();
            for (int i = 0; i < gameObject.transform.parent.childCount; i++)
            {
                gameObject.transform.parent.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void SetTextBox(Manager textBox)
    {
        try
        {
            textbox = textBox;
            if (GlobalData.TalkSystem.useNewSys)
            {
                if (GlobalData.Language.isEn)
                {
                    buttonText.text = textbox.data[0].en;
                }
                else
                {
                    buttonText.text = textbox.data[0].cn;
                }
            }
            else
            {
                buttonText.text = textbox.TxtLine[0];
            }
            
            if (playerTalkBack != null)
            {
                playerTalkBack.SetActive(false);
            }
            
            
        }
        catch (Exception e)
        {
            Debug.LogError($"按钮错误 错误类型{e}");
        }
        
    }



}
