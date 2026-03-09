using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MiniAmandeTalk : MonoBehaviour
{
    // 存储文本行数据的管理器引用
    private Manager _textLine;
    // 控制文本显示的协程
    private Coroutine _coroutine;
    // 显示文本的UI组件
    [SerializeField]
    private TextMeshProUGUI text;
    // 当前读取的文本行索引
    private int _readLine;
    // 每行文本的显示间隔时间（秒）
    public int waitTime = 2; // 设置默认值，避免未赋值导致的问题

    /// <summary>
    /// 初始化文本显示，重置行索引并启动协程
    /// </summary>
    /// <param name="textLine">包含文本行的管理器</param>
    public void SetText(Manager textLine)
    {
        _textLine = textLine;
        _readLine = 0;

        // 如果已有协程在运行，先停止，避免多个协程同时执行
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        // 启动新的文本显示协程
        _coroutine = StartCoroutine(ShowText());
    }
    
    public void SetText(string textLine)
    {
        text.text = textLine;
    }

    /// <summary>
    /// 逐行显示文本的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowText()
    {
        // 修复循环条件：索引小于文本行总数时执行
        while (_readLine < _textLine.TxtLine.Count)
        {
            // 显示当前行文本
            text.text = _textLine.TxtLine[_readLine];
            // 索引自增，准备下一行
            _readLine++;
            // 等待指定时间后再显示下一行
            yield return new WaitForSeconds(waitTime);
        }

        // 所有文本显示完毕，重置协程引用
        _coroutine = null;
    }
}