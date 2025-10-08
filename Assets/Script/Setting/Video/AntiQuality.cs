using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AntiQuality : MonoBehaviour
{
    private Camera mainCamera;
    private UniversalAdditionalCameraData cameraData;
    private TMP_Dropdown UI;

    private void Awake()
    {
        UI = gameObject.GetComponent<TMP_Dropdown>();
        mainCamera = Camera.main;
        // 获取相机的 URP 扩展数据组件
        cameraData = mainCamera.GetComponent<Camera>().GetUniversalAdditionalCameraData();
    }

    private void OnEnable()
    {

        SetQuality(UI.value);
    }



    public void SetQuality(int Value)
    {
        switch (Value)
        {
            case 0:
                SetSMAALowQuality();
                
                break;
               
            case 1:
                SetSMAAMediumQuality(); 
                break;
            
            case 2:
                SetSMAAHighQuality();
                break;

        }
            
    }

    void SetSMAALowQuality()
    {
        cameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
        cameraData.antialiasingQuality = AntialiasingQuality.Low;
    }
    void SetSMAAMediumQuality()
    {
        cameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
        cameraData.antialiasingQuality = AntialiasingQuality.Medium;
    }
    void SetSMAAHighQuality()
    {
        cameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
        cameraData.antialiasingQuality = AntialiasingQuality.High;
    }


}
