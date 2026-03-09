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
        
        // 安全检查：确保限制区域已赋值
        if (limitArea == null)
        {
            Debug.LogWarning($"[{gameObject.name}] 未设置限制区域(limitArea)，拖拽将不受限制！", this);
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        // 如果没有设置限制区域，直接移动
        if (limitArea == null)
        {
            UpdatePositionWithoutLimit(eventData);
            return;
        }

        // 核心修正：将屏幕触摸点转换为限制区域的局部坐标
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                limitArea, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            // 计算限制区域的有效范围（扣除拖拽对象自身的尺寸）
            // 基于limitArea的局部坐标系计算边界
            float minX = -limitArea.rect.width / 2 + _rectTransform.rect.width / 2;
            float maxX = limitArea.rect.width / 2 - _rectTransform.rect.width / 2;
            float minY = -limitArea.rect.height / 2 + _rectTransform.rect.height / 2;
            float maxY = limitArea.rect.height / 2 - _rectTransform.rect.height / 2;

            // 关键修正：直接限制localPoint，不再额外减去limitArea的锚点坐标
            localPoint.x = Mathf.Clamp(localPoint.x, minX, maxX);
            localPoint.y = Mathf.Clamp(localPoint.y, minY, maxY);

            // 将限制后的局部坐标赋值给拖拽对象的锚点位置
            _rectTransform.anchoredPosition = localPoint;
        }
    }

    /// <summary>
    /// 无限制区域时的移动逻辑
    /// </summary>
    private void UpdatePositionWithoutLimit(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform.parent as RectTransform, 
                eventData.position, 
                eventData.pressEventCamera, 
                out Vector2 localPoint))
        {
            _rectTransform.anchoredPosition = localPoint;
        }
    }

    /// <summary>
    /// 重置到初始位置（可选方法，方便外部调用）
    /// </summary>
    public virtual void ResetToStartPosition()
    {
        _rectTransform.anchoredPosition = startPosition;
    }
}