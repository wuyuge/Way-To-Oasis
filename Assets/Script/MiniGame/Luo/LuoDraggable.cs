using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class LuoDraggable : Draggable, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField]
    public bool canRelease;
    
    
    private Vector3 _startWorldPosition; // 使用世界坐标保存初始位置

    public override void Awake()
    {
        base.Awake();
        limitArea = transform.parent.GetComponent<RectTransform>();
        
        // 保存世界坐标的初始位置
        _startWorldPosition = transform.position;
    }

    public void CollisionEnter()
    {
        canRelease = true;
    }
    
    public void CollisionExit()
    {
        canRelease = false;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (canRelease) canRelease = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!canRelease)
        {
            // 使用世界坐标重置位置
            transform.position = _startWorldPosition;
            
            // 如果需要同时重置本地位置，可以加上这行
            // rectTransform.anchoredPosition = _startAnchoredPosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 可以在这里更新起始位置，如果需要的话
        // _startWorldPosition = transform.position;
    }
    
    public void SetLimitArea(RectTransform area)
    {
        limitArea = area;
    }
    
    
}