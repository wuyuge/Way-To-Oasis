using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuoItem : MonoBehaviour
{
    public LuoPipeType type;

    private void OnEnable()
    {
        StartCoroutine(DelayAddItem());
    }

    IEnumerator DelayAddItem()
    {
        // 延迟0.2秒，可自行修改数值
        yield return new WaitForSeconds(0.2f);
    
        // 延迟后执行添加逻辑
        LuoGlobalData.ItemList.Add(GetComponent<RectTransform>());
    }

    private void OnDestroy()
    {
        LuoGlobalData.ItemList.Remove(GetComponent<RectTransform>());
        LuoGlobalData.LevelLoader.aS.Play();
    }
}