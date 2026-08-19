using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwitchLanguage : MonoBehaviour
{
    public Manager language;

    private void OnEnable()
    {
        GetComponent<Toggle>().isOn = language.isEn;
    }

    public void Switch(bool v)
    {
        language.isEn = v;
    }
    
    
}
