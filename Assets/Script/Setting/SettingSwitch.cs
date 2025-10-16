using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSwitch : MonoBehaviour
{
    public GameObject LinkObj;
    public void Click()
    {
        LinkObj.transform.SetAsLastSibling();
        LinkObj.SetActive(true);
        gameObject.transform.parent.parent.gameObject.SetActive(false);
    }
}
