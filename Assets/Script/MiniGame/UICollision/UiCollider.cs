using UnityEngine;

public static class UiCollider
{
    // 【修复报错】获取UI在屏幕上的真实矩形
    private static Rect GetScreenRect(RectTransform rectTransform)
    {
        if (rectTransform == null) 
            return new Rect();
        
        // 必须是 4 个元素！修复这里
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        
        Vector2 min = corners[0];
        Vector2 max = corners[2];
        
        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }

    // 方法名不变，兼容你的旧代码
    public static bool IsCollision(RectTransform a, RectTransform b)
    {
        if (a == null || b == null) return false;
        if (!a.gameObject.activeInHierarchy || !b.gameObject.activeInHierarchy) return false;

        Rect rectA = GetScreenRect(a);
        Rect rectB = GetScreenRect(b);
        return rectA.Overlaps(rectB);
    }
}