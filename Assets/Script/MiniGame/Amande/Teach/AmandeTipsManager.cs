using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AmandeTipsManager : Draggable
{
    public AmandeTips tips;
    public TextMeshProUGUI tipsText, tipsName;
    private bool _isOn;
    public GameObject tipsBar;
    public Canvas tipsCanvas;
    public bool dragging;
    // 鼠标偏移，可在Inspector直接调
    public Vector2 offset = new Vector2(15, -15);
    public List<RectTransform> teachList;
    public RectTransform rectTransform;
    public Manager language;
    public AudioSource aS;
    
    public override void Awake()
    {
        base.Awake();
        rectTransform = GetComponent<RectTransform>();
        foreach (Transform value in transform.parent)
        {
            if (value != transform)
            {
                teachList.Add(value.GetComponent<RectTransform>());
            }
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        dragging = true;
        OnClick();
    }

    public void SetTextBar()
    {
        
        if (dragging)
        {
            dragging = false;
            return;
        }
        aS.Play();
        if (!IsLastSibling())
        {
            foreach (var value in teachList)
            {
                if (CompareUILayer(rectTransform, value) == 1)
                {
                    continue;
                }

                
                if (UiCollider.IsCollision(value,rectTransform))
                {
                    return;
                }
            }
        }
        tipsBar.SetActive(true);
        if (tipsBar.activeSelf)
        {
            Vector2 uiPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                tipsCanvas.GetComponent<RectTransform>(),
                Input.mousePosition,
                tipsCanvas.worldCamera,
                out uiPosition
            );

            // 加上偏移
            uiPosition += offset;
            tipsBar.GetComponent<RectTransform>().anchoredPosition = uiPosition;
            
            tipsText.text = language.isEn ? tips.enDescription : tips.description;
            tipsName.text = language.isEn ? tips.enMedicineName : tips.medicineName;
        }
    }

    public void OnMouseExit()
    {
        tipsBar.SetActive(false);
    }

    public void OnClick()
    {
        transform.SetAsLastSibling();
    }
    
    // 当前物体是不是父级最后一个子对象
    bool IsLastSibling()
    {
        return transform.GetSiblingIndex() == transform.parent.childCount - 1;
    }
    
    /// <summary>
    /// 比较两个UI谁在更上层
    /// 返回值：
    ///  1 : uiA 在 uiB 上面
    /// -1 : uiB 在 uiA 上面
    ///  0 : 同级一样高
    /// </summary>
    public int CompareUILayer(Transform uiA, Transform uiB)
    {
        int indexA = uiA.GetSiblingIndex();
        int indexB = uiB.GetSiblingIndex();

        if (indexA > indexB)
            return 1;
        else if (indexA < indexB)
            return -1;
        else
            return 0;
    }
}
