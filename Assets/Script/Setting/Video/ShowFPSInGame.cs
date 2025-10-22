using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowFPSInGame : MonoBehaviour , SettingInitialize
{

    public GameObject FPSText;
    private SettingDataManager SaveManager;

    public void Initialize(SettingDataManager manager)
    {
        SaveManager = manager;
        if (manager.setting.ShowFPS)
        {
            Clik(1);
            GetComponent<TMP_Dropdown>().value = 1;
        }
        else
        {
            Clik(0);
            GetComponent<TMP_Dropdown>().value = 0;
        }
    }



    public void Clik(int Value)
    {


        if(Value == 0)
        {
            FPSText.SetActive(false);
            SaveManager.setting.ShowFPS = false;
        }
        else if(Value == 1)
        {
            FPSText.SetActive(true);
            SaveManager.setting.ShowFPS = true;
        }
        


        
    }




}
