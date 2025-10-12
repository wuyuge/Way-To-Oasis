using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskManager : MonoBehaviour
{

    public bool ClickClose = true;
    private bool ClickDelay = false;

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space)) && ClickClose && !ClickDelay)
        {
            ClickDelay = true;
            Invoke("SetMask", 0.5f);

        }
    }

    public void SetClik(bool Value)
    {
        ClickClose = Value;
    }

    void SetMask()
    {
        ClickDelay = false;
        gameObject.SetActive(false);
    }


    private void OnDisable()
    {
        ClickDelay = false;
        ClickClose = true;
    }


}
