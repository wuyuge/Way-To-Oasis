using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbandonTips : MonoBehaviour
{
    public Progress progress;
    public Manager Abandon;
    public void Cancel()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void Confirm()
    {
        progress.SwitchProgress();
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void SetShow(bool Value)
    {
        Abandon.GeneralBool = Value;
    }
}
