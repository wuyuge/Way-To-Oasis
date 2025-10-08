using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScreenMode : MonoBehaviour
{
    public void SetMode(int Value)
    {
        switch (Value)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Debug.Log("设置为 全屏");
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Debug.Log("设置为 窗口模式");
                break;
        }
        return;
    }
}
