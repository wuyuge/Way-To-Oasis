using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AbandonTips : MonoBehaviour
{
    public Progress progress;
    public Manager Abandon;
    public AbandonTips confirm;
    public bool show;
    public Toggle toggle;
    public void Cancel()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
        toggle.isOn = false;
    }

    public void Confirm()
    {
        Abandon.GeneralBool = show;
        progress.SwitchProgress(true);
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void SetShow(bool Value)
    {
        confirm.show = Value;
    }
}