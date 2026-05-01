using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ParallaxBackGround : MonoBehaviour
{
    [System.Serializable]
    public class LimitRange
    {
        public float max, min;
    }

    public LimitRange xRange, yRange;
    private float _screenWidth, _screenHeight;
    public float rateX, rateY;
    public float xLength, yLength;
    private RectTransform _rectTransform;
    [FormerlySerializedAs("filp")] public bool flip;

    // 平滑速度（可在Inspector调大小）
    [Header("平滑参数")]
    public float smoothSpeed = 8f;

    // 目标位置
    private Vector2 _targetPos;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _screenWidth = Screen.width;
        _screenHeight = Screen.height;

        xLength = xRange.max + xRange.min;
        yLength = yRange.max + yRange.min;
    }

    private void Update()
    {
        // 计算鼠标比例
        rateX = Input.mousePosition.x / _screenWidth;
        rateY = Input.mousePosition.y / _screenHeight;

        if (flip)
        {
            rateX = 1 - rateX;
            rateY = 1 - rateY;
        }

        // 计算目标位置
        _targetPos = new Vector2(rateX * xLength, rateY * yLength);

        // 平滑移动（核心！）
        _rectTransform.anchoredPosition = Vector2.Lerp(
            _rectTransform.anchoredPosition,
            _targetPos,
            smoothSpeed * Time.deltaTime
        );
    }
}