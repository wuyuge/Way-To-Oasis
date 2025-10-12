using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskManager : MonoBehaviour
{

    public bool ClickClose = true;

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space)) && ClickClose)
        {

            gameObject.SetActive(false);

        }
    }

    public void SetClik(bool Value)
    {
        ClickClose = Value;
    }







}
