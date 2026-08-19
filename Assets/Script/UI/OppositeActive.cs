using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OppositeActive : MonoBehaviour
{
    public GameObject linkObject;
    private Animator _anim;
    [SerializeField]
    private bool disableInTalk;
    private Button _button;

    private void Awake()
    {
        try
        {
            _button = GetComponent<Button>();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message,this);
        }
    }

    public void OnClick()
    {
        
        if (_anim is null)
        {
            _anim = linkObject.GetComponent<Animator>();
        }

        if (!linkObject.activeSelf)
        {
            linkObject.SetActive(true);
        }
        else
        {
            _anim.SetTrigger("Close");
        }
        
    }

    private void Update()
    {
        if (disableInTalk)
        {
            _button.interactable = !GlobalData.ShowText.CanShowText;
        }
    }
}
