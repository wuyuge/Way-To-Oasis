using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LuoTalkSys : MonoBehaviour
{
    public int line;
    private Image _image;
    private RectTransform _rect;
    public TextMeshProUGUI text;
    private bool _showingStart;
    public GameObject mask;
    public Manager language;

    [Header("表情")]
    public Sprite calm;
    public Sprite dislike;
    public Sprite nervous;

    [Header("对话数据")]
    public LuoTalkData start;
    public LuoTalkData breakData;
    public LuoTalkData click;
    public LuoTalkData fail;
    public LuoTalkData mistake;
    public LuoTalkData heavyMistake;
    public LuoTalkData repeat;
    public LuoTalkData success;
    public LuoTalkData timeOut;

    private float _lastTime;
    [Header("超时")]
    public float timeOutTime;

    private bool _breakCd;
    private bool _mistakeCd;

    [Tooltip("有分支的文本段显示间隔")]
    public float interval;

    public bool showing;

    private bool _fail;
    public AudioSource tap;
    public bool canEnd;

    [Header("打字机效果")]
    [Tooltip("每个字之间的间隔（秒）")]
    public float typeInterval = 0.05f;

    private Coroutine _typeCoroutine;
    private string _currentFullText;

    // 当前是否正在逐字输出
    public bool IsTyping => _typeCoroutine != null;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _rect = GetComponent<RectTransform>();
        LuoGlobalData.TalkSys = this;
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

    public void ShowText(string s)
    {
        if (_fail)
        {
            return;
        }
        PlayText(s);
    }

    private void OnEnable()
    {
        line = 0;
        canEnd = false;
        if (!start.showed)
        {
            _showingStart = true;
            ShowText(GetText(start, line));
            Debug.Log("起始对话");
            mask.SetActive(true);
            start.showed = true;
        }
    }

    private void Update()
    {
        if (canEnd && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            SetEnd();
        }

        if (_showingStart && Input.GetKeyDown(KeyCode.Mouse0))
        {
            // 正在逐字输出时，点击先补全当前句子，不进入下一句
            if (IsTyping)
            {
                SkipCurrentTyping();
                return;
            }
            line++;
            if (line > start.talkDatas.Count - 1)
            {
                _showingStart = false;
                mask.SetActive(false);
                return;
            }
            ShowText(GetText(start, line));
        }

        if (Input.anyKey)
        {
            _lastTime = 0;
        }
        _lastTime += Time.deltaTime;
        if (_lastTime > timeOutTime)
        {
            ShowText(GetText(timeOut, Random.Range(0, timeOut.talkDatas.Count)));
            _lastTime = 0;
        }
    }

    private string GetText(LuoTalkData data, int l)
    {
        SetExpress(data.talkDatas[l].express);
        if (!language.isEn)
        {
            return data.talkDatas[l].cn;
        }
        return data.talkDatas[l].en;
    }

    private void SetExpress(LuoTalkData.Express express)
    {
        switch (express)
        {
            case LuoTalkData.Express.平静:
                _image.sprite = calm;
                break;
            case LuoTalkData.Express.嫌弃:
                _image.sprite = dislike;
                break;
            case LuoTalkData.Express.紧张:
                _image.sprite = nervous;
                break;
        }
        _image.SetNativeSize();
        _rect.sizeDelta = new Vector2(_rect.sizeDelta.x * 0.125f, _rect.sizeDelta.y * 0.125f);
    }

    public void Click()
    {
        if (_showingStart || showing)
        {
            return;
        }

        var index = Random.Range(0, click.talkDatas.Count);
        ShowText(GetText(click, index));
        if (click.talkDatas[index].cnHaveBranch && !language.isEn)
        {
            StartCoroutine(IntervalShowText(click.talkDatas[index].cnBranch[0]));
        }

        if (click.talkDatas[index].enHaveBranch && language.isEn)
        {
            StartCoroutine(IntervalShowText(click.talkDatas[index].enBranch[0]));
        }
    }

    private IEnumerator IntervalShowText(string s)
    {
        showing = true;
        yield return new WaitForSeconds(interval);
        ShowText(s);
        yield return new WaitForSeconds(interval);
        showing = false;
    }

    public void SetRepeat()
    {
        ShowText(GetText(repeat, Random.Range(0, repeat.talkDatas.Count)));
    }

    public void SetSuccess()
    {
        ShowText(GetText(success, Random.Range(0, success.talkDatas.Count)));
        GlobalData.ShowText?.CompleteMiniGame();
        canEnd = true;
    }

    private void SetEnd()
    {
        LuoGlobalData.LevelLoader.SetEnd();
    }

    public void SetBreak()
    {
        if (_breakCd)
        {
            return;
        }

        ShowText(GetText(breakData, Random.Range(0, breakData.talkDatas.Count)));
        _breakCd = true;
        Invoke(nameof(SetBreakCd), 5);
    }

    private void SetBreakCd()
    {
        _breakCd = false;
    }

    public void SetFail()
    {
        ShowText(GetText(fail, Random.Range(0, fail.talkDatas.Count)));
        _fail = true;
        Invoke(nameof(SetEnd), 1.25f);
    }

    public void CheckMistake(int v)
    {
        if (_mistakeCd)
        {
            return;
        }
        if (v < LuoGlobalData.MaxCorrect / 2)
        {
            ShowText(GetText(heavyMistake, Random.Range(0, heavyMistake.talkDatas.Count)));
        }
        else if (v < LuoGlobalData.MaxCorrect)
        {
            ShowText(GetText(mistake, Random.Range(0, mistake.talkDatas.Count)));
        }

        _mistakeCd = true;
        Invoke(nameof(SetMistakeCd), 5);
    }

    private void SetMistakeCd()
    {
        _mistakeCd = false;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _typeCoroutine = null;
    }
}