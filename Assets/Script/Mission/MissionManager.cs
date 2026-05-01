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
    [SerializeField]
    private Animator anim;
    // 每日任务配置列表（按天数顺序）
    public List<Mission> missionList;
    public MiniGameIntroManager miniGameIntroManager;
    private void Awake()
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
        int curIndex = 0;
        foreach (var value in missions)
        {
            value.text = string.Empty;
        }
        foreach (var value in miniGameIntroManager.miniGameData)
        {
            if (value.infos[GlobalData.Day].canPlay)
            {
                missions[curIndex].text = "帮助" + value.name;
                curIndex++;
            }
        }
        
    }
}

