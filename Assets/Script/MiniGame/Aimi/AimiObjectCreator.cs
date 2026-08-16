using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AimiObjectCreator : MonoBehaviour
{
    public GameObject objectPrefab;
    public RectTransform originPoint;
    public float radius;
    public List<AimiData> dataList;
    [Range(0,15)]public int minCreateCount = 8;
    public List<Sprite> nums;
    public Image num1, num2;
    public GameObject skip;
    private void OnEnable()
    {
        AimiGlobalManager.ObjectNums = 0;
        AimiGlobalManager.CheckNums = 0;
        CreateObject();
        Invoke(nameof(SetSkip),90);
    }

    private void CreateObject()
    {
        var creatCount = Random.Range(8, dataList.Count);
        for (int i = 0; i < creatCount; i++)
        {
            var temp = Instantiate(objectPrefab,transform);
            temp.GetComponent<RectTransform>().anchoredPosition = GetRandomPosition();
            temp.GetComponent<AimiObject>().data = dataList[Random.Range(0, dataList.Count)];
            AimiGlobalManager.ObjectNums++;
        }
    }

    private Vector2 GetRandomPosition()
    {
        var x = Random.Range(-radius + originPoint.anchoredPosition.x, radius + originPoint.anchoredPosition.x);
        var y = Random.Range(-radius + originPoint.anchoredPosition.y, radius + originPoint.anchoredPosition.y);
        
        return new Vector2(x, y);
    }

    private void LateUpdate()
    {
        UpdateNum();
    }

    private void UpdateNum()
    {
        if (AimiGlobalManager.ObjectNums >= 10)
        {
            num1.sprite = nums[((AimiGlobalManager.ObjectNums - AimiGlobalManager.CheckNums) / 10) % 10];
        }
        num2.sprite = nums[(AimiGlobalManager.ObjectNums - AimiGlobalManager.CheckNums) % 10];
    }

    private void SetSkip()
    {
        skip.SetActive(true);
    }
    
}
