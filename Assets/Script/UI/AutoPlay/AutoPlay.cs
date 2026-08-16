using System;
using UnityEngine;
using UnityEngine.UI;

public class AutoPlay : MonoBehaviour
{
    public Manager autoPlayManager;
    private Image _image;
    public Sprite before, after,speedUp;
    private int _state;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.sprite = before;
        autoPlayManager.GeneralBool = false;
        autoPlayManager.Weight = 1;
        _state = 0;
    }

    public void SetValue()
    {
        _state++;
        if (_state == 1)
        {
            autoPlayManager.GeneralBool = true;
            autoPlayManager.Weight = 1;
        }
        else if (_state == 2)
        {
            autoPlayManager.GeneralBool = true;
            autoPlayManager.Weight = 4;
            
        }
        else
        {
            _state = 0;
            autoPlayManager.GeneralBool = false;
            autoPlayManager.Weight = 1;
        }

        switch (_state)
        {
            case 0:
                _image.sprite = before;
                break;
            case 1:
                _image.sprite = speedUp;
                break;
            case 2:
                _image.sprite = after;
                break;
        }
    }
}
