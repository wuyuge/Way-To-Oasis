using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AimiManager : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float waitTime;
    public Manager completeHalf, complete, timeOut, start, fail;
    public int curLine;
    private bool _complete, _failed;
    public float textInterval;
    private float wait;
    public Manager language;
    public Animator anim;
    public Image image;
    public List<Sprite> expressions;
    public AudioSource tap;

    [Header("打字机效果")]
    [Tooltip("每个字之间的间隔（秒）")]
    public float typeInterval = 0.05f;

    private Coroutine _typeCoroutine;
    private string _currentFullText;

    // 当前是否正在逐字输出
    public bool IsTyping => _typeCoroutine != null;

    void Start()
    {
        AimiGlobalManager.TalkManager = this;
    }

    private void OnEnable()
    {
        complete.GeneralBool = false;
        completeHalf.GeneralBool = false;
        _complete = false;
        curLine = 0;
        if (start.GeneralBool)
        {
            StartCoroutine(Check());
        }
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

    private IEnumerator Check()
    {
        int index = 0;
        while (true)
        {
            if (!completeHalf.GeneralBool)
            {
                if (AimiGlobalManager.CheckNums >= AimiGlobalManager.ObjectNums / 2)
                {
                    yield return new WaitForSeconds(1.5f);
                    index = Random.Range(0, completeHalf.data.Count);
                    PlayText(language.isEn ? completeHalf.data[index].en : completeHalf.data[index].cn);
                    image.sprite = expressions[completeHalf.data[index].expression];
                    image.SetNativeSize();
                    completeHalf.GeneralBool = true;
                }
            }

            if (AimiGlobalManager.CheckNums >= AimiGlobalManager.ObjectNums)
            {
                PlayText(language.isEn ? complete.data[0].en : complete.data[0].cn);
                image.sprite = expressions[complete.data[0].expression];
                image.SetNativeSize();
                curLine = 0;
                curLine++;
                _complete = true;

                yield break;
            }

            if (_failed)
            {
                yield break;
            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    public void SetEnd()
    {
        anim.SetTrigger("End");
    }

    private void Update()
    {
        if (wait > 0)
        {
            wait -= Time.deltaTime;
            if (wait <= 0)
            {
                waitTime = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            // 如果正在逐字输出，点击先把当前句子显示完整，不进入下一句
            if (IsTyping)
            {
                SkipCurrentTyping();
                return;
            }

            if (!start.GeneralBool)
            {
                PlayText(language.isEn ? start.data[curLine].en : start.data[curLine].cn);
                image.sprite = expressions[start.data[curLine].expression];
                image.SetNativeSize();
                curLine++;
                if (curLine > start.data.Count - 1)
                {
                    start.GeneralBool = true;
                    StartCoroutine(Check());
                }
            }
            else if (_complete)
            {
                PlayText(language.isEn ? complete.data[curLine].en : complete.data[curLine].cn);
                image.sprite = expressions[complete.data[curLine].expression];
                image.SetNativeSize();
                curLine++;
                // 合并原下方分支的逻辑：到倒数第二句时标记 complete 完成
                if (curLine >= complete.data.Count - 1)
                {
                    complete.GeneralBool = true;
                }
                if (curLine > complete.data.Count - 1)
                {
                    Invoke(nameof(SetEnd), 1.25f);
                }
            }
            else if (_failed)
            {
                PlayText(language.isEn ? fail.data[curLine].en : fail.data[curLine].cn);
                image.sprite = expressions[fail.data[curLine].expression];
                image.SetNativeSize();
                curLine++;
                if (curLine > fail.data.Count - 1)
                {
                    AimiGlobalManager.Failed = false;
                    Invoke(nameof(SetEnd), 1.25f);
                }
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _typeCoroutine = null;
    }

    public void SetTimeOut()
    {
        int index = Random.Range(0, timeOut.data.Count);

        PlayText(language.isEn ? timeOut.data[index].en : timeOut.data[index].cn);
        image.sprite = expressions[timeOut.data[index].expression];
        image.SetNativeSize();
    }

    public void SetText(string txt, int e)
    {
        if (wait > 0)
        {
            return;
        }
        wait = textInterval;
        PlayText(txt);
        image.sprite = expressions[e];
        image.SetNativeSize();
    }

    public void SetFail()
    {
        _failed = true;
        AimiGlobalManager.Failed = true;
        PlayText(language.isEn ? fail.data[0].en : fail.data[0].cn);
        image.sprite = expressions[fail.data[0].expression];
        image.SetNativeSize();
        curLine = 0;
        curLine++;
    }
}