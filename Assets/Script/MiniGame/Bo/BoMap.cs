using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoMap : Draggable, IPointerUpHandler, IPointerDownHandler
{
    public List<GameObject> otherMap = new List<GameObject>();
    public List<BoCollider> colliders;
    public bool isLink;

    public override void Awake()
    {
        limitArea = transform.parent.GetComponent<RectTransform>();
        base.Awake();
    }

    private void OnDisable()
    {
        foreach (var value in colliders)
        {
            value.enabled = false;
        }
    }

    // 原有的OnPointerUp（指针在元素上抬起时触发）
    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    // 记录拖拽开始状态
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
}