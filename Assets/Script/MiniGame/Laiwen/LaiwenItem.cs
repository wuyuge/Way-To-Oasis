using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 莱文物品信息类（序列化后可在Inspector面板显示）
/// </summary>
[System.Serializable]
public class LaiwenItemInfo
{
    public LaiwenItem item;        // 物品对象
    public RectTransform position; // 碰撞位置节点
}

/// <summary>
/// 莱文可拖拽物品主脚本（继承拖拽基类 + 鼠标事件接口）
/// </summary>
public class LaiwenItem : Draggable, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler
{
    // 静态：当前显示提示的物品（全局唯一）
    private static LaiwenItem _currentTipItem;

    [Header("物品尺寸")]
    public int length, width;      // 物品占格子的长、宽
    public RectTransform rectTransform;

    [Header("物品关联数据")]
    public List<LaiwenBagCell> linkObject; // 物品占据的背包格子
    public int maxCell;                   // 最大占据格子数（长×宽）
    public bool onlyHorizon;              // 仅允许水平放置
    public bool onlyVertical;             // 仅允许垂直放置
    public bool placed;                   // 是否已正确放置
    public bool doNotOnly;                // 不强制单一方向
    private Transform _originParent;      // 物品初始父物体
    public RectTransform childCollider;   // 子物体碰撞区域
    public LaiwenItemManager data;        // 物品配置数据
    public GameObject textBar;            // 提示文本框
    public TextMeshProUGUI content;       // 提示文本内容
    public bool isDragging;               // 是否正在拖拽
    public Manager additionText;          // 额外文本管理器
    private static List<GameObject> _placedObject = new List<GameObject>();
    public Manager language;
    public AudioSource aS;
    /// <summary>
    /// 初始化组件与初始状态
    /// </summary>
    public override void Awake()
    {
        base.Awake();
        rectTransform = GetComponent<RectTransform>();
        _originParent = transform.parent;          // 记录原始父物体
        childCollider = transform.GetChild(0).GetComponent<RectTransform>();

        // 查找提示UI并默认隐藏
        try
        {
            textBar = transform.parent.parent.Find("TextBar").gameObject;
            content = textBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            textBar.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.LogError($"莱文小游戏物品文本对象获取失败,{e}", this);
        }
    }

    private void Start()
    {
        aS = transform.parent.GetComponent<AudioSource>();
    }

    /// <summary>
    /// 拖拽中：标记拖拽状态 + 设置当前操作物品
    /// </summary>
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        isDragging = true;
        if (LaiwenMiniData.CurItem != this)
        {
            LaiwenMiniData.CurItem = this;
        }
    }

    /// <summary>
    /// 启用时：初始化物品信息、方向限制、加入全局列表
    /// </summary>
    private void OnEnable()
    {
        var data = new LaiwenItemInfo();
        data.item = this;
        data.position = childCollider;
        
        maxCell = length * width; // 计算最大占据格子数

        // 自动设置方向限制：1格长→仅水平；1格宽→仅垂直
        if (length == 1 && !doNotOnly) onlyHorizon = true;
        if (width == 1 && !doNotOnly) onlyVertical = true;

        // 加入全局物品列表
        if (LaiwenItemGroup.Infos == null)
            LaiwenItemGroup.Infos = new List<LaiwenItemInfo>();
        
        LaiwenItemGroup.Infos.Add(data);
        LaiwenMiniData.AddItems();
        
    }

    /// <summary>
    /// 添加占据的背包格子（检查方向、数量限制）
    /// </summary>
    /// <returns>是否添加成功</returns>
    public bool AddList(LaiwenBagCell value)
    {
        // 格子已满
        if (linkObject.Count == maxCell) return false;
        
        // 水平/垂直方向限制判断
        if (onlyHorizon && linkObject.Count > 0 && value.index / 5 != linkObject[0].index / 5) return false;
        if (onlyVertical && linkObject.Count > 0 && value.index % 4 != linkObject[0].index % 4) return false;

        linkObject.Add(value);
        
        // 格子占满 → 标记已正确放置
        if (linkObject.Count == maxCell) placed = true;
        return true;
    }

    /// <summary>
    /// 移除关联格子 → 取消放置状态
    /// </summary>
    public void RemoveList(LaiwenBagCell value)
    {
        linkObject.Remove(value);
        placed = false;
    }

    /// <summary>
    /// 鼠标抬起：放置/归位 + 结算分数
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        aS.Play();
        if (placed)
        {
            
            // 正确放置：对齐到格子中心
            if (maxCell == 1)
            {
                transform.SetParent(linkObject[0].transform);
                rectTransform.anchoredPosition = Vector2.zero;
            }
            else
            {
                transform.SetParent(linkObject[0].transform.parent.parent);
                rectTransform.anchoredPosition = GetMiddlePosition();
            }
            // 正确放置计数+1
            LaiwenMiniData.LaiwenManager.AddCorrect();
            _placedObject.Add(gameObject);
        }
        else
        {
            foreach (var value in _placedObject)
            {
                if (value == gameObject) continue;
                if (UiCollider.IsCollision(value.GetComponent<RectTransform>(),rectTransform))
                {
                    LaiwenMiniData.OverLap = true;
                    break;
                }
                
            }
            // 放置失败 → 回到初始位置
            rectTransform.anchoredPosition = startPosition;
            
            // 格子不足标记
            if (linkObject.Count > 0 && linkObject.Count < maxCell)
                LaiwenMiniData.LackSpace = true;
            
        }

        textBar.SetActive(false);
        isDragging = false;
    }

    /// <summary>
    /// 鼠标按下：拿起已放置物品 + 取消分数
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (placed)
        {
            transform.SetParent(_originParent);
            LaiwenMiniData.LaiwenManager.DecreaseCorrect(); // 分数-1
            _placedObject.Remove(gameObject);
        }

        textBar.SetActive(false);
        isDragging = false;
    }

    /// <summary>
    /// 鼠标移入：显示物品提示文本（拖拽时不显示）
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        content.text = language.isEn? data.enContext : data.context;
        if (!isDragging) textBar.SetActive(true);
    }

    /// <summary>
    /// 强制隐藏提示文本
    /// </summary>
    public void SetTextBar()
    {
        textBar.SetActive(false);
    }

    /// <summary>
    /// 计算多格物品的中心位置（用于居中摆放）
    /// </summary>
    private Vector2 GetMiddlePosition()
    {
        Vector2 totalPos = Vector2.zero;
        foreach (var cell in linkObject)
        {
            Vector2 cellLocalPos = ((RectTransform)linkObject[0].transform.parent.parent)
                .InverseTransformPoint(cell.rectTransform.position);
            totalPos += cellLocalPos;
        }
        return totalPos / linkObject.Count; // 平均坐标 = 中心
    }
}

/// <summary>
/// 全局静态类：存储所有莱文物品信息
/// </summary>
public static class LaiwenItemGroup
{
    public static List<LaiwenItemInfo> Infos = new List<LaiwenItemInfo>();
}