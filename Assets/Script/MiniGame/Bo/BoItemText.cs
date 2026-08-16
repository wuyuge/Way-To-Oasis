using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoItemText : MonoBehaviour
{
    [Header("跟随鼠标偏移量")]
    public Vector2 offset = new Vector2(15, -15);
    public Vector2 flipOffset = new Vector2(15, -15);
    public RectTransform flipPoint;
    [Header("是否开启跟随模式")]
    public bool isOpen = true;

    private RectTransform _rect;
    private Canvas _canvas;
    public TextMeshProUGUI text;
    public Image image;
    public Manager language;
    public GameObject button;

    private bool _isFlipped;
    private Vector3 _baseScale;
    private Vector3 _textBaseScale;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();
        BoGlobalData.itemText = this;
        BoGlobalData.Button = button;
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0.0f);
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.0f);
        text.text = "";

        _baseScale = transform.localScale;
        if (text != null)
            _textBaseScale = text.transform.localScale;
    }

    void Update()
    {
        if (!isOpen) return;

        FollowMouse();
    }

    void FollowMouse()
    {
        Vector2 mousePos = Input.mousePosition;

        bool shouldFlip = false;
        if (flipPoint != null)
        {
            Vector2 flipScreenPos = RectTransformUtility.WorldToScreenPoint(
                _canvas.worldCamera, flipPoint.position);
            shouldFlip = mousePos.x < flipScreenPos.x;
        }

        mousePos += shouldFlip ? flipOffset : offset;

        if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            _rect.anchoredPosition = mousePos;
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                mousePos,
                _canvas.worldCamera,
                out Vector2 localPos);
            _rect.anchoredPosition = localPos;
        }

        if (shouldFlip != _isFlipped)
        {
            _isFlipped = shouldFlip;
            float sign = shouldFlip ? -1f : 1f;

            Vector3 scale = _baseScale;
            scale.x = _baseScale.x * sign;
            transform.localScale = scale;

            if (text != null)
            {
                Vector3 ts = _textBaseScale;
                ts.x = _textBaseScale.x * sign;
                text.transform.localScale = ts;
            }
        }
    }

    public void SetOpen(bool openState, BoItemData d)
    {
        if (openState)
        {
            text.color = Color.black;
            image.color = Color.white;
            text.text = language.isEn ? d.description.en : d.description.cn;
        }
        else
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0.0f);
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.0f);
            text.text = "";
        }
    }
}