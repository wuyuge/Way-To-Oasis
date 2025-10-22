using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Anti : MonoBehaviour,SettingInitialize
{
    private Camera mainCamera;
    private UniversalAdditionalCameraData cameraData;
    private SettingDataManager Manager;

    public void Initialize(SettingDataManager manager)
    {
        mainCamera = Camera.main;
        // 获取相机的 URP 扩展数据组件
        cameraData = mainCamera.GetComponent<Camera>().GetUniversalAdditionalCameraData();
        Manager = manager;
        SetAnti(manager.setting.Anti);
        GetComponent<TMP_Dropdown>().value = manager.setting.Anti;


    }



    // 设置为 SMAA 并指定质量（示例：设置为 High 质量）
    public void SetAnti(int Value)
    {

        Manager.setting.Anti = Value;


        if(Value == 0)
        {
            
            DisableAntiAliasing();
        }
        else if(Value == 1)
        {
            
            cameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
            cameraData.antialiasingQuality = AntialiasingQuality.High;

        }
        
    }

    // 关闭抗锯齿
    public void DisableAntiAliasing()
    {
        cameraData.antialiasing = AntialiasingMode.None;
    }

    
}

