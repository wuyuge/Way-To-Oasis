using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SetScreenMode : MonoBehaviour,SettingInitialize
{
    private SettingDataManager SaveManager;

    public void Initialize(SettingDataManager manager)
    {
        
        SaveManager = manager;
        switch (manager.setting.ScreenMode)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                GetComponent<TMP_Dropdown>().value = 0;
               
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                GetComponent<TMP_Dropdown>().value = 1;
                
                break;
        }
        return;


    }


    public void SetMode(int Value)
    {
        if(SaveManager == null) SaveManager = GameObject.Find("SaveManager").GetComponent<SettingDataManager>();
        switch (Value)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                SaveManager.setting.ScreenMode = 0;
                
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                SaveManager.setting.ScreenMode = 1;
                
                break;
        }
        
    }
}
