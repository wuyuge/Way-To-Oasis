using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于管理存档界面
/// </summary>

public class SaveMenu : MonoBehaviour,ISaveMenuInterface
{
    public List<FileButtonRefresh> Buttons;
    public Manager autoSaveIsOn;
    
    public void UpdateSaveMenu()
    {
        foreach (var value in Buttons)
        {
            
            value.Refresh();
            
            
        }
    }

    private void OnDisable()
    {
        autoSaveIsOn.GeneralBool = true;
    }
}
