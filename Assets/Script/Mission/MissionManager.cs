using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionManager : MonoBehaviour
{
    // UI容器：任务文本的父物体
    public GameObject content;
    // 用于显示任务的文本组件列表
    [SerializeField]
    private List<TextMeshProUGUI> missions = new List<TextMeshProUGUI>();
    // 每日任务配置列表（按天数顺序）
    public List<Mission> missionList;
    private void Start()
    {
        // 初始化任务文本组件列表
        InitMissionTextComponents();
    }

    private void OnEnable()
    {
        UpdateMissionDisplay();
    }

    /// <summary>
    /// 初始化：从content子物体中获取所有TextMeshProUGUI组件
    /// </summary>
    private void InitMissionTextComponents()
    {
        // 清空旧数据，避免重复添加
        missions.Clear();

        // 校验content是否为空
        if (content == null)
        {
            Debug.LogError("Content物体未赋值，请在Inspector中指定任务文本的父物体！", this);
            return;
        }

        // 正确遍历content的子物体（修复原代码的遍历错误）
        foreach (Transform child in content.transform)
        {
            TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                missions.Add(textComponent);
            }
            else
            {
                Debug.LogWarning($"子物体 {child.name} 没有挂载TextMeshProUGUI组件，已跳过", child);
            }
        }

        if (missions.Count == 0)
        {
            Debug.LogWarning("Content下未找到任何TextMeshProUGUI组件，请检查UI结构", this);
        }
    }

    /// <summary>
    /// 更新任务显示：根据当前天数加载对应任务并设置样式
    /// </summary>
    private void UpdateMissionDisplay()
    {
        try
        {
            // 1. 基础空值校验
            if (missionList == null || missionList.Count == 0)
            {
                Debug.LogError("任务配置列表（missionList）未赋值或为空，请在Inspector中添加Mission配置文件", this);
                return;
            }

            // 2. 校验当前天数是否有效
            int currentDayIndex = GlobalData.Day - 1;
            if (currentDayIndex < 0 || currentDayIndex >= missionList.Count)
            {
                Debug.LogError($"当前天数 {GlobalData.Day} 超出任务配置范围（配置数量：{missionList.Count}）", this);
                return;
            }

            // 3. 获取当天的任务配置
            Mission tempMission = missionList[currentDayIndex];
            if (tempMission == null || tempMission.missions == null || tempMission.missions.Count == 0)
            {
                Debug.LogWarning($"第 {GlobalData.Day} 天没有配置任务", this);
                return;
            }

            // 4. 遍历任务并更新显示（避免数组越界）
            for (int i = 0; i < missions.Count && i < tempMission.missions.Count; i++)
            {
                // 校验文本组件是否为空
                if (missions[i] == null)
                {
                    Debug.LogWarning($"第 {i+1} 个任务文本组件为空", this);
                    continue;
                }

                // 设置任务名称
                missions[i].text = tempMission.missions[i].name;
                // 设置样式：已完成显示删除线，未完成正常显示
                missions[i].fontStyle = tempMission.missions[i].isComplete ? FontStyles.Strikethrough : FontStyles.Normal;
                missions[i].color = tempMission.missions[i].isComplete ? Color.gray : Color.black;
            }

            // 5. 处理任务数量不匹配的情况（文本组件多于任务数）
            if (missions.Count > tempMission.missions.Count)
            {
                for (int i = tempMission.missions.Count; i < missions.Count; i++)
                {
                    missions[i].text = string.Empty; // 清空多余的文本
                }
                Debug.LogWarning($"任务文本组件数量（{missions.Count}）多于当天任务数（{tempMission.missions.Count}），已清空多余文本", this);
            }

        }
        catch (Exception e)
        {
            Debug.LogError($"更新任务显示时出错：{e.Message}", this);
        }
    }
}

