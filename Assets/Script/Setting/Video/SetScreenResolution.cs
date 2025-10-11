using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetScreenResolution : MonoBehaviour
{
    
    // 存储支持的分辨率列表
    public Resolution[] supportedResolutions;
    private TMP_Dropdown TMP_Dropdown;

    void Start()
    {
        TMP_Dropdown = gameObject.GetComponent<TMP_Dropdown>();
        // 获取当前显示器支持的所有分辨率（去重，保留最高刷新率）
        supportedResolutions = Screen.resolutions;
        // 倒序排序（从高到低）
        System.Array.Reverse(supportedResolutions);
        foreach (Resolution r in supportedResolutions)
        {
            Debug.Log("支持的分辨率: " + r.width + "x" + r.height + " @" + r.refreshRateRatio + "Hz");
        }

        TMP_Dropdown.ClearOptions();
        foreach (Resolution r in supportedResolutions)
        {
            string Hz = r.refreshRateRatio.ToString();
            if (Hz.Contains("."))
            {
                string[] strings = Hz.Split(".");
                Hz = strings[0];
            }
            TMP_Dropdown.options.Add(new TMP_Dropdown.OptionData(r.width + "x" + r.height + " @" + Hz + " Hz"));
        }
        TMP_Dropdown.RefreshShownValue();
    }



    public void SetResolution(int Value)
    {
        Resolution resolution = supportedResolutions[Value];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode,resolution.refreshRateRatio);
        Debug.Log("设置分辨率为: " + resolution.width + "x" + resolution.height);
        return;
    }






}
