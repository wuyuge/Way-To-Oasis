using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSwitch : MonoBehaviour
{
    public GameObject LinkObj;
    public GameObject offObj;

    public void Click()
    {
        LinkObj.transform.SetAsLastSibling();
        LinkObj.SetActive(true);
        offObj.SetActive(false);
    }
}
