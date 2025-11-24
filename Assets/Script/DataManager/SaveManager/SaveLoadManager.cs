using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SaveLoadManager : MonoBehaviour
{

    public GameObject linkMenu;
    public GameObject shop;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }


    private void OnEnable()
    {
        if (shop != null)
        {
            _button.interactable = !shop.activeSelf;
        }
        
    }


    public void Open()
    {
        linkMenu.SetActive(true);
    }
    

}
