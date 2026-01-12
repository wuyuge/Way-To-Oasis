using System;
using UnityEngine;
using UnityEngine.UI;

public class AutoPlay : MonoBehaviour
{
    public Manager autoPlayManager;
    private Image _image;
    public Sprite before, after;

    private void Awake()
    {
        _image = GetComponent<Image>();
        gameObject.GetComponent<Toggle>().isOn = autoPlayManager.GeneralBool;
        if (autoPlayManager.GeneralBool)
        {
            _image.sprite = after;
            return;
        }
        _image.sprite = before;
        
    }

    public void SetValue(bool value)
    {
        autoPlayManager.GeneralBool = value;
        if (value)
        {
            _image.sprite = after;
            return;
        }
        _image.sprite = before;
    }
}
