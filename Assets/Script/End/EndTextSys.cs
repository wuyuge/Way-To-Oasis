using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndTextSys : MonoBehaviour
{
    [Header("UI")]
    public GameObject tips;
    public TextMeshProUGUI textUI;
    public EndMask mask;

    [Header("对话数据")]
    public Manager textLine, language;
    public float textInterval = 0.05f;

    private int _curLine = 0;
    private bool _showing;
    private string _showText = ""; // 原始带富文本标签完整文本
    public AudioSource tap;
    public Manager playerName;

    private void Start()
    {
        tips.SetActive(false);
        textUI.text = "";
        Debug.Log("启动");
    }

    private void Update()
    {
        // 左键 / 空格 确认
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
        {
            OnClickConfirm();
        }
    }

    // 外部调用：开启对话
    public void StartDialog()
    {
        _curLine = 0;
        textUI.text = "";
        tips.SetActive(true);
        NextLine();
    }

    // 点击确认主逻辑
    private void OnClickConfirm()
    {
        // 文字还在打字中：直接一次性显示全部文字（带富文本标签）
        if (_showing)
        {
            ShowAllText();
            return;
        }

        // 打字完成，读取下一行
        NextLine();
    }

    // 读取下一行文本
    private void NextLine()
    {
        // 下标越界：对话结束，关闭弹窗
        if (_curLine < 0 || _curLine >= textLine.data.Count)
        {
            Debug.Log("结束输出");
            EndDialog();
            return;
        }

        var targetItem = textLine.data[_curLine];
        // 兼容空数据，兜底空字符串
        if (language.isEn)
            _showText = targetItem?.en ?? "";
        else
            _showText = targetItem?.cn ?? "";

        _curLine++;
        StartCoroutine(ShowTextCoroutine(_showText));
    }

    // 逐字打字协程：保留富文本标签效果，同时实现打字动画
    private IEnumerator ShowTextCoroutine(string fullText)
    {
        _showing = true;
        textUI.text = "";

        StringBuilder currentText = new StringBuilder(); // 拼接当前显示的文本（含标签）
        bool inTag = false; // 是否处于富文本标签内
        StringBuilder tagBuffer = new StringBuilder(); // 缓存标签内容

        if (fullText.Contains("{PlayerName}"))
        {
            fullText = fullText.Replace("{PlayerName}",playerName.TxtLine[0]);
        }
        
        foreach (char c in fullText)
        {
            if (c == '<')
            {
                // 进入标签区间，开始缓存标签字符
                inTag = true;
                tagBuffer.Append(c);
                continue;
            }

            if (c == '>')
            {
                // 退出标签区间，将标签完整拼接，不等待间隔
                inTag = false;
                tagBuffer.Append(c);
                currentText.Append(tagBuffer.ToString());
                tagBuffer.Clear(); // 清空标签缓存
                textUI.text = currentText.ToString();
                continue;
            }

            if (inTag)
            {
                // 处于标签内，继续缓存标签字符
                tagBuffer.Append(c);
            }
            else
            {
                // 普通字符，逐字拼接并等待间隔，保留打字动画
                currentText.Append(c);
                textUI.text = currentText.ToString();
                tap.Play();
                yield return new WaitForSeconds(textInterval);
            }
        }

        _showing = false;
    }

    // 跳过打字，显示完整文本
    private void ShowAllText()
    {
        StopAllCoroutines();
        if (_showText.Contains("{PlayerName}"))
        {
            _showText = _showText.Replace("{PlayerName}",playerName.TxtLine[0]);
        }
        textUI.text = _showText;
        _showing = false;
    }

    // 结束对话
    private void EndDialog()
    {
        StopAllCoroutines();
        textUI.text = "";
        tips.SetActive(true);
        mask.Click();
    }
}