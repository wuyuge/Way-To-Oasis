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

    private Coroutine rainCoroutine;       // 协程引用，用于控制状态

    public Volume postProcessing;
    private LiftGammaGain gammaGain;
    public float transitionSpeed = 2f;     // 过渡速度（值越大变化越快）
    private float targetAlpha;             // 目标alpha值
    private float currentAlpha;            // 当前alpha值

    // 下雨和非下雨状态的目标值
    public float rainAlpha = -0.2f;        // 下雨时的alpha值
    public float clearAlpha = 0f;          // 晴天时的alpha值

    private void Start()
    {
        // 初始化Volume组件
        if (postProcessing != null && postProcessing.profile.TryGet(out gammaGain))
        {
            // 初始状态设置
            currentAlpha = clearAlpha;
            targetAlpha = clearAlpha;
            UpdatePostProcessing();
        }

        // 初始时确保雨系统状态正确
        if (rainSystem != null)
        {
            rainSystem.SetActive(isRaining);
        }

        // 启动随机下雨协程
        StartRandomRain();
    }

    private void Update()
    {
        // 只有当Volume和LiftGammaGain组件有效时才更新
        if (postProcessing != null && gammaGain != null)
        {
            // 平滑过渡当前alpha值到目标值
            if (Mathf.Abs(currentAlpha - targetAlpha) > 0.01f)
            {
                currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * transitionSpeed);
                UpdatePostProcessing();
            }
        }
    }

    // 更新PostProcessing参数
    private void UpdatePostProcessing()
    {
        gammaGain.gamma.value = new Vector4(1f, 1f, 1f, currentAlpha);
        gammaGain.gain.value = new Vector4(1f, 1f, 1f, currentAlpha);
    }

    /// <summary>
    /// 启动随机下雨的协程
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
    /// 随机下雨循环的协程
    /// </summary>
    private IEnumerator RandomRainCycle()
    {
        while (true)
        {
            // 随机等待一段时间后开始下雨
            float waitTime = Random.Range(rainIntervalRange.x, rainIntervalRange.y);
            yield return new WaitForSeconds(waitTime);

            // 开始下雨 - 设置目标值为下雨状态
            isRaining = true;
            targetAlpha = rainAlpha;
            UpdateRainSystem();

            // 随机下雨持续时间
            float rainDuration = Random.Range(rainDurationRange.x, rainDurationRange.y);
            yield return new WaitForSeconds(rainDuration);

            // 停止下雨 - 设置目标值为晴天状态
            isRaining = false;
            targetAlpha = clearAlpha;
            UpdateRainSystem();
        }
    }

    /// <summary>
    /// 更新雨系统的激活状态
    /// </summary>
    private void UpdateRainSystem()
    {
        if (rainSystem != null)
        {
            rainSystem.SetActive(isRaining);
        }
    }

    /// <summary>
    /// 手动停止随机下雨
    /// </summary>
    public void StopRandomRain()
    {
        if (rainCoroutine != null)
        {
            StopCoroutine(rainCoroutine);
            rainCoroutine = null;
        }
        isRaining = false;
        targetAlpha = clearAlpha;
        UpdateRainSystem();
    }
}