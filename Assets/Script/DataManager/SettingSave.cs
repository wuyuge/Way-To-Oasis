using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSave : MonoBehaviour
{

    public SettingDataManager SettingManager;

    private void Awake()
    {
        SettingManager = GameObject.Find("SaveManager").GetComponent<SettingDataManager>();
    }

    public void Save()
    {
        SettingManager.Save();
    }


}
