using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class SettingManager : MonoBehaviour
{

    public List<GameObject> SettingObj;


    public void OpenSetting()
    {
        foreach (var item in SettingObj)
        {
            item.SetActive(true);
            item.transform.SetAsLastSibling();
        }
        
    }

    public void CloseSetting()
    {
        foreach (var item in SettingObj)
        {
            item.SetActive(false);
        }
    }


}
