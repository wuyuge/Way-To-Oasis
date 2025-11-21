using System;
using TMPro;
using UnityEngine;

public class DevSaveSentString : MonoBehaviour
{
    public string sendTag;
    private DeveloperSaveChanger _saveChanger;

    private void OnEnable()
    {
        GetComponent<TMP_InputField>().text = string.Empty;
    }


    public void SendString(string value)
    {

        if (value == string.Empty)
        {
            return;
        }
        
        _saveChanger = _saveChanger == null ? gameObject.transform.parent.GetComponent<DeveloperSaveChanger>() : _saveChanger;
        
        
        _saveChanger.RcvData(value,sendTag);
        
    }
    
    
    
}
