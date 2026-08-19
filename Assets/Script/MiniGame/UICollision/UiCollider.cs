using UnityEngine;

public static class UiCollider
{
    /// <summary>
    /// 获取 UI 四个角的屏幕坐标
    /// </summary>
    private static Vector2[] GetScreenCorners(RectTransform rectTransform)
    {
        if (rectTransform == null)
            return new Vector2[4];

        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);

        // 1. 获取父级Canvas并判空
        Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("RectTransform父物体不存在Canvas！", rectTransform);
            return new Vector2[4];
        }

        // 2. 自动适配Overlay模式，拿到有效相机
        Camera renderCam;
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            renderCam = Camera.main;
        }
        else
        {
            renderCam = canvas.worldCamera;
        }

        // 兜底相机判空，防止崩溃
        if (renderCam == null)
        {
            Debug.LogError("未能获取有效UI渲染相机，请检查Canvas配置！", canvas);
            return new Vector2[4];
        }

        Vector2[] screenCorners = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            screenCorners[i] = RectTransformUtility.WorldToScreenPoint(renderCam, worldCorners[i]);
        }
        return screenCorners;
    }

    /// <summary>
    /// 检测两个UI（支持旋转、圆形、矩形）是否碰撞
    /// 方法名、参数、返回值 完全不变，旧代码无需修改
    /// </summary>
    public static bool IsCollision(RectTransform a, RectTransform b)
    {
        if (a == null || b == null) return false;
        if (!a.gameObject.activeInHierarchy || !b.gameObject.activeInHierarchy) return false;

        Vector2[] cornersA = GetScreenCorners(a);
        Vector2[] cornersB = GetScreenCorners(b);
        return SeparatingAxisTheorem(cornersA, cornersB);
    }

    #region 旋转矩形精准碰撞算法 (SAT)
    private static bool SeparatingAxisTheorem(Vector2[] polyA, Vector2[] polyB)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector2 edge = polyA[(i + 1) % 4] - polyA[i];
            Vector2 axis = new Vector2(-edge.y, edge.x).normalized;

            if (!IsOverlapOnAxis(polyA, polyB, axis))
                return false;
        }

        for (int i = 0; i < 4; i++)
        {
            Vector2 edge = polyB[(i + 1) % 4] - polyB[i];
            Vector2 axis = new Vector2(-edge.y, edge.x).normalized;

            if (!IsOverlapOnAxis(polyA, polyB, axis))
                return false;
        }

        return true;
    }

    private static bool IsOverlapOnAxis(Vector2[] a, Vector2[] b, Vector2 axis)
    {
        ProjectPolygon(a, axis, out float minA, out float maxA);
        ProjectPolygon(b, axis, out float minB, out float maxB);
        return !(maxA < minB || maxB < minA);
    }

    private static void ProjectPolygon(Vector2[] poly, Vector2 axis, out float min, out float max)
    {
        float proj = Vector2.Dot(poly[0], axis);
        min = proj;
        max = proj;

        for (int i = 1; i < poly.Length; i++)
        {
            proj = Vector2.Dot(poly[i], axis);
            min = Mathf.Min(min, proj);
            max = Mathf.Max(max, proj);
        }
    }
    #endregion
}