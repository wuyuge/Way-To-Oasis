using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AddPlayerName : MonoBehaviour
{
    private TextMeshProUGUI text;
    public Manager PlayerName;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = text.text.Replace("{PlayerName}", PlayerName.TxtLine[0]);
    }

}
