using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RandomRain : MonoBehaviour
{
    public bool isRaining = false;          // 当前是否在下雨
    public GameObject rainSystem;          // 雨系统物体
    public Vector2 rainDurationRange = new Vector2(10f, 30f);  // 下雨持续时间范围（秒）
    public Vector2 rainIntervalRange = new Vector2(60f, 180f); // 雨停后下次可能下雨的间隔范围（秒）

    private Coroutine rainCoroutine;       // 随机下雨协程引用
    private Coroutine manualRainCoroutine; // 手动下雨协程引用
    private Coroutine postProcessDelayCoroutine; // 后处理延迟协程

    public Volume postProcessing;
    private LiftGammaGain gammaGain;
    public float transitionSpeed = 2f;     // 过渡速度（值越大变化越快）
    private float targetAlpha;             // 目标alpha值
    private float currentAlpha;            // 当前alpha值

    // 下雨和非下雨状态的目标值
    public float rainAlpha = -0.2f;        // 下雨时的alpha值
    public float clearAlpha = 0f;          // 晴天时的alpha值
    public float postProcessDelay = 0.5f;  // 后处理延迟时间（秒）

    private void Start()
    {
        // 初始化Volume组件和后处理参数
        if (postProcessing != null && postProcessing.profile.TryGet(out gammaGain))
        {
            currentAlpha = clearAlpha;
            targetAlpha = clearAlpha;
            UpdatePostProcessing();
        }
        else
        {
            Debug.LogWarning("未设置后处理Volume或LiftGammaGain组件！");
        }

        // 初始状态设置
        if (rainSystem != null)
        {
            rainSystem.SetActive(isRaining);
        }

        // 启动随机下雨协程
        StartRandomRain();
    }

    private void Update()
    {
        // 平滑过渡后处理效果
        if (gammaGain != null)
        {
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * transitionSpeed);
            UpdatePostProcessing();
        }
    }

    // 更新后处理参数
    private void UpdatePostProcessing()
    {
        if (gammaGain != null)
        {
            gammaGain.gamma.value = new Vector4(1f, 1f, 1f, currentAlpha);
            gammaGain.gain.value = new Vector4(1f, 1f, 1f, currentAlpha);
        }
    }

    /// <summary>
    /// 启动随机下雨循环
    /// </summary>
    public void StartRandomRain()
    {
        if (rainCoroutine != null)
        {
            StopCoroutine(rainCoroutine);
        }
        rainCoroutine = StartCoroutine(RandomRainCycle());
    }

    /// <summary>
    /// 随机下雨循环协程
    /// </summary>
    private IEnumerator RandomRainCycle()
    {
        while (true)
        {
            float waitTime = Random.Range(rainIntervalRange.x, rainIntervalRange.y);
            yield return new WaitForSeconds(waitTime);

            // 随机下雨直接同步后处理（无延迟）
            StartRainImmediate();

            float rainDuration = Random.Range(rainDurationRange.x, rainDurationRange.y);
            yield return new WaitForSeconds(rainDuration);

            StopRainImmediate();
        }
    }

    /// <summary>
    /// 手动开始下雨（无参数版本）
    /// 雨系统立即激活，后处理延迟0.5秒生效
    /// </summary>
    public void ManualStartRain()
    {
        // 停止可能存在的手动下雨和延迟协程
        if (manualRainCoroutine != null)
        {
            StopCoroutine(manualRainCoroutine);
        }
        if (postProcessDelayCoroutine != null)
        {
            StopCoroutine(postProcessDelayCoroutine);
        }

        // 立即激活雨系统，但先不改变后处理
        isRaining = true;
        if (rainSystem != null)
        {
            rainSystem.SetActive(true);
        }

        // 启动后处理延迟协程
        postProcessDelayCoroutine = StartCoroutine(DelayPostProcessStart());

        // 随机生成持续时间并启动计时器
        float duration = Random.Range(rainDurationRange.x, rainDurationRange.y);
        manualRainCoroutine = StartCoroutine(ManualRainTimer(duration));
    }

    /// <summary>
    /// 延迟启动后处理效果的协程
    /// </summary>
    private IEnumerator DelayPostProcessStart()
    {
        yield return new WaitForSeconds(postProcessDelay);
        // 延迟结束后才开始后处理过渡
        targetAlpha = rainAlpha;
        postProcessDelayCoroutine = null;
    }

    /// <summary>
    /// 手动下雨计时器协程
    /// </summary>
    private IEnumerator ManualRainTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        // 停止下雨时同步后处理（无延迟）
        StopRainImmediate();
        manualRainCoroutine = null;
    }

    /// <summary>
    /// 立即开始下雨（同步后处理）
    /// </summary>
    private void StartRainImmediate()
    {
        isRaining = true;
        targetAlpha = rainAlpha;
        if (rainSystem != null)
        {
            rainSystem.SetActive(true);
        }
    }

    /// <summary>
    /// 立即停止下雨（同步后处理）
    /// </summary>
    private void StopRainImmediate()
    {
        isRaining = false;
        targetAlpha = clearAlpha;
        if (rainSystem != null)
        {
            rainSystem.SetActive(false);
        }
    }

    /// <summary>
    /// 停止所有下雨相关协程
    /// </summary>
    public void StopAllRain()
    {
        if (rainCoroutine != null)
        {
            StopCoroutine(rainCoroutine);
            rainCoroutine = null;
        }

        if (manualRainCoroutine != null)
        {
            StopCoroutine(manualRainCoroutine);
            manualRainCoroutine = null;
        }

        if (postProcessDelayCoroutine != null)
        {
            StopCoroutine(postProcessDelayCoroutine);
            postProcessDelayCoroutine = null;
        }

        StopRainImmediate();
    }
}