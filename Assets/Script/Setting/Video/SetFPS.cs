using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetFPS : MonoBehaviour,SettingInitialize
{
    public TMP_Dropdown UI;
    private SettingDataManager Manager;
    
    public void Initialize(SettingDataManager manager)
    {
        Manager = manager;

        SetFps(manager.setting.RefreshFPS);
        GetComponent<TMP_Dropdown>().value = manager.setting.RefreshFPS;



    }


    

    public void SetFps(int value)
    {
        Manager.setting.RefreshFPS = value;

        switch (value)
        {
            case 0:
                Application.targetFrameRate = 30;
                
                
                break;
            case 1:
                Application.targetFrameRate = 60;
                
                break;
            case 2:
                Application.targetFrameRate = 120;
                
                break;
            case 3:
                Application.targetFrameRate = -1;
                
                break;



        }




    }


    private void Update()
    {
        if (QualitySettings.vSyncCount != 0)
        {
            Baned();
        }
        else if (QualitySettings.vSyncCount == 0)
        {
            Opened();
        }
        
    }


    void Baned()
    {
        gameObject.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
        UI.enabled = false;
        UI.captionText.text = "ÒÑ½ûÓÃ";
        
    }

    void Opened()
    {
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        UI.enabled = true;
        UI.captionText.text = UI.options[UI.value].text;
        
    }



}
