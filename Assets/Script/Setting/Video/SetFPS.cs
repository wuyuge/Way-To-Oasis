using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetFPS : MonoBehaviour
{
    public TMP_Dropdown UI;
    private bool VSyncMode;

    private void OnEnable()
    {
        VSyncMode = false;
    }

    public void SetFps(int value)
    {


        switch (value)
        {
            case 0:
                Application.targetFrameRate = 30;
                Debug.Log("设置帧率 30");
                break;
            case 1:
                Application.targetFrameRate = 60;
                Debug.Log("设置帧率 60");
                break;
            case 2:
                Application.targetFrameRate = 120;
                Debug.Log("设置帧率 120");
                break;
            case 3:
                Application.targetFrameRate = 240;
                Debug.Log("设置帧率 240");
                break;
            case 4:
                Application.targetFrameRate = -1;
                Debug.Log("设置帧率 无限");
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
        UI.captionText.text = "已禁用";
        VSyncMode = true;
    }

    void Opened()
    {
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        UI.enabled = true;
        UI.captionText.text = UI.options[UI.value].text;
        VSyncMode = false;
    }



}
