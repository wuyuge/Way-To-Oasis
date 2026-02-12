using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 行走触发对话系统（Day1-3）
/// 功能：角色行走时根据天气、角色存活状态随机触发对话，支持单条/连续对话
/// </summary>
public class WalkingTalk : MonoBehaviour
{
    [Header("对话配置")]
    [Tooltip("1-3天的对话列表")]
    public List<TalkList> talkListD13;
    
    [Header("系统引用")]
    [Tooltip("天气系统")]
    public RandomRain rain;
    [Tooltip("角色状态管理器")]
    public MiniCharacterManager state;
    [Tooltip("对话展示系统")]
    public MiniCharacterTalkSys talkSys;
    
    [Header("时间配置")]
    [Tooltip("对话触发间隔（秒）")]
    public float waitTime = 5f;
    [Tooltip("单条对话展示时长（秒）")]
    public float textWaitTime = 3f;
    
    [Header("角色引用")]
    [Tooltip("Aimi角色（需关联存活状态）")]
    public Character aimi;
    [Tooltip("Amande角色（需关联存活状态）")]
    public Character amande;
    [Tooltip("Bo角色（需关联存活状态）")]
    public Character bo;
    [Tooltip("Lai角色（需关联存活状态）")]
    public Character lai;
    [Tooltip("Luo角色（需关联存活状态）")]
    public Character luo;

    // 私有状态变量
    private bool _isTalking; // 是否正在展示对话
    private Manager _showingManager; // 当前选中的文本容器
    private int _currentIndex; // 当前选中的对话组索引
    private string _currentText; // 当前要展示的文本
    private int _continuousLine; // 连续对话当前行索引
    private Coroutine _continuousTalkCoroutine; // 连续对话协程引用（防止重复开启）

    [System.Serializable]
    public class TalkList
    {
        [Tooltip("文本容器（存储具体对话行）")]
        public Manager textContainer;
        [Tooltip("是否仅雨天触发")]
        public bool isRaining;
        [Tooltip("是否为连续对话（逐行展示）")]
        public bool isContinuous;
        [Tooltip("该对话需要存活的角色")]
        public List<SpeakerType> speaker;
        
        /// <summary>
        /// 说话者类型枚举
        /// </summary>
        public enum SpeakerType
        {
            Aimi, Amande, Bo, Luo, Lai, All
        }
    }

    private void Start()
    {
        // 空引用检查（关键组件）
        ValidateReferences();
        
        // 启动对话检测协程
        StartCoroutine(DialogueDetectionCoroutine());
    }

    /// <summary>
    /// 对话检测协程（核心逻辑）
    /// </summary>
    private IEnumerator DialogueDetectionCoroutine()
    {
        while (true)
        {
            // 正在对话/角色未行走 → 等待
            if (_isTalking || (state != null && !state.isWalking))
            {
                yield return new WaitForSeconds(waitTime);
                continue;
            }

            // 步骤1：选择有效的对话组
            if (!SelectValidTalkGroup())
            {
                yield return new WaitForSeconds(waitTime);
                continue;
            }

            // 步骤2：选择具体的对话文本
            SelectDialogueText();

            // 步骤3：发送文本到展示系统
            SendDialogueText();

            // 等待下一次检测
            yield return new WaitForSeconds(waitTime);
        }
    }

    /// <summary>
    /// 选择有效的对话组（按天气、天数、角色存活状态筛选）
    /// </summary>
    /// <returns>是否选择成功</returns>
    private bool SelectValidTalkGroup()
    {
        _showingManager = null;
        _currentIndex = -1;

        // 无对话组 → 直接返回
        if (talkListD13 == null || talkListD13.Count == 0)
        {
            Debug.LogWarning("对话列表为空，无法选择对话组", this);
            return false;
        }

        // 过滤：仅保留Day≤3且角色存活的对话组
        List<TalkList> baseValidGroups = talkListD13.Where(talkGroup => 
            GlobalData.Day <= 3 &&
            talkGroup.textContainer != null && 
            talkGroup.textContainer.TxtLine != null && 
            talkGroup.textContainer.TxtLine.Count > 0 &&
            IsSpeakerAlive(talkGroup.speaker)
        ).ToList();

        // 无基础有效对话组 → 返回
        if (baseValidGroups.Count == 0)
        {
            Debug.LogWarning("无有效对话组（角色死亡/无文本/天数超限）", this);
            return false;
        }

        // 天气筛选逻辑
        bool isCurrentRaining = rain != null && rain.isRaining;
        List<TalkList> finalValidGroups = new List<TalkList>();

        if (isCurrentRaining)
        {
            // 雨天逻辑：70%概率选雨天专属对话，30%选非雨天对话
            int randomValue = Random.Range(1, 101);
            if (randomValue <= 70)
            {
                // 筛选雨天专属对话
                finalValidGroups = baseValidGroups.Where(t => t.isRaining).ToList();
                // 如果没有雨天专属对话，降级到非雨天对话
                if (finalValidGroups.Count == 0)
                {
                    finalValidGroups = baseValidGroups.Where(t => !t.isRaining).ToList();
                }
            }
            else
            {
                // 30%概率选非雨天对话
                finalValidGroups = baseValidGroups.Where(t => !t.isRaining).ToList();
            }
        }
        else
        {
            // 非雨天：只选非雨天对话
            finalValidGroups = baseValidGroups.Where(t => !t.isRaining).ToList();
        }

        // 最终无有效对话 → 返回
        if (finalValidGroups.Count == 0)
        {
            Debug.LogWarning("无符合天气条件的有效对话组", this);
            return false;
        }

        // 随机选择最终有效对话组
        var selected = finalValidGroups[Random.Range(0, finalValidGroups.Count)];
        _currentIndex = talkListD13.IndexOf(selected);
        _showingManager = selected.textContainer;
        return true;
    }

    /// <summary>
    /// 选择具体的对话文本（单条/连续）
    /// </summary>
    private void SelectDialogueText()
    {
        if (_showingManager == null || _currentIndex == -1 || _currentIndex >= talkListD13.Count)
        {
            _currentText = null;
            return;
        }

        TalkList currentTalkGroup = talkListD13[_currentIndex];
        
        // 非连续对话：随机选一条
        if (!currentTalkGroup.isContinuous)
        {
            int randomLine = Random.Range(0, _showingManager.TxtLine.Count);
            _currentText = _showingManager.TxtLine[randomLine];
        }
        // 连续对话：选第一行（后续逐行播放）
        else
        {
            _currentText = _showingManager.TxtLine.Count > 0 ? _showingManager.TxtLine[0] : null;
        }
    }

    /// <summary>
    /// 发送对话文本到展示系统
    /// </summary>
    private void SendDialogueText()
    {
        if (string.IsNullOrEmpty(_currentText) || _currentIndex == -1 || _currentIndex >= talkListD13.Count)
        {
            return;
        }

        TalkList currentTalkGroup = talkListD13[_currentIndex];
        
        // 连续对话：启动协程逐行发送
        if (currentTalkGroup.isContinuous)
        {
            _continuousLine = 0;
            // 停止旧协程，防止重复开启
            if (_continuousTalkCoroutine != null)
            {
                StopCoroutine(_continuousTalkCoroutine);
            }
            _continuousTalkCoroutine = StartCoroutine(ContinuousDialogueCoroutine());
            return;
        }

        // 单条对话：解析并展示
        (string speakerName, string content) = ParseDialogueText(_currentText);
        if (string.IsNullOrEmpty(content))
        {
            Debug.LogWarning($"对话文本解析失败：{_currentText}", this);
            return;
        }

        _isTalking = true;
        talkSys?.ShowText(speakerName, content);
        Invoke(nameof(ResetTalkState), textWaitTime);
    }

    /// <summary>
    /// 连续对话协程（逐行展示）
    /// </summary>
    private IEnumerator ContinuousDialogueCoroutine()
    {
        while (true)
        {
            // 索引越界检查
            if (_continuousLine >= _showingManager.TxtLine.Count)
            {
                ResetTalkState();
                _continuousTalkCoroutine = null; // 清空协程引用
                yield break;
            }

            // 解析当前行文本
            string currentLine = _showingManager.TxtLine[_continuousLine];
            (string speakerName, string content) = ParseDialogueText(currentLine);
            if (string.IsNullOrEmpty(content))
            {
                Debug.LogWarning($"连续对话解析失败：{currentLine}", this);
                _continuousLine++;
                yield return new WaitForSeconds(textWaitTime);
                continue;
            }

            // 展示文本
            _isTalking = true;
            talkSys?.ShowText(speakerName, content);
            
            // 递增行索引并等待
            _continuousLine++;
            yield return new WaitForSeconds(textWaitTime);
        }
    }

    /// <summary>
    /// 解析对话文本（格式：角色名：内容）
    /// </summary>
    /// <param name="text">原始文本</param>
    /// <returns>（说话者名，内容）</returns>
    private (string, string) ParseDialogueText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return ("未知", string.Empty);
        }

        int colonIndex = text.IndexOf('：');
        // 无冒号 → 格式错误
        if (colonIndex <= 0 || colonIndex >= text.Length - 1)
        {
            Debug.LogWarning($"对话文本格式错误（缺少中文冒号）：{text}", this);
            return ("未知", text);
        }

        // 解析说话者和内容
        string speakerName = text.Substring(0, colonIndex).Trim();
        string content = text.Substring(colonIndex + 1).Trim();
        return (speakerName, content);
    }

    /// <summary>
    /// 校验说话者是否存活
    /// </summary>
    /// <param name="speakers">需要存活的角色列表</param>
    /// <returns>是否全部存活</returns>
    private bool IsSpeakerAlive(List<TalkList.SpeakerType> speakers)
    {
        if (speakers == null || speakers.Count == 0)
        {
            return true; // 无角色要求 → 默认存活
        }

        foreach (var speaker in speakers)
        {
            switch (speaker)
            {
                case TalkList.SpeakerType.Aimi:
                    if (aimi == null || aimi.Dead) return false;
                    break;
                case TalkList.SpeakerType.Amande:
                    if (amande == null || amande.Dead) return false;
                    break;
                case TalkList.SpeakerType.Bo:
                    if (bo == null || bo.Dead) return false;
                    break;
                case TalkList.SpeakerType.Lai:
                    if (lai == null || lai.Dead) return false;
                    break;
                case TalkList.SpeakerType.Luo:
                    if (luo == null || luo.Dead) return false;
                    break;
                case TalkList.SpeakerType.All:
                    if (aimi == null || aimi.Dead ||
                        amande == null || amande.Dead ||
                        bo == null || bo.Dead ||
                        lai == null || lai.Dead ||
                        luo == null || luo.Dead)
                    {
                        return false;
                    }
                    break;
            }
        }

        return true;
    }

    /// <summary>
    /// 重置对话状态
    /// </summary>
    private void ResetTalkState()
    {
        _isTalking = false;
    }

    /// <summary>
    /// 校验外部引用（防止空指针）
    /// </summary>
    private void ValidateReferences()
    {
        if (rain == null) Debug.LogError("未赋值 RandomRain 组件！", this);
        if (state == null) Debug.LogError("未赋值 MiniCharacterManager 组件！", this);
        if (talkSys == null) Debug.LogError("未赋值 MiniCharacterTalkSys 组件！", this);
        
        // 角色引用警告（非致命，仅提示）
        if (aimi == null) Debug.LogWarning("未赋值 Aimi 角色引用！", this);
        if (amande == null) Debug.LogWarning("未赋值 Amande 角色引用！", this);
        if (bo == null) Debug.LogWarning("未赋值 Bo 角色引用！", this);
        if (lai == null) Debug.LogWarning("未赋值 Lai 角色引用！", this);
        if (luo == null) Debug.LogWarning("未赋值 Luo 角色引用！", this);
    }

    /// <summary>
    /// 编辑器调试：清空对话状态
    /// </summary>
    [ContextMenu("清空对话状态")]
    private void ClearDialogueState()
    {
        _isTalking = false;
        _continuousLine = 0;
        if (_continuousTalkCoroutine != null)
        {
            StopCoroutine(_continuousTalkCoroutine);
            _continuousTalkCoroutine = null;
        }
    }
}