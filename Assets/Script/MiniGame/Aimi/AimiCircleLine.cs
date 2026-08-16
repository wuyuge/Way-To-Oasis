using System;
using UnityEngine;

public class AimiCircleLine : MonoBehaviour
{
    private RectTransform _rectTransform;
    public RectTransform coll;
    [Range(0, 0.1f)] public float rate = 0.02f;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        AimiGlobalManager.LineColl = coll;
    }
    

    private void FixedUpdate()
    {
        // 取真实欧拉角度
        float curAng = _rectTransform.localEulerAngles.z;
        // 持续递减角度 = 顺时针旋转
        float targetAng = curAng - 360f;
        float smoothAng = Mathf.Lerp(curAng, targetAng, rate);

        // 限制角度范围防止溢出
        smoothAng = Mathf.Repeat(smoothAng, 360f);
        _rectTransform.localEulerAngles = new Vector3(0, 0, smoothAng);
    }
}