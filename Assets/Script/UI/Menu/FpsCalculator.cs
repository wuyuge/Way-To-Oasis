using UnityEngine;

public static class FPSCounter
{
    // 帧率计算的采样间隔（秒），值越小更新越频繁但波动越大
    private static float updateInterval = 0.5f;
    private static float accum = 0f;       // 累计时间
    private static int frames = 0;         // 累计帧数
    private static float timeLeft;         // 距离下次更新的剩余时间
    private static float currentFPS = 0f;  // 当前帧率

    /// <summary>
    /// 计算当前帧率（需在Update中调用）
    /// </summary>
    /// <returns>平滑后的帧率值</returns>
    public static float CalculateFPS()
    {
        // 初始化剩余时间
        if (timeLeft <= 0f)
        {
            // 计算并缓存帧率（四舍五入到整数）
            currentFPS = Mathf.Round(frames / updateInterval);
            // 重置计数器
            accum = 0f;
            frames = 0;
            // 重置采样间隔
            timeLeft = updateInterval;
        }
        else
        {
            // 累加每帧时间（Time.deltaTime是两帧之间的时间）
            accum += Time.deltaTime;
            // 减少剩余时间
            timeLeft -= Time.deltaTime;
            // 累加帧数
            frames++;
        }

        return currentFPS;
    }

    
}