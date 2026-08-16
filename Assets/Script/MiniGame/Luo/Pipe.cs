using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public abstract class Pipe : MonoBehaviour
{
    [Header("旋转平滑速度")]
    public float rotateSpeed = 5f;
    private Quaternion targetRot;
    private bool isRotating = false;
    private Coroutine _coroutine;
    public bool isConnected;
    public bool isStart, isDestination;
    protected Toward startPos, destinationPos;
    public Toward curToward;
    public Pipe up, down, left, right;
    public int index;
    private List<Toward> _walls = new List<Toward>();
    public bool breakTag;
    [Header("颜色")]
    public Sprite normal, red;
    private const int MaxRollTime = 40;
    public bool RandomColor = true;
    public AudioSource aS;
    private Coroutine _rollCheckRoutine; // RollCheck 协程（新增）

    /// <summary>
    /// 列
    /// </summary>
    public int col;
    /// <summary>
    /// 行
    /// </summary>
    public int row;

    public int RollTime
    {
        get;
        set;
    }
    


    private void OnEnable()
    {
        if (aS == null) aS = GetComponent<AudioSource>();

        if (!RandomColor)
        {
            GetComponent<Image>().sprite = normal;
            return;
        }
        var i = Random.Range(0, 2);
        if (i == 0)
        {
            GetComponent<Image>().sprite = normal;
        }
        else
        {
            GetComponent<Image>().sprite = red;
        }
    }


    void Update()
    {
        if (!RandomColor)
        {
            GetComponent<Image>().sprite = normal;
            
        }
        if (!isRotating) return;

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotateSpeed);

        if (Quaternion.Angle(transform.rotation, targetRot) < 7.5f)
        {
            transform.rotation = targetRot;
            isRotating = false;
        }
    }

    #region 旋转

    public void RotateRight90()
    {
        if (isRotating) return;
        aS.Play();
        targetRot = Quaternion.Euler(0, 0, transform.eulerAngles.z + 90f);
        isRotating = true;
        LuoGlobalData.TotalRollTime++;
        if (LuoGlobalData.TotalRollTime >= MaxRollTime)
        {
            LuoGlobalData.TalkSys.SetFail();
        }
        SetRepeat();
        RotateToward();
        RefreshAllConnections();
    }

    public void RotateLeft90()
    {
        if (isRotating) return;
        aS.Play();
        targetRot = Quaternion.Euler(0, 0, transform.eulerAngles.z - 90f);
        isRotating = true;
        LuoGlobalData.TotalRollTime++;
        if (LuoGlobalData.TotalRollTime >= MaxRollTime)
        {
            LuoGlobalData.TalkSys.SetFail();
        }
        SetRepeat();
        RotateToward();
        RefreshAllConnections();
    }

    #endregion

    private void SetRepeat()
    {
        RollTime++;
        foreach (var value in LuoGlobalData.PipeList)
        {
            if (value == null) continue;
            if(value != this)
            {
                value.RollTime = 0;
            }
        }

        if (RollTime >= 12)
        {
            LuoGlobalData.TalkSys.SetRepeat();
            RollTime = 3;
        }
    }
    
    

    /// <summary>
    /// 全图重置连通性 + 从起点重新扩散
    /// </summary>
    private void RefreshAllConnections()
    {
        // 1. 重置所有管道的连通状态
        foreach (var pipe in LuoGlobalData.PipeList)
        {
            if (pipe != null) pipe.isConnected = false;
        }

        // 2. 起点设为连通，开始洪水填充
        if (LuoGlobalData.StartPipe != null)
        {
            LuoGlobalData.StartPipe.isConnected = true;
            LuoGlobalData.StartPipe.Check();
        }
    }

    public void SetStart(Toward pos)
    {
        isStart = true;
        startPos = pos;
        isConnected = true; // 起点默认连通
        LuoGlobalData.StartPipe = this;
    }

    public void SetDestination(Toward pos)
    {
        isDestination = true;
        destinationPos = pos;
        LuoGlobalData.DestinationPipe = this;
    }

    /// <summary>
    /// 重新计算自身连通性；如果从"不通变通"，就向四周扩散
    /// </summary>
    public virtual void Check()
    {
        // 子类重写：根据开口方向 + 邻居状态 更新 isConnected
        CalculateConnection();
        CheckInitial();

        if (isConnected && !LuoGlobalData.LinkedPipeList.Contains(this))
        {
            LuoGlobalData.LinkedPipeList.Add(this);
            if (isDestination)
            {
                _coroutine = StartCoroutine(CheckBreak());
            }
        }
        
        if (isStart)
        {
            if (LuoGlobalData.LinkedPipeList.Count > LuoGlobalData.MaxCorrect)
            {
                LuoGlobalData.MaxCorrect = LuoGlobalData.LinkedPipeList.Count;
            }
            else if (LuoGlobalData.LinkedPipeList.Count < LuoGlobalData.MaxCorrect)
            {
                LuoGlobalData.TalkSys.CheckMistake(LuoGlobalData.LinkedPipeList.Count);
            }
        }

        if (!isConnected && LuoGlobalData.LinkedPipeList.Contains(this))
        {
            LuoGlobalData.LinkedPipeList.Remove(this);
            if (isDestination)
            {
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }
            }
        }
        
        if (isDestination && isConnected)
        {
            CheckDestination();
        }
        
    }
    
    protected abstract void CheckDestination();
    

    IEnumerator RollCheck()
    {
        while (true)
        {
            Check();
            yield return new WaitForSecondsRealtime(0.25f);
        }
    }
    
    
    

    /// <summary>
    /// 子类重写：根据自身管道类型判断是否连通
    /// </summary>
    protected virtual void CalculateConnection()
    {
        CheckInitial(); // 兼容原有 CheckInitial 逻辑
    }

    protected abstract void CheckInitial();

    public virtual void RotateToward()
    {
        curToward = curToward switch
        {
            Toward.Up    => Toward.Right,
            Toward.Right => Toward.Down,
            Toward.Down  => Toward.Left,
            Toward.Left  => Toward.Up,
            _ => curToward
        };
    }

    public void SetToward(Toward pos, int num)
    {
        curToward = pos;
        index = num;
        col = index % 4;
        row = index / 4;
        LuoGlobalData.PipeList[index] = this;
        SetEdge();

        // 先停止旧的，再启动新的，防止重复叠加
        if (_rollCheckRoutine != null)
            StopCoroutine(_rollCheckRoutine);
        _rollCheckRoutine = StartCoroutine(RollCheck());
    }

    /// <summary>
    /// 所有管道生成完毕后统一调用一次，建立邻居关系
    /// </summary>
    public void CacheNeighbors()
    {
        int col = index % 4;
        int row = index / 4;

        up    = row > 0 ? LuoGlobalData.PipeList[index - 4] : null;
        down  = row < 3 ? LuoGlobalData.PipeList[index + 4] : null;
        left  = col > 0 ? LuoGlobalData.PipeList[index - 1] : null;
        right = col < 3 ? LuoGlobalData.PipeList[index + 1] : null;
    }

    /// <summary>
    /// 根据方向取邻居（工具方法，子类通用）
    /// </summary>
    protected Pipe GetNeighbor(Toward dir)
    {
        return dir switch
        {
            Toward.Up    => up,
            Toward.Down  => down,
            Toward.Left  => left,
            Toward.Right => right,
            _ => null
        };
    }
    
    
    /// <summary>
    /// 某个方向是否有开口（由子类根据管道类型实现）
    /// </summary>
    public abstract bool HasOpening(Toward dir);

    /// <summary>
    /// 工具方法：取反方向
    /// </summary>
    protected Toward GetOpposite(Toward t)
    {
        return t switch
        {
            Toward.Up    => Toward.Down,
            Toward.Down  => Toward.Up,
            Toward.Left  => Toward.Right,
            Toward.Right => Toward.Left,
            _ => t
        };
    }

    private IEnumerator CheckBreak()
    {
        while (true)
        {
            // 自身已被销毁，直接退出
            if (this == null) yield break;

            bool pass = true;
            // 用副本遍历，避免遍历期间列表被修改
            var snapshot = new List<Pipe>(LuoGlobalData.LinkedPipeList);
            foreach (var value in snapshot)
            {
                // 跳过已销毁的管道
                if (value == null) continue;

                if (!value.CheckPipeBreak())
                {
                    pass = false;
                    Debug.Log("出现断点在", value.gameObject);
                    break;
                }
            }

            // 列表为空时不能算通关
            if (snapshot.Count == 0)
                pass = false;

            if (pass)
            {
                LuoGlobalData.TalkSys.SetSuccess();
                _coroutine = null; // 协程正常结束，置空避免后续 StopCoroutine 报错
                yield break;
            }
            else
            {
                LuoGlobalData.TalkSys.SetBreak();
            }

            if (isDestination && !isConnected)
            {
                _coroutine = null;
                yield break;
            }

            yield return new WaitForSecondsRealtime(0.25f);
        }
    }


    protected abstract bool CheckPipeBreak();

    
    protected bool IsEdge()
    {
        return (row == 0 || row == 3) && (col == 0 || col == 3);
    }
    
    protected bool IsEdge(Toward dir)
    {
        return _walls.Contains(dir);
    }
    
    

    private void SetEdge()
    {
        if (row == 0)
        {
            _walls.Add(Toward.Up);
        }
        else if (row == 3)
        {
            _walls.Add(Toward.Down);
        }

        if (col == 0)
        {
            _walls.Add(Toward.Left);
        }
        else if (col == 3)
        { 
            _walls.Add(Toward.Right);
        }
        
    }
    

    /// <summary>
    /// 
    /// </summary>
    /// <returns> a为上下   b为左右</returns>
    protected (Toward a, Toward b) GetEdge()
    {
        (Toward a, Toward b) result = new ();

        if (row == 0)
        {
            result.a = Toward.Up;
        }

        if (row == 3)
        {
            result.a = Toward.Down;
        }

        if (col == 0)
        {
            result.b = Toward.Left;
        }

        if (col == 3)
        {
            result.b = Toward.Right;
        }

        return result;

    }
    
    private void OnDestroy()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        if (_rollCheckRoutine != null)
        {
            StopCoroutine(_rollCheckRoutine);
            _rollCheckRoutine = null;
        }
    }

}