using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting; // 用于去重排序

public class SetScreenResolution : MonoBehaviour
{
    // 存储支持的分辨率列表
    public Resolution[] supportedResolutions;
    private TMP_Dropdown TMP_Dropdown;

    void Start()
    {
        TMP_Dropdown = gameObject.GetComponent<TMP_Dropdown>();
        
        // 1. 获取所有支持的分辨率，并去重、排序（从高到低）
        supportedResolutions = Screen.resolutions
            .DistinctBy(r => new { r.width, r.height }) // 去重：相同宽高只保留一个
            .OrderByDescending(r => r.width)
            .ThenByDescending(r => r.height)
            .ToArray();

        TMP_Dropdown.ClearOptions();
        List<string> options = new List<string>();

        // 2. 生成分辨率下拉选项
        foreach (Resolution r in supportedResolutions)
        {
            string Hz = r.refreshRateRatio.ToString();
            if (Hz.Contains("."))
            {
                Hz = Hz.Split(".")[0];
            }
            options.Add(r.width + "x" + r.height + " @" + Hz + " Hz");
        }

        TMP_Dropdown.AddOptions(options);
        TMP_Dropdown.RefreshShownValue();

        // 3. 核心：初始化选中当前游戏窗口分辨率
        SetCurrentResolutionAsDefault();
    }

    /// <summary>
    /// 自动选中当前正在使用的分辨率
    /// </summary>
    void SetCurrentResolutionAsDefault()
    {
        int currentWidth = Screen.width;
        int currentHeight = Screen.height;

        // 遍历列表找到当前分辨率的索引
        for (int i = 0; i < supportedResolutions.Length; i++)
        {
            if (supportedResolutions[i].width == currentWidth && 
                supportedResolutions[i].height == currentHeight)
            {
                TMP_Dropdown.value = i; // 设置下拉框选中项
                TMP_Dropdown.RefreshShownValue();
                break;
            }
        }
    }

    /// <summary>
    /// 设置选中的分辨率
    /// </summary>
    public void SetResolution(int value)
    {
        Resolution resolution = supportedResolutions[value];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);
    }
}