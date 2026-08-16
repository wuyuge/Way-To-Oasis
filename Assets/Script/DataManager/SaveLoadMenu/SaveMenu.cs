using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 用于管理存档界面
/// </summary>

public class SaveMenu : MonoBehaviour,ISaveMenuInterface
{
    public List<FileButtonRefresh> Buttons;
    public Manager autoSaveIsOn;
    public ScrollRect scrollRect;
    
    private void Awake()
    {
        SLManager.SaveMenu = this;
    }

    private void OnEnable()
    {
        StartCoroutine(ResetScroll());
    }

    

    IEnumerator ResetScroll()
    {
        yield return null;
        scrollRect.verticalNormalizedPosition = 1; // 1=顶部，0=底部！注意和scrollbar反过来！
    }

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
