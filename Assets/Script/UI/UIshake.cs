using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIshake : MonoBehaviour
{
    [Header("震动参数")]
    public float minShakeRange = 1f;    // 最小震动幅度
    public float maxShakeRange = 3f;    // 最大震动幅度
    public float minInterval = 0.1f;    // 震动间隔（最小）
    public float maxInterval = 0.5f;    // 震动间隔（最大）

    private RectTransform uiRect;
    private Vector2 originalPos;
    private Coroutine shakeCoroutine;   // 保存协程引用，用于停止
    private bool isShaking = false;     // 震动状态标记

    public float StopDelay;

    void Awake()
    {
        uiRect = GetComponent<RectTransform>();
        originalPos = uiRect.anchoredPosition;
    }

    // 外部调用：开始震动
    public void StartShake()
    {
        if (isShaking) return; // 避免重复启动
        isShaking = true;
        shakeCoroutine = StartCoroutine(ShakeLoop());
        Invoke("StopShake", StopDelay);

    }

    // 外部调用：停止震动
    public void StopShake()
    {
        if (!isShaking) return;
        isShaking = false;
        StopCoroutine(shakeCoroutine); // 停止协程
        uiRect.anchoredPosition = originalPos; // 复位
    }

    // 震动循环逻辑
    private IEnumerator ShakeLoop()
    {
        while (isShaking)
        {
            // 随机偏移
            float x = Random.Range(-maxShakeRange, maxShakeRange);
            float y = Random.Range(-maxShakeRange, maxShakeRange);
            uiRect.anchoredPosition = originalPos + new Vector2(x, y);

            // 等待间隔后复位
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            uiRect.anchoredPosition = originalPos;

            // 下一次震动前的等待
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
        }
    }
}

