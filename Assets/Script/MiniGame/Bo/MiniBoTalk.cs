using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniBoTalk : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public int curLine;
    public BoItemData curData;
    public Manager language;
    public Animator anim;
    public Manager start, complete;
    public bool _complete;
    public Image bo;
    public List<Sprite> expressions;
    public AudioSource tap;

    [Header("打字机效果")]
    [Tooltip("每个字之间的间隔（秒）")]
    public float typeInterval = 0.05f;

    private Coroutine _typeCoroutine;
    private string _currentFullText;

    // 当前是否正在逐字输出
    public bool IsTyping => _typeCoroutine != null;

    private void Awake()
    {
        BoGlobalData.TalkSys = this;
    }

    private void OnEnable()
    {
        if (!start.GeneralBool)
        {
            curLine = 0;
            PlayText(language.isEn ? start.data[curLine]?.en : start.data[curLine]?.cn);
            bo.sprite = expressions[start.data[curLine].expression];
            bo.SetNativeSize();
            curLine++;
        }
        _complete = false;
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
        textUI.text = _currentFullText;
    }

    private IEnumerator TypeText(string content)
    {
        textUI.text = string.Empty;
        foreach (char c in content)
        {
            textUI.text += c;
            tap.Play();
            yield return new WaitForSeconds(typeInterval);
        }
        textUI.text = content;
        _typeCoroutine = null;
    }

    public void Update()
    {
        // 没有对话数据直接退出
        if (curData == null && start.GeneralBool && !_complete) return;

        // 空格 或 鼠标左键点击
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            // 正在逐字输出时，点击先补全当前句子，不进入下一句
            if (IsTyping)
            {
                SkipCurrentTyping();
                return;
            }

            if (!start.GeneralBool)
            {
                Debug.Log("初始对话");
                PlayText(language.isEn ? start.data[curLine]?.en : start.data[curLine]?.cn);
                bo.sprite = expressions[start.data[curLine].expression];
                bo.SetNativeSize();
                curLine++;
                if (curLine > start.data.Count - 1)
                {
                    start.GeneralBool = true;
                }
                return;
            }

            if (_complete)
            {
                PlayText(language.isEn ? complete.data[curLine]?.en : complete.data[curLine]?.cn);
                bo.sprite = expressions[complete.data[curLine].expression];
                bo.SetNativeSize();
                curLine++;
                if (curLine > complete.data.Count - 1)
                {
                    _complete = false;
                    curData = null;
                }
                return;
            }

            // 防止索引越界
            if (curLine >= curData.data.Count)
            {
                curLine = 0;
                return;
            }

            // 获取对应语言文本
            var lineData = curData.data[curLine];
            bo.sprite = expressions[curData.data[curLine].expression];
            bo.SetNativeSize();
            string showStr = language.isEn ? lineData?.en : lineData?.cn;
            ShowText(showStr);
        }
    }

    private void ShowText(string text)
    {
        if (!start.GeneralBool || _complete)
        {
            return;
        }
        PlayText(text);
        curLine++;
    }

    public void StopGame()
    {
        BoGlobalData.anim.Hide();
        anim.SetTrigger("End");
    }

    public void SetComplete()
    {
        _complete = true;
        curLine = 0;
        PlayText(language.isEn ? complete.data[curLine]?.en : complete.data[curLine]?.cn);
        bo.sprite = expressions[complete.data[curLine].expression];
        bo.SetNativeSize();
        curLine++;
    }

    /// <summary>
    /// 外部调用：开启一段新对话，重置行数
    /// </summary>
    public void StartTalk(BoItemData data)
    {
        if (_complete)
        {
            return;
        }
        curData = data;
        curLine = 0;
        if (data != null && curData.data.Count > 0)
        {
            string showStr = language.isEn ? curData.data[0]?.en : curData.data[0]?.cn;
            bo.sprite = expressions[curData.data[curLine].expression];
            bo.SetNativeSize();
            ShowText(showStr);
        }
    }

    /// <summary>
    /// 清空对话
    /// </summary>
    public void ClearTalk()
    {
        if (_typeCoroutine != null)
        {
            StopCoroutine(_typeCoroutine);
            _typeCoroutine = null;
        }
        curData = null;
        curLine = 0;
        textUI.text = "";
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _typeCoroutine = null;
    }
}