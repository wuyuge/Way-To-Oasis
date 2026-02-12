using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Draggable : MonoBehaviour, IDragHandler
{
    // 在 Inspector 面板中拖入你想要限制的区域（通常是另一个 RectTransform）
    [SerializeField] protected RectTransform limitArea;

    private RectTransform _rectTransform;
    [SerializeField]
    protected Vector3 startPosition;

    public virtual void Awake()
    {
        _rectTransform = transform as RectTransform;
        startPosition = _rectTransform.anchoredPosition; // 改用锚点坐标更稳定
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        // 核心修正：直接将屏幕触摸点转换为限制区域的局部坐标（跳过世界坐标转换）
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                limitArea, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            // 限制局部坐标在限制区域内（确保拖拽对象不会超出边界）
            // 计算限制区域的有效范围（扣除拖拽对象自身的尺寸）
            float minX = -limitArea.rect.width / 2 + _rectTransform.rect.width / 2;
            float maxX = limitArea.rect.width / 2 - _rectTransform.rect.width / 2;
            float minY = -limitArea.rect.height / 2 + _rectTransform.rect.height / 2;
            float maxY = limitArea.rect.height / 2 - _rectTransform.rect.height / 2;

            // 修正坐标范围（适配limitArea的锚点和偏移）
            localPoint.x = Mathf.Clamp(localPoint.x - limitArea.anchoredPosition.x, minX, maxX);
            localPoint.y = Mathf.Clamp(localPoint.y - limitArea.anchoredPosition.y, minY, maxY);

            // 关键：将限制后的局部坐标赋值给拖拽对象的锚点位置
            _rectTransform.anchoredPosition = localPoint;
        }
    }
}