using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetPlayerName : MonoBehaviour
{
    public Manager PlayerName;
    public TextMeshProUGUI TextBack;

    private void Awake()
    {
        if (PlayerName.TxtLine[0] != "■■"  && !string.IsNullOrEmpty( PlayerName.TxtLine[0]))
        {
            TextBack.text = PlayerName.TxtLine[0];
        }
        else
        {
            TextBack.text = "输入你的名字...";
        }
        
    }

    public void SetName(string name)
    {
        if (!string.IsNullOrEmpty(name))
            PlayerName.TxtLine[0] = name;
        else
        {
            PlayerName.TxtLine[0] = "■■";
            TextBack.text = "输入你的名字...";
        }
    }
}
