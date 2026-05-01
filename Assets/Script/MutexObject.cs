using System;
using System.Collections.Generic;
using UnityEngine;

public class MutexObject : MonoBehaviour
{
    public List<GameObject> offObjects;
    private Animator _anim;
    private bool _close;

    private void OnEnable()
    {
        foreach (var value in offObjects)
        {
            value.SetActive(false);
        }
        _anim = GetComponent<Animator>();
        _close = false;
    }

    private void LateUpdate()
    {
        if (GlobalData.ShowText.CanShowText && !_close)
        {
            _anim.SetTrigger("Close");
            _close = true;
        }
        
    }
}
