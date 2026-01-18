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
    public override void Awake()
    {
        base.Awake();
        limitArea = transform.parent.GetComponent<RectTransform>();
    }

    public void CollisionEnter()
    {
        canRelease = true;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        canRelease = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
        if (!canRelease)
        {
            gameObject.GetComponent<RectTransform>().position = startPosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
}
