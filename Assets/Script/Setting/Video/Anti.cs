using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Anti : MonoBehaviour
{
    private Camera mainCamera;
    private UniversalAdditionalCameraData cameraData;
    public GameObject SetQuality;

    void Start()
    {
        mainCamera = Camera.main;
        // 获取相机的 URP 扩展数据组件
        cameraData = mainCamera.GetComponent<Camera>().GetUniversalAdditionalCameraData();
        Debug.Log(cameraData.ToString());
    }

    // 设置为 SMAA 并指定质量（示例：设置为 High 质量）
    public void SetAnti(int Value)
    {
        if(Value == 0)
        {
            SetQuality.SetActive(false);
            DisableAntiAliasing();
        }
        else if(Value == 1)
        {
            SetQuality.SetActive(true);
            transform.Find("SetAntiQuality").gameObject.GetComponent<AntiQuality>().SetQuality(2);
            
        }
        
    }

    // 关闭抗锯齿
    public void DisableAntiAliasing()
    {
        cameraData.antialiasing = AntialiasingMode.None;
    }

    
}

