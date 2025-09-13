using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightSystem : MonoBehaviour
{
    [Tooltip("控制昼夜变化的颜色渐变")]
    public Gradient gradient;

    [Tooltip("当前时间进度(0-1)，0表示最暗，1表示最亮")]
    [Range(0f, 1f)] public float time;

    [Tooltip("是否启用昼夜系统")]
    public bool on = true;
    public bool complete;
    [Tooltip("完成一次昼夜循环所需的秒数")]
    public float cycleDurationInSeconds = 60f; // 默认60秒完成一次循环
    public Progress progress;
    private float timeIncrementPerFixedUpdate;
    private Light2D lightComponent;
    public BackGroundMoving BackGround;
    [Header("渐变滑块")]
    public float Frist, Second;

    void Start()
    {
        // 获取Light2D组件并缓存，避免重复获取
        lightComponent = GetComponent<Light2D>();

        // 计算每帧的时间增量，使昼夜循环能在指定秒数内完成
        CalculateTimeIncrement();
    }

    // 计算时间增量的方法，当修改循环时间后可以手动调用
    public void CalculateTimeIncrement()
    {
        // FixedUpdate每秒大约执行50次
        // 总帧数 = 循环秒数 * 50
        // 每帧增量 = 1 / 总帧数
        timeIncrementPerFixedUpdate = 1f / (cycleDurationInSeconds * 50f);
    }

    private void FixedUpdate()
    {
        if (on && lightComponent != null)
        {
            // 更新时间进度
            if(time < Frist)
            {
                time += 0.8f * timeIncrementPerFixedUpdate;
            }
            else
            time += timeIncrementPerFixedUpdate;

            if (progress.start && time > Frist)//清晨
            { time = Frist; BackGround.open = false; }
            else if (progress.talk && time > Second)//黄昏
            { time = Second; BackGround.open = false; }
            else if (!progress.start　&& !progress.food) BackGround.open = on;
            else if(progress.food) { BackGround.open = false; }

            
            // 确保时间在0-1范围内循环
            if (time >= 1f)
            {
                time -= 1f; // 减去1而不是设为0，保持循环的连续性
                on = false;
                complete = true;
            }

            // 应用颜色变化
            lightComponent.color = gradient.Evaluate(time);
        }
    }

    // 当循环时间改变时自动重新计算增量
    private void OnValidate()
    {
        // 确保循环时间不会小于0.1秒，避免数值问题
        if (cycleDurationInSeconds < 0.1f)
        {
            cycleDurationInSeconds = 0.1f;
        }

        // 在编辑器模式下也更新时间增量
        CalculateTimeIncrement();
    }

    public void Reset_Color()
    {
        lightComponent.color = gradient.Evaluate(time);
    }


}
