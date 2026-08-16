using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoItem : MonoBehaviour
{
    public BoItemData data;
    private RectTransform rectTransform;
    public bool set;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    

    private void Update()
    {
        if (IsMouseOverRect(rectTransform,Camera.main))
        {
            if (!BoGlobalData.Complete)
            {
                return;
            }

            if (BoGlobalData.TalkSys.curData != data)
            {
                BoGlobalData.TalkSys.StartTalk(data);
                BoGlobalData.itemText.SetOpen(true, data);
                set = true;
            }
        }
        else
        {
            if (set)
            {
                BoGlobalData.itemText.SetOpen(false, data);
                set = false;
            }
        }
    }

    /// <summary>
    /// [带UI相机版本] 单个UI鼠标重合检测
    /// </summary>
    public static bool IsMouseOverRect(RectTransform rect, Camera uiCamera)
    {
        if (rect == null || !rect.gameObject.activeInHierarchy)
            return false;

        Vector2 mouseScreenPos = Input.mousePosition;
        Vector2 localPos;
        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect, mouseScreenPos, uiCamera, out localPos);

        if (!success) return false;
        return rect.rect.Contains(localPos);
    }
}
