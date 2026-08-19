using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MiniAmandeTalk : MonoBehaviour
{
    // 显示文本的UI组件
    [SerializeField]
    private TextMeshProUGUI text;
    public Manager start, fail, heavyFail, explain, success;
    public bool startTalk, showExplain, awakeInit;
    public int waitTime;
    private int _failState;// 1为丢弃药品次数小于3次，2为大于3次
    [SerializeField]
    private int _curLine;
    public Animator canvasAnim;
    private Manager _curText;
    public AmandeMission missionManager;
    private bool _creatingMedicine;
    private int _createTime;
    public Transform medicineContainer;
    public List<Animator> medicineAnim = new List<Animator>();
    private bool _canEnd;
    public Manager language;
    [Header("表情")] public List<Sprite> expressions;
    public Image amandeImage;
    public AudioSource tap;
    public TextMeshProUGUI targetText;
    public string initialTextCn;
    public string initialTextEn;

    [Header("打字机效果")]
    [Tooltip("每个字之间的间隔（秒）")]
    public float typeInterval = 0.05f;

    private Coroutine _typeCoroutine;
    private string _currentFullText;

    // 当前是否正在逐字输出
    public bool IsTyping => _typeCoroutine != null;

    private void Awake()
    {
        if (awakeInit)
        {
            start.GeneralBool = false;
            explain.GeneralBool = false;
        }
    }

    private void Start()
    {
        if (!start.GeneralBool)
        {
            startTalk = true;
        }
        StartCoroutine(Check());
    }

    private void OnEnable()
    {
        foreach (var value in missionManager.missions)
        {
            value.composed = false;
        }

        targetText.text = language.isEn ? initialTextEn.Replace("{Name}", "") : initialTextCn.Replace("{Name}", "");
        _createTime = 0;

        AmandeGlobal.Mission.Clear();
        foreach (var value in missionManager.missions)
        {
            AmandeGlobal.Mission.Add(value);
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
        while (true)
        {
            if (startTalk && !start.GeneralBool)
            {
                start.GeneralBool = true;
                StartCoroutine(ShowText());
                yield break;
            }

            if (!showExplain)
            {
                StartCoroutine(ShowText());
                showExplain = true;
            }

            if (_failState != 0)
            {
                switch (_failState)
                {
                    case 1:
                        _curText = fail;
                        break;
                    case 2:
                        _curText = heavyFail;
                        break;
                }
                ShowFailText();
                _failState = 0;
            }
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void ShowFailText()
    {
        Debug.Log("失败文本");
        var index = Random.Range(0, _curText.data.Count);
        var s = language.isEn ? _curText.data[index].en : _curText.data[index].cn;
        PlayText(s);
        SetExpression(_curText.data[index].expression);
    }

    private IEnumerator ShowText()
    {
        if (_creatingMedicine)
        {
            if (CheckMedicine())
            {
                _creatingMedicine = false;
            }
            else
            {
                yield break;
            }
        }

        if (startTalk)
        {
            SetExpression(start.data[_curLine].expression);
            PlayText(language.isEn ? start.data[_curLine].en : start.data[_curLine].cn);
            _curLine++;
            if (_curLine > start.data.Count - 1)
            {
                _curLine = 0;
                startTalk = false;
                StartCoroutine(Check());
            }
            yield break;
        }

        if (!explain.GeneralBool)
        {
            SetExpression(explain.data[_curLine].expression);
            var displayText = language.isEn ? explain.data[_curLine].en : explain.data[_curLine].cn;

            #region 判断洛尔坎是否死亡跳转
            if (displayText == "${CheckLuoAlive}")
            {
                if (GlobalData.TalkSystem != null)
                {
                    if (GlobalData.TalkSystem.characterComponentList[2].Dead)
                    {
                        explain = explain.Option1;
                    }
                    else
                    {
                        explain = explain.Option2;
                    }
                }
                else
                {
                    explain = explain.Option2;
                }
                explain.GeneralBool = false;
                _curLine = 0;
                SetExpression(explain.data[_curLine].expression);
                displayText = language.isEn ? explain.data[_curLine].en : explain.data[_curLine].cn;
                PlayText(displayText);
                _curLine++;
                yield break;
            }

            if (displayText == "${Create}")
            {
                var replaceText = language.isEn
                    ? missionManager.missions[_createTime].targetEn
                    : missionManager.missions[_createTime].targetCn;
                targetText.text = language.isEn ? initialTextEn : initialTextCn;
                targetText.text = targetText.text.Replace("{Name}", replaceText);
                _creatingMedicine = true;
                _curLine++;
                _createTime++;
                yield break;
            }

            if (displayText == "${Last}")
            {
                var replaceText = language.isEn
                    ? missionManager.missions[_createTime].targetEn
                    : missionManager.missions[_createTime].targetCn;
                targetText.text = language.isEn ? initialTextEn : initialTextCn;
                targetText.text = targetText.text.Replace("{Name}", replaceText);
                _canEnd = true;
                _curLine++;
                _createTime++;
                yield break;
            }

            #endregion

            PlayText(displayText);
            _curLine++;
            if (_curLine > explain.TxtLine.Count - 1)
            {
                _curLine = 0;
                explain.GeneralBool = true;
            }
            yield break;
        }

        var index = Random.Range(0, _curText.TxtLine.Count);
        SetExpression(index);
        PlayText(language.isEn ? _curText.data[index].en : _curText.data[index].cn);
    }

    private void Update()
    {
        if ((startTalk || !explain.GeneralBool) && Input.anyKeyDown)
        {
            // 正在逐字输出时，点击先补全当前句子，不进入下一句
            if (IsTyping)
            {
                SkipCurrentTyping();
                return;
            }
            StartCoroutine(ShowText());
        }
    }

    public void ShowSuccess()
    {
        var index = Random.Range(0, success.data.Count);
        SetExpression(success.data[index].expression);
        PlayText(language.isEn ? success.data[index].en : success.data[index].cn);
        if (!_canEnd)
        {
            return;
        }
        GlobalData.ShowText?.CompleteMiniGame();
        Invoke(nameof(SetEndAnim), 1.25f);
    }

    private void SetEndAnim()
    {
        canvasAnim.SetTrigger("end");
        foreach (var value in medicineAnim)
        {
            value.SetTrigger("end");
        }
    }

    private void SetExpression(int index)
    {
        amandeImage.sprite = expressions[index - 1];
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _typeCoroutine = null;
    }

    public void SetFail(int value)
    {
        _failState = value;
    }

    private bool CheckMedicine()
    {
        var tempCreated = 0;
        foreach (var value in missionManager.missions)
        {
            if (MedicineManager.ComposedMedicine.Contains(value.medicine))
            {
                value.composed = true;
                tempCreated++;
            }
        }

        if (tempCreated == _createTime)
        {
            return true;
        }
        return false;
    }
}