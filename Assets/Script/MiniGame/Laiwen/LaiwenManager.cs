using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LaiwenManager : MonoBehaviour
{
    public Manager startText, halfCorrectText, timeOut, complete, lackSpace, overLap;
    public TextMeshProUGUI text;
    private TimeSpan _time;
    public bool showingStart;
    public int curLine = 0;
    public int correctItem = 0;
    public float waitTime = 0;
    public bool halfCorrect, completeTag;
    public LaiwenAnimator animator;
    public Manager language;
    [Header("表情")]
    public List<Sprite> expressions;
    public Image laiwenImage;
    public AudioSource tap;
    public bool itemShowed;
    public LaiwenItem curItem;

    [Header("打字机效果")]
    [Tooltip("每个字之间的间隔（秒）")]
    public float typeInterval = 0.05f;

    private Coroutine _typeCoroutine;
    private string _currentFullText;

    // 当前是否正在逐字输出
    public bool IsTyping => _typeCoroutine != null;

    private void OnEnable()
    {
        LaiwenMiniData.LaiwenManager = this;
        _time = DateTime.Now.TimeOfDay;
        if (!startText.GeneralBool)
        {
            startText.GeneralBool = true;
            showingStart = true;
            StartCoroutine(ShowText(startText));
            return;
        }
        StartCoroutine(Check());
    }

    public void SetExpression(int index)
    {
        laiwenImage.sprite = expressions[index - 1];
    }

    /// <summary>
    /// 逐字显示文本。如果上一次还在打字，会先停掉。
    /// </summary>
    private void PlayText(string content)
    {
        _currentFullText = content;
        if (_typeCoroutine != null)
        {
            StopCoroutine(_typeCoroutine);
        }
        _typeCoroutine = StartCoroutine(TypeText(content));
    }

    /// <summary>
    /// 立即把当前正在打的文本显示完整（用于点击跳过）。
    /// </summary>
    private void SkipCurrentTyping()
    {
        if (_typeCoroutine != null)
        {
            StopCoroutine(_typeCoroutine);
            _typeCoroutine = null;
        }
        text.text = _currentFullText;
    }

    private IEnumerator TypeText(string content)
    {
        text.text = string.Empty;
        foreach (char c in content)
        {
            text.text += c;
            tap.Play();
            yield return new WaitForSeconds(typeInterval);
        }
        text.text = content;
        _typeCoroutine = null;
    }

    private IEnumerator ShowText(Manager value)
    {
        if (showingStart)
        {
            PlayText(language.isEn ? startText.data[curLine].en : startText.data[curLine].cn);
            SetExpression(startText.data[curLine].expression);
            curLine++;
            if (startText.data.Count <= curLine)
            {
                showingStart = false;
                StartCoroutine(Check());
            }
            yield break;
        }
        var line = Random.Range(0, value.data.Count);
        SetExpression(value.data[line].expression);
        var displayText = language.isEn ? value.data[line].en : value.data[line].cn;
        if (displayText.Contains("{Item}"))
        {
            displayText = displayText.Replace("{Item}", LaiwenMiniData.CurItem.data.dataName);
        }
        PlayText(displayText);
    }

    private IEnumerator Check()
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            //检测进度
            if (correctItem >= LaiwenMiniData.TotalItems / 2 && !halfCorrect)
            {
                halfCorrect = true;
                StartCoroutine(ShowText(halfCorrectText));
                yield return new WaitForSeconds(3);
            }

            //超时
            var curTime = DateTime.Now.TimeOfDay;
            var diff = TimeSpan.FromHours(Math.Abs((_time - curTime).TotalHours));
            if (diff.TotalMinutes > 2)
            {
                _time = DateTime.Now.TimeOfDay;
                StartCoroutine(ShowText(timeOut));
                yield return new WaitForSeconds(1);
            }

            //物品对话
            if (LaiwenMiniData.CurItem != null)
            {
                if (curItem != LaiwenMiniData.CurItem)
                {
                    if (LaiwenMiniData.CurItem.additionText != null)
                    {
                        StartCoroutine(ShowText(LaiwenMiniData.CurItem.additionText));
                        itemShowed = true;
                        curItem = LaiwenMiniData.CurItem;
                        yield return new WaitForSeconds(0.2f);
                    }
                }
            }

            if (LaiwenMiniData.OverLap)
            {
                LaiwenMiniData.OverLap = false;
                LaiwenMiniData.LackSpace = false;
                StartCoroutine(ShowText(overLap));
                yield return new WaitForSeconds(2f);
            }

            if (LaiwenMiniData.LackSpace)
            {
                LaiwenMiniData.LackSpace = false;
                StartCoroutine(ShowText(lackSpace));
                yield return new WaitForSeconds(2f);
            }
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void Update()
    {
        if (showingStart)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
            {
                // 正在逐字输出时，点击先补全当前句子，不进入下一句
                if (IsTyping)
                {
                    SkipCurrentTyping();
                    return;
                }
                StartCoroutine(ShowText(startText));
            }
        }

        if (correctItem >= LaiwenMiniData.TotalItems && !completeTag)//检测游戏完成条件
        {
            StopAllCoroutines();
            LaiwenMiniData.Clear();
            completeTag = true;
            StartCoroutine(ShowText(complete));
            Invoke(nameof(Complete), 0.75f);
        }
    }

    private void Complete()
    {
        animator.SetOff();
        GlobalData.ShowText?.CompleteMiniGame();
        Debug.Log("莱文小游戏完成");
    }

    public void AddCorrect()
    {
        correctItem++;
    }

    public void DecreaseCorrect()
    {
        correctItem--;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _typeCoroutine = null;
    }
}

public static class LaiwenMiniData
{
    public static int TotalItems = 0;
    public static LaiwenManager LaiwenManager { get; set; }
    public static LaiwenItem CurItem { get; set; }

    public static bool LackSpace { get; set; }
    public static bool OverLap { get; set; }

    public static void AddItems()
    {
        TotalItems++;
    }

    public static void RemoveItems()
    {
        TotalItems--;
    }

    public static void Clear()
    {
        TotalItems = 0;
    }
}