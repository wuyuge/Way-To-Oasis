using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipText : MonoBehaviour
{
    public Button button;
    public bool lockButton;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        GlobalData.SkipButton = this;
    }


    private void LateUpdate()
    {
        button.interactable = !lockButton;
        
    }

    public void Skip()
    {
        if (GlobalData.TalkSystem.useNewSys)
        {
            GlobalData.NewTalkSysShowText.Skip();
        }
    }
    
}
