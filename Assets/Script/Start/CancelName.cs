using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CancelName : MonoBehaviour
{
    public TMP_InputField Name;
    public TextMeshProUGUI DefultBar;
    public string DefultText;

    public void Awake()
    {
        DefultText = DefultBar.text;
    }


    public void Click()
    {
        Name.text = string.Empty;
        DefultBar.text = DefultText;

    }


}
