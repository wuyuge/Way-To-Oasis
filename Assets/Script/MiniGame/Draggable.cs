using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

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
        startPosition = GetComponent<RectTransform>().position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _rectTransform, eventData.position, eventData.pressEventCamera, out Vector3 worldPos))
        {
            // 计算 UI 在限制区域内的局部坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                limitArea, worldPos, eventData.pressEventCamera, out Vector2 localPoint);

            // 限制局部坐标在限制区域的尺寸范围内
            // Clamp 函数确保数值在 min 和 max 之间
            localPoint.x = Mathf.Clamp(localPoint.x, -limitArea.rect.width / 2 + _rectTransform.rect.width / 2, limitArea.rect.width / 2 - _rectTransform.rect.width / 2);
            localPoint.y = Mathf.Clamp(localPoint.y, -limitArea.rect.height / 2 + _rectTransform.rect.height / 2, limitArea.rect.height / 2 - _rectTransform.rect.height / 2);

            // 更新位置
            _rectTransform.anchoredPosition = localPoint;
        }
    }
    
}

