using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoSpawner : MonoBehaviour
{
    public RectTransform leftBound;
    public RectTransform rightBound;
    public RectTransform lockPosition;
    public RectTransform piece;
    private bool complete = false;
    public GameObject full;

    private void Start()
    {
        // 获取两个边界的锚点本地坐标
        Vector2 leftAnchored = leftBound.anchoredPosition;
        Vector2 rightAnchored = rightBound.anchoredPosition;

        // 计算随机上下限（自动处理谁左谁右、谁上谁下）
        float minX = Mathf.Min(leftAnchored.x, rightAnchored.x);
        float maxX = Mathf.Max(leftAnchored.x, rightAnchored.x);
        float minY = Mathf.Min(leftAnchored.y, rightAnchored.y);
        float maxY = Mathf.Max(leftAnchored.y, rightAnchored.y);

        // 遍历所有直接子物体
        foreach (Transform child in transform)
        {
            RectTransform rt = child.GetComponent<RectTransform>();
            if (rt == null) continue;
            
            rt.anchoredPosition = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );
        }
    }

    private void OnEnable()
    {
        complete = false;
        BoGlobalData.Complete = false;
    }

    private void Update()
    {
        if (transform.childCount == 1 && !BoGlobalData.Complete)
        {
            BoGlobalData.Complete = true;
            BoGlobalData.Button.SetActive(true);
            BoGlobalData.TalkSys.SetComplete();
            complete = true;
            Debug.Log("成功");
            full.SetActive(true);
            gameObject.SetActive(false);
        }

        if (complete)
        {
            piece.anchoredPosition = lockPosition.anchoredPosition;
        }
    }
}