using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSave : MonoBehaviour
{

    public SettingDataManager SettingManager;

    private void Awake()
    {
        if (SettingManager is null)
        {
            SettingManager = GameObject.Find("SaveManager").GetComponent<SettingDataManager>();
        }
        
    }

    public void Save()
    {
        SettingManager.Save();
    }


}
