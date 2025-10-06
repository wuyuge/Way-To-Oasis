using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VSyncSetting : MonoBehaviour
{
    

    public void SetVSync(int vSync)
    {
        QualitySettings.vSyncCount = vSync;
        Debug.Log("´¹Ö±Í¬²½ " + vSync);
    }




}
