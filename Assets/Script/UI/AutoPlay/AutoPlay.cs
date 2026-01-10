using System;
using UnityEngine;
using UnityEngine.UI;

public class AutoPlay : MonoBehaviour
{
    public Manager autoPlayManager;
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        gameObject.GetComponent<Toggle>().isOn = autoPlayManager.GeneralBool;
        if (autoPlayManager.GeneralBool)
        {
            _image.color = Color.green;
            return;
        }
        _image.color = new Color32(221,196,165,255);
    }

    public void SetValue(bool value)
    {
        autoPlayManager.GeneralBool = value;
        if (value)
        {
            _image.color = Color.green;
            return;
        }
        _image.color = new Color32(221,196,165,255);
    }
}
