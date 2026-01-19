using System.Collections.Generic;
using UnityEngine;

public static class UiCollider
{
    
    // 从 RectTransform 获取屏幕空间 Rect
    private static Rect GetScreenRect(RectTransform rectTransform)
    {
        var size = Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale);
        return new Rect((Vector2)rectTransform.position - size * rectTransform.pivot, size);
    }

    public static bool IsCollision(RectTransform a,RectTransform b)
    {
        var rectA = GetScreenRect(a);
        var rectB = GetScreenRect(b);
        return rectA.Overlaps(rectB);
    }
}
