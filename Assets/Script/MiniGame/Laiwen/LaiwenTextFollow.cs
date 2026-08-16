using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LaiwenTextFollow : MonoBehaviour
{
    [Header("鼠标偏移量（可选）")]
    public Vector2 offset;
    public Vector2 flipOffset;

    [Header("是否平滑跟随")]
    public bool isSmooth = true;

    [Header("平滑速度（越大跟随越快）")]
    public float smoothSpeed = 20f;

    [Header("边界限制")]
    [Tooltip("将这个物体拖入：UI只能在这个RectTransform范围内移动")]
    public RectTransform limitArea;

    [Header("边界内边距（让UI不贴边）")]
    public Vector2 padding = new Vector2(10, 10);

    private RectTransform rectTransform;
    public RectTransform image;
    public RectTransform text;
    public RectTransform flipPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // 关闭UI射线检测（修复版：有组件才操作，无组件不报错）
        CanvasRenderer canvasRenderer = GetComponent<CanvasRenderer>();
        if (canvasRenderer != null)
        {
            canvasRenderer.cullTransparentMesh = true;
        }

        // 修复：先获取再判断，不存在就不执行
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
        }
    }

    void Update()
    {
        FollowMouse();
    }

    void FollowMouse()
    {
        Vector2 mouseUIPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent.GetComponent<RectTransform>(),
            Input.mousePosition,
            Camera.main,
            out mouseUIPos);

        Vector2 targetPos;
        if (mouseUIPos.x > flipPosition.anchoredPosition.x)
        {
            image.localScale = new Vector3( -Mathf.Abs(image.localScale.x), Mathf.Abs(image.localScale.y),
                -Mathf.Abs(image.localScale.z));
            text.localScale = new Vector3( -Mathf.Abs(text.localScale.x), Mathf.Abs(text.localScale.y),
                Mathf.Abs(text.localScale.z));
            targetPos = mouseUIPos + flipOffset;
        }
        else
        {
            image.localScale = new Vector3( Mathf.Abs(image.localScale.x), Mathf.Abs(image.localScale.y),
                Mathf.Abs(image.localScale.z));
            text.localScale = new Vector3( Mathf.Abs(text.localScale.x), Mathf.Abs(text.localScale.y),
                Mathf.Abs(text.localScale.z));
            targetPos = mouseUIPos + offset;
        }
        

        // 限制移动范围
        if (limitArea != null)
        {
            targetPos = GetClampedPosition(targetPos);
        }

        // 移动
        if (isSmooth)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(
                rectTransform.anchoredPosition,
                targetPos,
                smoothSpeed * Time.deltaTime);
        }
        else
        {
            rectTransform.anchoredPosition = targetPos;
        }
    }

    Vector2 GetClampedPosition(Vector2 targetPos)
    {
        Rect rect = limitArea.rect;

        float minX = rect.xMin + padding.x;
        float maxX = rect.xMax - padding.x;
        float minY = rect.yMin + padding.y;
        float maxY = rect.yMax - padding.y;

        float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetPos.y, minY, maxY);

        return new Vector2(clampedX, clampedY);
    }
}