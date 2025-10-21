using UnityEngine;

/// <summary>
/// UI元素跟随器，使当前元素保持与目标元素的固定偏移
/// </summary>
[ExecuteAlways] // 支持在编辑模式下实时预览跟随效果
public class TalkBarLinker : MonoBehaviour
{
    [Header("跟随目标")]
    [Tooltip("需要跟随的目标UI元素")]
    public RectTransform linkTarget; // 修正命名，更清晰

    [Header("偏移设置")]
    [Tooltip("X轴方向的偏移量")]
    public float offsetX;           // 修正命名，语义更明确
    [Tooltip("Y轴方向的偏移量")]
    public float offsetY;

    // 缓存当前元素的RectTransform，避免重复获取
    private RectTransform _selfRect;

    private void Awake()
    {
        // 初始化时获取自身RectTransform并缓存
        _selfRect = GetComponent<RectTransform>();
    }

    private void LateUpdate() // 改用LateUpdate，确保目标位置更新后再跟随
    {
        // 目标为空时不执行操作，避免空引用错误
        if (linkTarget == null)
            return;

        // 直接操作anchoredPosition，在UI系统中更高效（避免转换世界坐标）
        _selfRect.anchoredPosition = new Vector2(
            linkTarget.anchoredPosition.x + offsetX,
            linkTarget.anchoredPosition.y + offsetY
        );
    }

    // 编辑模式下实时更新（可选，根据需求开启）
    private void OnValidate()
    {
        if (_selfRect == null)
            _selfRect = GetComponent<RectTransform>();

        if (linkTarget != null)
        {
            _selfRect.anchoredPosition = new Vector2(
                linkTarget.anchoredPosition.x + offsetX,
                linkTarget.anchoredPosition.y + offsetY
            );
        }
    }
}