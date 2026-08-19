using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VSyncSetting : MonoBehaviour,SettingInitialize
{
    private SettingDataManager Manager;

    public void Initialize(SettingDataManager manager)
    {
        Manager = manager;
        QualitySettings.vSyncCount = manager.setting.Vsync;
        GetComponent<TMP_Dropdown>().value = manager.setting.Vsync;


    }
    

    public void SetVSync(int vSync)
    {
        QualitySettings.vSyncCount = vSync;
        Manager.setting.Vsync = vSync;
        
    }




}
