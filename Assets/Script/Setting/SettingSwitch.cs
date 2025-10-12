using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSwitch : MonoBehaviour
{
    public void Click()
    {
        gameObject.transform.parent.SetAsLastSibling();
    }
}
