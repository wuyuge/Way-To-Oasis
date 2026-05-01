using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ShopIntro : MonoBehaviour
{
    public bool click = false;
    private Animator _animator;
    public int curInt = 0;
    public VideoPlayer video;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        curInt = 2;
        click = false;
        video.targetTexture.Release();
    }

    private void Update()
    {
        if (click && Input.anyKeyDown)
        {
            curInt++;
            if (curInt > 6)
            {
                curInt = 6;
            }
            _animator.SetInteger("Phase", curInt);
        }
    }

    public void SetClick()
    {
        click = true;
        
    }

    public void SetShop()
    {
        GlobalData.Progress.SetShop();
    }
}
