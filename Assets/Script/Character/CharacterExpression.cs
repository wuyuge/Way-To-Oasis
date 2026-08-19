using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色表情控制类，用于高效管理和切换不同角色的表情图片
/// </summary>
public class CharacterExpression : MonoBehaviour
{
    /// <summary>
    /// 表情数据结构，存储表情名称和对应的图片
    /// </summary>
    [Serializable]
    public class Expression
    {
        public string name;      // 表情名称（如：开心、生气、惊讶等）
        public Sprite sprite;    // 表情对应的精灵图片（修正命名：单个表情用单数）
    }

    /// <summary>
    /// 角色表情容器，关联特定角色的所有表情和显示图片的UI组件
    /// </summary>
    [Serializable]
    public class CharacterExpressionContainer
    {
        public string characterName;          // 角色名称（用于标识不同角色，修正命名）
        public List<Image> relatedImages;     // 关联的UI图片组件列表（修正命名）
        public List<Expression> expressions;  // 该角色拥有的所有表情列表（修正命名）

        // 缓存表情字典（优化查找效率）
        [NonSerialized] public Dictionary<string, Sprite> expressionDict;
    }

    [Header("角色表情配置")]
    public List<CharacterExpressionContainer> ExpressionContainers = new List<CharacterExpressionContainer>();

    // 缓存角色容器字典（优化查找效率）
    private Dictionary<string, CharacterExpressionContainer> _characterDict;

    private void Awake()
    {
        InitializeDictionaries();
    }

    /// <summary>
    /// 初始化字典缓存，将列表转换为字典以提高查找效率
    /// </summary>
    private void InitializeDictionaries()
    {
        _characterDict = new Dictionary<string, CharacterExpressionContainer>();

        foreach (var container in ExpressionContainers)
        {
            // 跳过空配置或名称为空的容器
            if (container == null || string.IsNullOrWhiteSpace(container.characterName))
            {
                Debug.LogWarning("存在配置无效的角色表情容器（空引用或无名称）");
                continue;
            }

            // 初始化角色内表情字典
            container.expressionDict = new Dictionary<string, Sprite>();
            foreach (var expr in container.expressions)
            {
                if (expr == null || string.IsNullOrWhiteSpace(expr.name) || expr.sprite == null)
                {
                    Debug.LogWarning($"角色 {container.characterName} 存在无效表情配置（空引用/无名称/无图片）");
                    continue;
                }

                // 统一转为小写存储，避免大小写问题
                string exprKey = expr.name.ToLower();
                if (container.expressionDict.ContainsKey(exprKey))
                {
                    Debug.LogWarning($"角色 {container.characterName} 存在重复表情名称：{expr.name}，已忽略重复项");
                    continue;
                }
                container.expressionDict[exprKey] = expr.sprite;
            }

            // 添加到角色字典（统一小写键）
            string charaKey = container.characterName.ToLower();
            if (_characterDict.ContainsKey(charaKey))
            {
                Debug.LogWarning($"存在重复角色名称：{container.characterName}，已忽略重复项");
                continue;
            }
            _characterDict[charaKey] = container;
        }
    }

    /// <summary>
    /// 设置指定角色的表情图片
    /// </summary>
    /// <param name="characterName">角色名称（不区分大小写）</param>
    /// <param name="expressionName">表情名称（不区分大小写）</param>
    /// <returns>是否设置成功</returns>
    public bool SetExpression(string characterName, string expressionName)
    {
        // 空值检查
        if (string.IsNullOrWhiteSpace(characterName))
        {
            Debug.LogError("角色名称不能为空");
            return false;
        }
        if (string.IsNullOrWhiteSpace(expressionName))
        {
            Debug.LogError("表情名称不能为空");
            return false;
        }

        // 统一转为小写键查找（解决大小写问题）
        string charaKey = characterName;
        string exprKey = expressionName.ToLower();

        // 查找角色容器
        if (!_characterDict.TryGetValue(charaKey, out var container))
        {
            Debug.LogError($"未找到角色配置：{characterName}");
            return false;
        }

        // 查找表情图片
        if (!container.expressionDict.TryGetValue(exprKey, out var targetSprite))
        {
            Debug.LogError($"角色 {characterName} 未找到表情：{expressionName}");
            return false;
        }

        // 应用表情到所有关联图片（跳过空引用）
        int nullCount = 0;
        foreach (var image in container.relatedImages)
        {
            if (image != null)
            {
                image.sprite = targetSprite;
            }
            else
            {
                nullCount++;
            }
        }

        // 提示空引用图片数量
        if (nullCount > 0)
        {
            Debug.LogWarning($"角色 {characterName} 有 {nullCount} 个关联图片组件为空引用");
        }

        return true;
    }

    public void SetExpression(string name,int index)
    {
        foreach (var container in ExpressionContainers)
        {
            if (container.characterName == name)
            {
                foreach (var image in container.relatedImages)
                {
                    image.sprite = container.expressions[index - 1].sprite;
                }
            }
        }
    }

    /// <summary>
    /// 重新初始化配置（用于动态修改配置后刷新）
    /// </summary>
    public void RefreshConfiguration()
    {
        InitializeDictionaries();
        
    }
}