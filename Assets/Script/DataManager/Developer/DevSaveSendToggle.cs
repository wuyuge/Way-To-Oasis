using System;
using UnityEngine;
using UnityEngine.UI;

public class DevSaveSendToggle : MonoBehaviour
{
    
    private DeveloperSaveChanger _developerSaveChanger;
/// <summary>
/// 初始化开发者存档对象
/// </summary>
    private void Awake()
    {
        _developerSaveChanger = gameObject.transform.parent.GetComponent<DeveloperSaveChanger>();
    }

    private void OnEnable()
    {
        GetComponent<Toggle>().isOn = false;
    }


    /// <summary>
    /// 发送数据以更新玩家存档中的特定字段。
    /// </summary>
    /// <param name="value">布尔值，用于设置"Amande"自杀标记。如果为true，则设置为"1"；如果为false，则设置为"0"。</param>
    public void SendData(bool value)
    {
        string sendValue = value ? "1": "0";
        
        _developerSaveChanger.RcvData(sendValue,"Amande");
        
        
        
    }
    
    
    
}
