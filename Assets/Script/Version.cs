using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Version : MonoBehaviour
{
    public TextMeshProUGUI text;
    void Start()
    {
        text.text = "Version:" + Application.version;
    }

    
}
