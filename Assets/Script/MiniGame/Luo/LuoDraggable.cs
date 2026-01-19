using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class LuoDraggable : Draggable,IPointerUpHandler,IPointerDownHandler
{
    [SerializeField]
    private bool canRelease;
    public PipeType itemType;
    public override void Awake()
    {
        base.Awake();
        limitArea = transform.parent.GetComponent<RectTransform>();
    }

    public void CollisionEnter()
    {
        canRelease = true;
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
            gameObject.GetComponent<RectTransform>().position = startPosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
}
