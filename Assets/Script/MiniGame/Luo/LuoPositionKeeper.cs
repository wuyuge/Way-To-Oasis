using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuoPositionKeeper : MonoBehaviour
{
    private Vector3 worldPosition;
    private Quaternion worldRotation;
    
    private void Awake()
    {
        // 保存初始世界坐标
        worldPosition = transform.position;
        worldRotation = transform.rotation;
    }
    
    private void LateUpdate()
    {
        // 每帧强制恢复位置
        transform.position = worldPosition;
        transform.rotation = worldRotation;
    }
}
