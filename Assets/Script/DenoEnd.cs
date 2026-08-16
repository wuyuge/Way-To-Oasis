using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DenoEnd : MonoBehaviour
{
    public string cn, en;
    public TextMeshProUGUI text;
    public float interval;
    public Manager language;
    public List<GameObject> buttons;

    private void OnEnable()
    {
        StartCoroutine(ShowText());
    }


    private IEnumerator ShowText()
    {
        
        foreach (var value in language.isEn ? en : cn)
        {
            yield return new WaitForSeconds(interval);
            text.text += value;
        }
        Invoke(nameof(SetButton),0.5f);
    }

    private void SetButton()
    {
        foreach (var value in buttons)
        {
            value.SetActive(true);
        }
    }
}
