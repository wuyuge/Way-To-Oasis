using System;
using UnityEngine;
using UnityEngine.UI;

public class LaiwenBagCell : MonoBehaviour
{
    public RectTransform rectTransform;
    public bool isColliding, added, isDown;
    public LaiwenItemInfo curInfo;
    public Image image;
    public int index;

    // 颜色缓存，避免每帧重复设置
    private Color _collidingColor = new Color32(0, 0, 0, 180);
    private Color _normalColor = new Color32(0, 0, 0, 125);
    private bool _lastCollidingState;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        index = transform.GetSiblingIndex();
        if (isDown)
            index += 20;

        /*// 初始化颜色为非碰撞状态
        image.color = _normalColor;
        _lastCollidingState = false;*/
    }

    private void Start()
    {
        image.color = new Color32(0, 0, 0, 0);
    }


    private void Update()
    {
        if (!isColliding)
        {
            // 寻找第一个碰撞的物体
            foreach (var info in LaiwenItemGroup.Infos)
            {
                if (UiCollider.IsCollision(rectTransform, info.position))
                {
                    curInfo = info;
                    isColliding = info.item.AddList(this);
                    if (isColliding)break; // 发现碰撞立即退出循环，避免继续遍历
                }
            }
        }
        else
        {
            // 已碰撞，检查是否还在碰撞范围内
            if (!UiCollider.IsCollision(rectTransform, curInfo.position))
            {
                isColliding = false;
                curInfo.item.RemoveList(this);
            }
        }

        /*// 只在碰撞状态改变时更新颜色
        if (isColliding != _lastCollidingState)
        {
            _lastCollidingState = isColliding;
            image.color = isColliding ? _collidingColor : _normalColor;
        }*/
    }
}