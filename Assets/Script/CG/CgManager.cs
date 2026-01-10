using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CgManager : MonoBehaviour
{
    public List<Sprite> cgList;
    public Image cgContainer;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public bool ShowCg(int index)
    {
        if (cgList.Count < index)
        {
            Debug.LogError("索引超出cg列表长度");
            return false;
        }
        cgContainer.sprite = cgList[index];
        return true;
        
    }
    
    public void HideCg()
    {
        _animator.SetTrigger("Close");
    }

    public void SetFalse()
    {
        gameObject.SetActive(false);
    }
}
