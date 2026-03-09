using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoCollider : MonoBehaviour
{
    public GameObject linkObject;
    public bool linked = false;
    public Transform linkPosition;
    private RectTransform _linkObjectRect,_rect;
    private BoMap _linkScript;

    private void Start()
    {
        if (linkObject is null)
        {
            Debug.LogError("连接物体未赋值");
            return;
        }
        _linkObjectRect = linkObject.GetComponent<RectTransform>();
        _rect = GetComponent<RectTransform>();
        _linkScript = linkObject.GetComponent<BoMap>();
    }

    private void LateUpdate()
    {
        if (_linkScript.isLink)
        {
            enabled = false;
        }
        if (_linkObjectRect is not null && !linked)
        {
            if (UiCollider.IsCollision(_linkObjectRect, _rect))
            {
                linked = true;
                linkObject.transform.SetParent(linkPosition,true);
                linkObject.transform.localPosition = Vector3.zero;
                _linkScript.isLink = true;
                _linkScript.enabled = false;
            }
        }
        
    }
}
