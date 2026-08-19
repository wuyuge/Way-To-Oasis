using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LuoLevelLoader : MonoBehaviour
{
    [Header("水源物体")]
    public Transform source1, source2;

    [Header("关卡数据池")]
    public List<LuoLevelData> data;

    [Header("左右水源点位")]
    public List<Transform> leftSourcePositions;
    public List<Transform> rightSourcePositions;

    [Header("已生成管道缓存")]
    public List<GameObject> pipes;

    [Header("16个管道生成点位")]
    public List<Transform> pipePositions;

    [Header("管道预制体")]
    public GameObject straightPrefab, teePrefab, anglePrefab, fourWayPrefab, blindPrefab,itemBox;
    
    [Header("管道物品预制体")]
    public GameObject straightItem, teeItem,angleItem,fourWayItem;

    public RectTransform limitArea;
    
    [Header("管道父物体（可选，整理层级）")]
    public Transform pipeParent;
    
    public List<RectTransform> itemPositions;

    private LuoLevelData _curData;
    private Animator _anim;
    [Range(1,2)] public int rank;

    public AudioSource aS;

    public void Initialize()
    {
        _anim = GetComponent<Animator>();
        // 清空上一轮所有管道（你原有倒序删除逻辑保留，仅加固空判断）
        ClearOldPipes();

        for (int i = 0; i < 16; i++)
        {
            LuoGlobalData.PipeList[i] = null;
        }

        LuoGlobalData.LinkedPipeList.Clear();
        LuoGlobalData.StartPipe = null;
        LuoGlobalData.DestinationPipe = null;
        LuoGlobalData.MaxCorrect = 0;

        LuoGlobalData.LevelLoader = this;
        LuoGlobalData.ItemList.Clear();
        if (data.Count == 0)
        {
            Debug.LogWarning("关卡数量不足，data列表为空", this);
            return;
        }

        // 随机取关卡：用索引避免Remove匹配bug
        // 筛选同等级数据
        List<LuoLevelData> matchList = data.Where(item => item.rank == rank).ToList();

        // 容错：没有对应rank的数据直接返回，防止报错死循环
        if (matchList.Count == 0)
        {
            Debug.LogError($"不存在rank={rank}的关卡数据！");
            return; // 或给默认数据
        }

        // 直接在符合条件的列表里随机，一次到位
        int randomIdx = Random.Range(0, matchList.Count);
        LuoLevelData level = matchList[randomIdx];
        
        _curData = level;
        data.Remove(level);

        // 设置水源位置，增加下标边界校验
        SetSourcePos(level);

        // 生成管道，拆分try范围，精准捕获点位越界
        SpawnLevelPipe(level);
    }

    public void ResetLevel()
    {
        // 先主动停止所有管道上的协程，避免销毁帧的竞态
        foreach (var pipe in LuoGlobalData.PipeList)
        {
            if (pipe != null)
                pipe.StopAllCoroutines();
        }

        foreach (Transform child in pipeParent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var value in LuoGlobalData.ItemList)
        {
            Destroy(value.gameObject);
        }

        LuoGlobalData.LinkedPipeList.Clear();

        SetSourcePos(_curData);
        SpawnLevelPipe(_curData);
    }

    public void Clear()
    {
        foreach (Transform child in pipeParent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var value in LuoGlobalData.ItemList)
        {
            Destroy(value.gameObject);
        }

        // 新增：静态数据一并清理
        LuoGlobalData.LinkedPipeList.Clear();
        LuoGlobalData.ItemList.Clear();
        for (int i = 0; i < 16; i++)
        {
            LuoGlobalData.PipeList[i] = null;
        }
    }

    public void SetEnd()
    {
        _anim.SetTrigger("End");
    }

    /// <summary>
    /// 销毁并清空已有管道
    /// </summary>
    private void ClearOldPipes()
    {
        for (int i = pipes.Count - 1; i >= 0; i--)
        {
            GameObject go = pipes[i];
            if (go != null)
                Destroy(go);
            pipes.RemoveAt(i);
        }
    }

    /// <summary>
    /// 赋值两个水源坐标，防止索引越界
    /// </summary>
    private void SetSourcePos(LuoLevelData level)
    {
        // 左水源 source1
        if (level.source1 < leftSourcePositions.Count)
        {
            Transform leftPos = leftSourcePositions[level.source1];
            if (leftPos != null)
                source1.position = leftPos.position;
            else
                Debug.LogError($"左水源点位[{level.source1}]为空", this);
        }
        else
        {
            Debug.LogError($"关卡source1索引{level.source1}超出leftSourcePositions范围", this);
        }

        // 右水源 source2
        if (level.source2 < rightSourcePositions.Count)
        {
            Transform rightPos = rightSourcePositions[level.source2];
            if (rightPos != null)
                source2.position = rightPos.position;
            else
                Debug.LogError($"右水源点位[{level.source2}]为空", this);
        }
        else
        {
            Debug.LogError($"关卡source2索引{level.source2}超出rightSourcePositions范围", this);
        }
    }

    /// <summary>
    /// 循环生成所有管道，单独捕获下标越界异常
    /// </summary>
    private void SpawnLevelPipe(LuoLevelData level)
    {
        List<LuoLevelData.LevelData> pipeDatas = level.level;
        int index = -1;
        for (int i = 0; i < pipeDatas.Count; i++)
        {
            LuoLevelData.LevelData pipeData = pipeDatas[i];

            index += 1;
            
            // 空管道直接跳过，不生成
            if (pipeData.pipe == LuoPipeType.None)
            {
                continue;
            }
            if (pipeData.pipe == LuoPipeType.Item)
            {
                Instantiate(itemBox,pipePositions[i].position,Quaternion.identity,pipeParent).GetComponent<LuoItemBox>().Set(index);
                continue;
            }

            // 校验点位下标，防止越界
            if (i >= pipePositions.Count || pipePositions[i] == null)
            {
                Debug.LogError($"管道点位{i}不存在或为空，关卡管道与点位数量不匹配", this);
                continue;
            }

            // 获取对应预制体
            GameObject targetPrefab = GetPipePrefab(pipeData.pipe);
            if (targetPrefab == null)
            {
                Debug.LogError($"管道类型 {pipeData.pipe} 预制体未赋值", this);
                continue;
            }

            // 生成旋转
            Quaternion rot = Quaternion.Euler(0, 0, -90 * pipeData.toward);
            Vector3 pos = pipePositions[i].position;

            GameObject newPipe;
            if (pipeParent != null)
                newPipe = Instantiate(targetPrefab, pos, rot, pipeParent);
            else
                newPipe = Instantiate(targetPrefab, pos, rot);
            var pipe = newPipe.GetComponent<Pipe>();
            if (pipeData.isStart)
            {
                pipe.SetStart(pipeData.startPos);
            }

            if (pipeData.isDestination)
            {
                pipe.SetDestination(pipeData.destinationPos);
            }

            switch (pipeData.toward)
            {
                case 0:
                    pipe.SetToward(Toward.Up,index);
                    break;
                case 1:
                    pipe.SetToward(Toward.Right,index);
                    break;
                case 2:
                    pipe.SetToward(Toward.Down,index);
                    break;
                case 3:
                    pipe.SetToward(Toward.Left,index);
                    break;
            }
            
            
            pipes.Add(newPipe);
        }

        foreach (var value in LuoGlobalData.PipeList)
        {
            if (value != null)
            {
                value.CacheNeighbors();
            }
        }

        List<RectTransform> position = new List<RectTransform>(itemPositions);
        foreach (var value in level.items)
        {
            GameObject newPipe = null;
            RectTransform pos = position[Random.Range(0, position.Count)];
            position.Remove(pos);
            switch (value)
            {
                case LuoPipeType.Straight:
                    newPipe = straightItem;
                    break;
                case LuoPipeType.Tee:
                    newPipe = teeItem;
                    break;
                case LuoPipeType.Angle:
                    newPipe = angleItem;
                    break;
                case LuoPipeType.FourWay:
                    newPipe = fourWayItem;
                    break;
            }
            
            Instantiate(newPipe,pos.position,Quaternion.identity,limitArea);
            
        }

        LuoGlobalData.TotalRollTime = 0;

    }

    public void Spawn(LuoPipeType type,int index,bool randomColor = true)
    { 
        var temp = Instantiate(GetPipePrefab(type), pipePositions[index].position, Quaternion.identity,pipeParent);
        var pipe = temp.GetComponent<Pipe>();
        pipe.RandomColor = randomColor;
        pipe.SetToward(Toward.Up,index);
        pipes.Add(temp);
        foreach (var value in LuoGlobalData.PipeList)
        {
            if (value != null)
            {
                value.CacheNeighbors();
            }
        }
    }
    
    
    

    /// <summary>
    /// 根据管道类型返回预制体
    /// </summary>
    private GameObject GetPipePrefab(LuoPipeType type)
    {
        return type switch
        {
            LuoPipeType.Straight => straightPrefab,
            LuoPipeType.Tee => teePrefab,
            LuoPipeType.Angle => anglePrefab,
            LuoPipeType.FourWay => fourWayPrefab,
            LuoPipeType.Blind => blindPrefab,
            _ => null
        };
    }
}

