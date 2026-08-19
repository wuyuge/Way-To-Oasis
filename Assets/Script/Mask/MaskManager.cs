#pragma warning disable
#pragma warning disable CS0612
using System;
using UnityEngine;
[Obsolete]
public class MaskManager : MonoBehaviour
{

    public bool ClickClose = true;
    private bool ClickDelay = false;
    

    public void SetClik(bool Value)
    {
        return;
        ClickClose = Value;
    }

    void SetMask()
    {
        return;
        ClickDelay = false;
        gameObject.SetActive(false);
    }


    private void OnDisable()
    {
        return;
        ClickDelay = false;
        ClickClose = true;
    }


}
