using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoPiece : Draggable, IPointerEnterHandler
{
    public RectTransform RectTransform { get; private set; }
    public Transform box;

    // 【新增】用标志位控制是否可拖拽，替代 enabled=false，保留 OnPointerEnter 能力
    private bool _canDrag = true;
    public bool CanDrag => _canDrag;

    public override void Awake()
    {
        base.Awake();
        RectTransform = GetComponent<RectTransform>();
        limitArea = transform.parent.GetComponent<RectTransform>();
        box = transform.parent;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetGlobalPiece();
    }

    public void SetGlobalPiece()
    {
        BoGlobalData.CurrentPiece = RectTransform;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (BoGlobalData.Complete)
        {
            return;
        }
        
        BoPiece rootPiece = GetRootPiece();
        if (rootPiece == null) return;

        if (rootPiece == this)
        {
            // 只有根碎片自己需要检查 _canDrag
            if (!_canDrag) return;
            base.OnDrag(eventData);
        }
        else
        {
            // 子碎片直接代理给根，不检查自己的 _canDrag
            rootPiece.OnDrag(eventData);
        }
    }

    /// <summary>
    /// 向上查找组合最顶层根碎片
    /// </summary>
    public BoPiece GetRootPiece()
    {
        Transform top = transform;
        while (top.parent != null && top.parent.GetComponent<BoPiece>() != null)
        {
            top = top.parent;
        }
        return top.GetComponent<BoPiece>();
    }

    public void TryDisable()
    {
        if (transform.parent != box)
        {
            // 【修改】不再禁用整个组件，只关闭拖拽能力
            _canDrag = false;
        }
    }
}