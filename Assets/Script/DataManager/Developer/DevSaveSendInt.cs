using System;
using TMPro;
using UnityEngine;
/// <summary>
/// 挂载在开发者界面设置day,存档编号,食物数量,阶段设置的UI上
/// </summary>
public class DevSaveSendInt : MonoBehaviour
{
    public string sendTag;
    private DeveloperSaveChanger _saveChanger;
    private void OnEnable()
    {
        SentInt(0);
        GetComponent<TMP_Dropdown>().value = 0;
    }

    public void SentInt(int value)
    {
        if (sendTag == null)
        {
            Debug.LogError("Tag is null");
            return;
        }

        if (sendTag == "saveIndex") value++;

        _saveChanger = _saveChanger == null ? gameObject.transform.parent.GetComponent<DeveloperSaveChanger>(): _saveChanger;
        
        _saveChanger.RcvData($"{value}",sendTag);
        
        
    }
    
    
}
