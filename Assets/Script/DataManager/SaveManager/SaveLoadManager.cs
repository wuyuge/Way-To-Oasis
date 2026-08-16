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
    public bool isLoad;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }


    private void OnEnable()
    {
        if (shop == null) return;
        bool canClick;
        if (isLoad)
        {
            canClick = !shop.activeSelf;
        }
        else
        {
            canClick = !GlobalData.OnMiniGame && !shop.activeSelf;
        }

        
        _button.interactable = canClick;
    }

    

    public void Open()
    {
        linkMenu.SetActive(true);
    }
    

}
