using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoLevelAnim : MonoBehaviour
{
    [Header("动画时长（秒）")]
    public float fadeDuration = 0.35f;
    [Header("OnEnable时是否自动显示")]
    public bool autoShowOnEnable = true;

    public List<Image> images;

    private Coroutine currentFadeCoroutine;
    public Animator full;

    private void OnEnable()
    {
        BoGlobalData.anim = this;
        images.Clear();
        Image[] i = GetComponentsInChildren<Image>(includeInactive: true);
        foreach (var value in i)
        {
            images.Add(value);
        }

        if (autoShowOnEnable)
        {
            Show();
        }
    }

    /// <summary>
    /// 渐显（透明度从0→1）
    /// </summary>
    public void Show()
    {
        StartFade(0f, 1f);
    }

    /// <summary>
    /// 渐隐（透明度从1→0）
    /// </summary>
    public void Hide()
    {
        StartFade(1f, 0f);
        full.SetTrigger("End");
    }

    // 启动渐变，先停止正在运行的动画避免冲突
    private void StartFade(float startAlpha, float targetAlpha)
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }
        currentFadeCoroutine = StartCoroutine(FadeCoroutine(startAlpha, targetAlpha));
    }

    private IEnumerator FadeCoroutine(float startAlpha, float targetAlpha)
    {
        // 初始化起始透明度
        foreach (var img in images)
        {
            if (img == null) continue;
            Color c = img.color;
            c.a = startAlpha;
            img.color = c;
        }

        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            foreach (var img in images)
            {
                if (img == null) continue;
                Color c = img.color;
                c.a = alpha;
                img.color = c;
            }
            yield return null;
        }

        // 最终锁定目标透明度
        foreach (var img in images)
        {
            if (img == null) continue;
            Color c = img.color;
            c.a = targetAlpha;
            img.color = c;
        }

        currentFadeCoroutine = null;
    }

    private void OnDisable()
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = null;
        }
    }
}