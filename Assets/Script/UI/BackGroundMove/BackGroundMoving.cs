using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMoving : MonoBehaviour
{
    [System.Serializable]
    public class BackgroundLayer
    {
        public GameObject layerObject;
        [Tooltip("移动速度系数，值越大越大移动越快")]
        public float speedFactor = 1f;
    }

    [Tooltip("基础移动速度")]
    public float baseSpeed = 0.01f;

    [Tooltip("背景层列表，包含游戏对象和各自的速度系数")]
    public List<BackgroundLayer> backgroundLayers = new List<BackgroundLayer>();

    [Tooltip("背景重置位置（当背景移动到该位置时重置）")]
    public float resetPositionX;

    [Tooltip("重置后的位置")]
    public float spawnPositionX;

    [Tooltip("初始位置")]
    public float startPositionX;

    [Tooltip("控制是否滚动")]
    public bool open = false;

    [Tooltip("停止位置")]
    public float StopPositionx;

    public GameObject BackImage;


    public GameObject LightSys, Skip;

    // 存储各背景层的初始位置
    public  List<Vector3> initialLayerPositions = new List<Vector3>();



    private void Start()
    {
        // 记录初始位置
        RecordInitialPositions();
    }

    // 记录所有背景层的初始位置
    private void RecordInitialPositions()
    {
        initialLayerPositions.Clear();

        foreach (var layer in backgroundLayers)
        {
            if (layer.layerObject != null)
            {
                initialLayerPositions.Add(layer.layerObject.transform.position);
            }
            else
            {
                // 对于空对象添加一个默认位置
                initialLayerPositions.Add(Vector3.zero);
            }
        }
    }

    private void FixedUpdate()
    {
        if (open && BackImage.GetComponent<RectTransform>().position.x > initialLayerPositions[9].x)
        { MoveAllLayers(); }
        if (LightSys.GetComponent<DayNightSystem>().complete)
        {
            //Skip.GetComponent<Skip>().TurnDark();
            LightSys.GetComponent<DayNightSystem>().complete = false;

            LightSys.GetComponent<DayNightSystem>().enabled = false;
            open = false;
        }

        

    }

    private void MoveAllLayers()
    {
        foreach (var layer in backgroundLayers)
        {
            if (layer.layerObject != null)
            {
                // 计算该层的实际移动速度 = 基础速度 × 速度系数
                float layerSpeed = baseSpeed * layer.speedFactor;

                // 移动背景层
                Vector2 newPosition = layer.layerObject.transform.position;
                newPosition.x -= layerSpeed;
                layer.layerObject.transform.position = newPosition;

                // 检查是否需要重置位置（循环滚动）
                CheckAndResetLayerPosition(layer.layerObject);
            }
        }
    }

    private void CheckAndResetLayerPosition(GameObject layer)
    {
        // 当背景移动到重置位置时，将其移回起始位置以实现无限滚动
        if (layer.transform.position.x <= resetPositionX)
        {
            Vector2 resetPos = layer.transform.position;
            resetPos.x = spawnPositionX;
            layer.transform.position = resetPos;
        }
    }

    // 在编辑器中可视化背景层
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // 绘制重置位置线
        Gizmos.DrawLine(
            new Vector3(resetPositionX, -100, 0),
            new Vector3(resetPositionX, 100, 0)
        );

        Gizmos.color = Color.green;
        // 绘制生成位置线
        Gizmos.DrawLine(
            new Vector3(spawnPositionX, -100, 0),
            new Vector3(spawnPositionX, 100, 0)
        );
    }

    public void Re_set()
    {
        Debug.Log("重置背景");

        // 还原所有背景层到初始记录的位置
        for (int i = 0; i < backgroundLayers.Count; i++)
        {
            var layer = backgroundLayers[i];
            if (layer.layerObject != null && i < initialLayerPositions.Count)
            {
                layer.layerObject.transform.position = initialLayerPositions[i];
            }
        }

        // 重置日夜系统状态
        if (LightSys != null && LightSys.GetComponent<DayNightSystem>() != null)
        {
            LightSys.GetComponent<DayNightSystem>().time = 0;
            LightSys.GetComponent<DayNightSystem>().Reset_Color();
            LightSys.GetComponent<DayNightSystem>().enabled = true;
            LightSys.GetComponent<DayNightSystem>().complete = false;
            LightSys.GetComponent<DayNightSystem>().on = true;
        }

        open = false;
    }
}