using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//教程4独立方法只用于控制打勾
public class Tutorial4 : MonoBehaviour
{
    private bool _haveSet = false;
    public GameObject c1, c2, c3;
    public Toggle food, body;
    public List<Manager> eat;

    // 只记录【历史最高权重】，不再记录上一帧
    private Dictionary<Manager, float> maxWeights = new Dictionary<Manager, float>();

    private void Update()
    {
        if (_haveSet)
        {
            enabled = false;
        }
        
        // 勾选判断
        if (food.isOn || body.isOn)
        {
            c1.SetActive(true);
        }

        // 遍历所有 Manager
        foreach (var value in eat)
        {
            // 1. 初始化最高权重
            if (!maxWeights.ContainsKey(value))
            {
                maxWeights[value] = value.Weight;
            }

            // 2. 自动更新历史最高权重（只会变大，不会变小）
            if (value.Weight > maxWeights[value])
            {
                maxWeights[value] = value.Weight;
            }

            // 3. 核心逻辑：历史最高 >1 且 当前权重 < 最高 → 打开 c3
            if (maxWeights[value] > 1 && value.Weight < maxWeights[value])
            {
                c3.SetActive(true);
                _haveSet = true;
            }

            // 原有逻辑
            if (value.Weight > 0)
            {
                c2.SetActive(true);
                
            }
        }
    }
}