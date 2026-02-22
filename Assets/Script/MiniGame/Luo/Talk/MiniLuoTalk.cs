using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MiniLuoTalk : MonoBehaviour
{
    public TextMeshProUGUI text;
    [System.Serializable]
    public class Expressions
    {
        public Sprite sprite;
    }
    public List<Expressions> expressionsList;
    public Manager normal,timeOut,success,mistake,heavyMistake,repeat,start;
    public float checkTime,waitTime;
    public int timeOutInterval;
    private Image _image;
    private static Coroutine _currentCoroutine;
    private int _lastSelectedIndex = -1;
    private bool _succeed,_isStart;
    [Tooltip("设置起始对话等待时间(秒)")] public int waitSeconds;

    #region  初始化设置
    private void Awake()
    {
        _image = GetComponent<Image>();
        if (start is null)
        {
            Debug.LogError("start未赋值",this);
        }
        
        
        
    }

    private void OnEnable()
    {
        ResetTalkLine();
        if (GlobalData.Day == 1)
        {
            start.GeneralBool = true;
        }
        if (start.GeneralBool)
        {
            _isStart = true;
            StartCoroutine(ShowText());
        }
    }

    public void ResetTalkLine()
    {
        System.DateTime now = System.DateTime.Now;
        int hourMinute = now.Hour * 100 + now.Minute;
        LuoStaticData.Time = hourMinute;
        LuoStaticData.Success = false;
        LuoStaticData.RollTime = 0;
        LuoStaticData.MaxReach = 0;
        LuoStaticData.CurrentReach = 0;
        LuoStaticData.CurrentPipe = null;
        _succeed = false;
        if (_currentCoroutine is not null)
        {
            StopCoroutine(_currentCoroutine);
        }
        _currentCoroutine = StartCoroutine(CheckText());
    }

    private void OnDisable()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null; // 清空引用
        }
    }
    

    #endregion


    private IEnumerator ShowText()
    {
        foreach (var value in start.TxtLine)
        {
            string tempText = value;
            text.text = value;
            yield return new WaitForSecondsRealtime(waitSeconds);
        }

        _isStart = false;
    }
    
    

    #region 点击设置文本

    public void Click()
    {
        // 1. 校验基础数据，避免空引用/索引越界
        if (normal == null || normal.TxtLine == null || normal.TxtLine.Count == 0)
        {
            Debug.LogWarning("TxtLine列表为空，无法获取对话文本");
            text.text = "";
            return;
        }
        
        // 特殊处理：如果只有1行文本，直接使用（无法随机到不同的）
        if (normal.TxtLine.Count == 1)
        {
            ProcessTextLine(0,normal);
            return;
        }

        // 2. 随机获取文本行，确保和上一次不同
        int tempIndex;
        do
        {
            tempIndex = Random.Range(0, normal.TxtLine.Count);
            // 循环条件：新索引和上一次相同，就重新随机
        } while (tempIndex == _lastSelectedIndex);
        
        // 更新上一次选中的索引
        _lastSelectedIndex = tempIndex;

        // 3. 处理选中的文本行
        ProcessTextLine(tempIndex,normal);
    }

    #endregion

    private void FixedUpdate()
    {
        if (!_succeed)
        {
            CheckSuccess();
        }
    }

    #region 文本处理 ProcessTextLine
    private void ProcessTextLine(int index,Manager targetManager)
    {
        if (_isStart)
        {
            return;
        }
        string targetLine = targetManager.TxtLine[index];
        if (string.IsNullOrEmpty(targetLine))
        {
            Debug.LogWarning($"索引{index}对应的文本行为空");
            text.text = "";
            return;
        }

        // 按中文冒号分割，过滤空值，避免分割后元素不足
        string[] splitParts = targetLine.Split(new[] { '：' }, StringSplitOptions.RemoveEmptyEntries);
        if (splitParts.Length < 2)
        {
            Debug.LogWarning($"文本行「{targetLine}」无有效中文冒号分割，直接显示整行");
            text.text = targetLine;
            return;
        }

        // 获取分割后第二段文本，校验非空
        string tempText = splitParts[1];
        if (string.IsNullOrEmpty(tempText))
        {
            Debug.LogWarning("分割后的第二段文本为空");
            text.text = "";
            return;
        }

        // 提取最后一个字符，转换为有效索引
        char lastChar = tempText.Last();
        if (!int.TryParse(lastChar.ToString(), out int expressionIndex))
        {
            Debug.LogWarning($"最后一个字符「{lastChar}」不是数字，无法匹配表情索引");
            text.text = tempText;
            return;
        }

        // 校验表情索引是否在有效范围内
        if (expressionIndex < 0 || expressionIndex >= expressionsList.Count)
        {
            Debug.LogWarning($"表情索引「{expressionIndex}」超出范围（列表长度：{expressionsList.Count}）");
            text.text = tempText;
            return;
        }

        // 设置表情和最终文本
        SetExpression(expressionIndex);
        string finalText = tempText.Remove(tempText.Length - 1);
        text.text = finalText;
    }

    private void SetExpression(int value)
    {
        // 二次校验，避免索引越界
        if (value >= 0 && value < expressionsList.Count)
        {
            _image.sprite = expressionsList[value].sprite;
        }
        else
        {
            Debug.LogError($"表情索引{value}无效，无法设置Sprite");
        }
    }
    #endregion
    
    private IEnumerator CheckText()
    {
        while (true)
        {
            
            if (CheckTime())
            {
                yield return new WaitForSeconds(waitTime);
                continue;
            }

            if (CheckMistake())
            {
                yield return new WaitForSeconds(waitTime);
                continue;
            }
            
            if (CheckRepeat())
            {
                yield return new WaitForSeconds(waitTime);
                continue;
            }
            
            yield return new WaitForSeconds(checkTime);
        }
    }


    private bool CheckTime()
    {
        System.DateTime now = System.DateTime.Now;
        int hourMinute = now.Hour * 100 + now.Minute;

        if (hourMinute - LuoStaticData.Time > timeOutInterval)
        {
            ProcessTextLine(Random.Range(0,timeOut.TxtLine.Count),timeOut);
            LuoStaticData.Time = hourMinute;
            return true;
        }

        return false;
    }

    private void CheckSuccess()
    {
        if (LuoStaticData.Success)
        {
            ProcessTextLine(Random.Range(0,success.TxtLine.Count),success);
            StopCoroutine(_currentCoroutine);
            _succeed = true;
            return;
        }

        _succeed = false;
    }

    private bool CheckMistake()
    {
        if (LuoStaticData.MaxReach - LuoStaticData.CurrentReach <= 2 && LuoStaticData.MaxReach - LuoStaticData.CurrentReach > 0)
        {
            ProcessTextLine(Random.Range(0,mistake.TxtLine.Count),mistake);
            return true;
        }
        if (LuoStaticData.MaxReach - LuoStaticData.CurrentReach > 2)
        {
            ProcessTextLine(Random.Range(0,heavyMistake.TxtLine.Count),heavyMistake);
            return true;
        }
        return false;
    }

    public bool CheckRepeat()
    {
        if (LuoStaticData.RollTime >= 8)
        {
            ProcessTextLine(Random.Range(0,repeat.TxtLine.Count),repeat);
            return true;
        }
        return false;
    }
    
    
}

public static class LuoStaticData
{
    public static GameObject CurrentPipe { get; set; }
    public static int RollTime { get; set; }
    public static int Time { get; set; }
    public static bool Success { get; set; }
    public static int MaxReach { get; set; }
    public static int CurrentReach { get; set; }
}


