using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public abstract class Pipe : MonoBehaviour
{
    public bool isConnected;
    public bool isStartPoint,isDestination;
    [SerializeField]
    protected GameObject above,below,left,right;
    public int pipeNumber;
    private PipeManager AboveComponent, LeftComponent,BelowComponent,RightComponent;
    private Animator _animator;
    private Image _objectImage;
    public PipeTowards startTowards,endTowards;
    private static List<GameObject> _reachPipeList = new List<GameObject>();
    protected static GameObject StartPoint;
    protected static PipeManager StartPipe;
    public bool startIsVertical,destinationIsVertical;
    public Sprite pipeSprite,pipeSpriteRed;
    protected PipeManager Manager;
    private static GameObject _startPipe, _endPipe;
    public GameObject upP, downP, leftP, rightP;
    private void Awake()
    {
        _objectImage = GetComponent<Image>();
        Manager = GetComponent<PipeManager>();
        pipeNumber = transform.GetSiblingIndex();
        below = pipeNumber - 4 >= 0  ? transform.parent.GetChild(pipeNumber - 4).gameObject : null;
        above = pipeNumber + 4 < transform.parent.childCount ? gameObject.transform.parent.GetChild(pipeNumber + 4).gameObject : null;
        var rightIndex = pipeNumber + 1;
        right = (rightIndex < transform.parent.childCount) && (pipeNumber % 4 != 3) 
            ? transform.parent.GetChild(rightIndex).gameObject 
            : null;
        var leftIndex = pipeNumber - 1;
        left = (leftIndex >= 0) && (pipeNumber % 4 != 0) 
            ? transform.parent.GetChild(leftIndex).gameObject 
            : null;
        try
        {
            if (above is not null)
            {
                AboveComponent = above.GetComponent<PipeManager>();
                
            }
            if (below is not null)
            {
                BelowComponent = below.GetComponent<PipeManager>();
                
            }
            if (left is not null)
            {
                LeftComponent = left.GetComponent<PipeManager>();
                
            }
            if (right is not null)
            {
                RightComponent = right.GetComponent<PipeManager>();
                
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e,this);
            throw;
        }
        
        _animator = GetComponent<Animator>();
        if (_startPipe is null)
        {
            _startPipe = gameObject.transform.parent.parent.parent.Find("StartPipe").gameObject;
        }
        if (_endPipe is null)
        {
            _endPipe = gameObject.transform.parent.parent.parent.Find("EndPipe").gameObject;
        }

        upP = transform.Find("Up").gameObject;
        downP = transform.Find("Down").gameObject;
        rightP = transform.Find("Right").gameObject;
        leftP = transform.Find("Left").gameObject;
    }

    /// <summary>
    /// 初始化管道组件。如果该管道是起点，开始尝试链接其他管道形成通路
    /// </summary>
    public virtual void Start()
    {
        if (isStartPoint)
        {
            StartPoint = gameObject;
            StartPipe = StartPoint.GetComponent<PipeManager>();
        }   

        
        if (isStartPoint)
        {
            if (startIsVertical)
            {
                if (above is null)
                {
                    startTowards = PipeTowards.Above;
                }
                else if (below is null)
                {
                    startTowards = PipeTowards.Below;
                }
               
            }
            else if (!startIsVertical)
            {
                if (left is null)
                {
                    startTowards = PipeTowards.Left;
                }

                if (right is null)
                {
                    startTowards = PipeTowards.Right;
                }
            }
            SwitchPipePosition(_startPipe,startTowards);
            CheckStartConnection();
            CheckConnectivity();
            
        }
        if (isDestination)
        {
            if (destinationIsVertical)
            {
                if (above is null)
                {
                    endTowards = PipeTowards.Above;
                }
                else if (below is null)
                {
                    endTowards = PipeTowards.Below;
                }

            }
            else
            {
                if (left is null)
                {
                    endTowards = PipeTowards.Left;
                }

                if (right is null)
                {
                    endTowards = PipeTowards.Right;
                }
            }
            SwitchPipePosition(_endPipe,endTowards);
        }

        var temp = Random.Range(0, 1);
        if (temp == 0)
        {
            _objectImage.sprite = pipeSprite;
        }
        else
        {
            _objectImage.sprite = pipeSpriteRed;
        }
    }

    #region 重置用

    

    
    private void OnEnable()
    {
        var temp = Random.Range(0, 1);
        if (temp == 0)
        {
            _objectImage.sprite = pipeSprite;
        }
        else
        {
            _objectImage.sprite = pipeSpriteRed;
        }
        
        
        RelinkOther();
    }

    void RelinkOther()
    {
        below = pipeNumber - 4 >= 0  ? transform.parent.GetChild(pipeNumber - 4).gameObject : null;
        above = pipeNumber + 4 < transform.parent.childCount ? gameObject.transform.parent.GetChild(pipeNumber + 4).gameObject : null;
        var rightIndex = pipeNumber + 1;
        right = (rightIndex < transform.parent.childCount) && (pipeNumber % 4 != 3) 
            ? transform.parent.GetChild(rightIndex).gameObject 
            : null;
        var leftIndex = pipeNumber - 1;
        left = (leftIndex >= 0) && (pipeNumber % 4 != 0) 
            ? transform.parent.GetChild(leftIndex).gameObject 
            : null;
        try
        {
            if (above is not null)
            {
                AboveComponent = above.GetComponent<PipeManager>();
                
            }
            if (below is not null)
            {
                BelowComponent = below.GetComponent<PipeManager>();
                
            }
            if (left is not null)
            {
                LeftComponent = left.GetComponent<PipeManager>();
                
            }
            if (right is not null)
            {
                RightComponent = right.GetComponent<PipeManager>();
                
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e,this);
            throw;
        }
        if (isStartPoint)
        {
            StartPoint = gameObject;
            StartPipe = StartPoint.GetComponent<PipeManager>();
        }   
        
        if (isStartPoint)
        {
            if (startIsVertical)
            {
                if (above is null)
                {
                    startTowards = PipeTowards.Above;
                }
                else if (below is null)
                {
                    startTowards = PipeTowards.Below;
                }
               
            }
            else if (!startIsVertical)
            {
                if (left is null)
                {
                    startTowards = PipeTowards.Left;
                }

                if (right is null)
                {
                    startTowards = PipeTowards.Right;
                }
            }
            
            CheckStartConnection();
            CheckConnectivity();
            
        }
        if (isDestination)
        {
            if (destinationIsVertical)
            {
                if (above is null)
                {
                    endTowards = PipeTowards.Above;
                }
                else if (below is null)
                {
                    endTowards = PipeTowards.Below;
                }

            }
            else if (!destinationIsVertical)
            {
                if (left is null)
                {
                    endTowards = PipeTowards.Left;
                }

                if (right is null)
                {
                    endTowards = PipeTowards.Right;
                }
            }
            CheckDestinationConnection();
        }
    }
    
    #endregion
    



    public enum PipeTowards
    {
        Above,
        Below,
        Left,
        Right,
        None
    }

    public void Click()
    {
        _animator.SetTrigger("rotate");
        if (LuoStaticData.CurrentPipe == gameObject)
        {
            LuoStaticData.RollTime++;
        }
        else
        {
            LuoStaticData.CurrentPipe = gameObject;
            LuoStaticData.RollTime = 0;
        }
    }

    /// <summary>
    /// 用于新组件被添加时对四个方向的调用更新
    /// </summary>
    /// <param name="towards"></param>
    public void UpdateLinkComponent(PipeTowards towards)
    {
        switch (towards)
        {
            case PipeTowards.Above:
                AboveComponent = above.GetComponent<PipeManager>();
                break;
            case PipeTowards.Below:
                BelowComponent = below.GetComponent<PipeManager>();
                break;
            case PipeTowards.Right:
                RightComponent = right.GetComponent<PipeManager>();
                break;
            case PipeTowards.Left:
                LeftComponent = left.GetComponent<PipeManager>();
                break;
        }
    }

    public void RestConnection()
    {
        for (int index = 0; index < transform.parent.childCount; index++)
        {
            PipeManager tempPipe = transform.parent.GetChild(index).gameObject.GetComponent<PipeManager>();
            if (!tempPipe.isStartPoint)
            {
                tempPipe.SetConnect(false);
            }
            
        }
        _reachPipeList.Clear();
    }

    public virtual void SetState()
    {
        RestConnection();
        if (isStartPoint)
        {
            CheckStartConnection();
            if (isStartPoint && isConnected)
            {
                CheckConnectivity();
            }
        }
        else
        {
            if (StartPipe is not null) StartPipe.CheckConnectivity();
            else
            {
                Debug.LogWarning("起始点空,重新链接起点");
                StartPoint = gameObject.transform.parent.GetChild(LuoGameStartPoint.GetStartPointIndex()).gameObject;
                StartPipe = StartPoint.GetComponent<PipeManager>();
            }
        }
        if (isDestination)
        {
            CheckDestinationConnection();
        }
    }

    public virtual void SetState(int stateIndex)
    {
        RestConnection();
        if (isStartPoint)
        {
            CheckStartConnection();
            if (isStartPoint && isConnected)
            {
                CheckConnectivity();
            }
        }
        else
        {
            if (StartPipe is not null) StartPipe.CheckConnectivity();
            else
            {
                Debug.LogWarning("起始点空,重新链接起点");
                StartPoint = gameObject.transform.parent.GetChild(LuoGameStartPoint.GetStartPointIndex()).gameObject;
                StartPipe = StartPoint.GetComponent<PipeManager>();
            }
        }
        if (isDestination)
        {
            CheckDestinationConnection();
        }
    }

    public virtual void CheckConnectivity()
    {
        if (!isConnected && isStartPoint)
        {
            RestConnection();
            return;
        }
        
        DepthFSearch(PipeTowards.Above);
        DepthFSearch(PipeTowards.Below);
        DepthFSearch(PipeTowards.Left);
        DepthFSearch(PipeTowards.Right);
        foreach (var pipe in _reachPipeList)
        {
            var manager = pipe.GetComponent<PipeManager>();
            if (manager.isDestination && manager.destinationConnected)
            {
                LuoStaticData.Success = true;
                TalkSysStaticData.TalkSysShowText.CompleteMiniGame();
                Invoke(nameof(EndGame),1.5f);
                
            }
        }

    }

    private void EndGame()
    {
        gameObject.transform.parent.parent.parent.parent.gameObject.GetComponent<Animator>().SetTrigger("End");
    }
    
    /// <summary>
    /// 检查管道在指定方向上是否有接口。
    /// </summary>
    /// <param name="towards">传入方为基准，接受方相对的方向 例如传入方在接受方上面,则传入Below</param>
    /// <returns>如果在指定方向上有接口，则返回true；否则返回false。</returns>
    public abstract bool HaveInterface(PipeTowards towards);

    public abstract void CheckStartConnection();
    
    public abstract void CheckDestinationConnection();

    /// <summary>
    /// 使用深度优先搜索算法检查并连接管道。该方法根据传入的方向尝试链接当前管道与相邻管道，如果相邻管道存在且尚未被访问过，并且两者之间有接口可以连接，则将相邻管道标记为已连接，并继续从该相邻管道开始进行深度优先搜索。
    /// </summary>
    /// <param name="towards">指定要检查和连接的相对方向（上、下、左、右）。</param>
    private void DepthFSearch(PipeTowards towards)
    {
        GameObject tempPipe;
        PipeManager tempComponent;
        PipeTowards tempTowards,tempSelfTowards;
        switch (towards)
        {
            //传入方在接受方下面
            case PipeTowards.Above:
                tempPipe = above;
                tempComponent = AboveComponent;
                tempTowards = PipeTowards.Below;
                tempSelfTowards = PipeTowards.Above;
                break;
            //传入方在接收方上面
            case PipeTowards.Below:
                tempPipe = below;
                tempComponent = BelowComponent;
                tempTowards = PipeTowards.Above;
                tempSelfTowards = PipeTowards.Below;
                break;
            //传入方在接收方右边
            case PipeTowards.Left:
                tempPipe = left;
                tempComponent = LeftComponent;
                tempTowards = PipeTowards.Right;
                tempSelfTowards = PipeTowards.Left;
                break;
            //传入方在接收方左边
            case PipeTowards.Right:
                tempPipe = right;
                tempComponent = RightComponent;
                tempTowards = PipeTowards.Left;
                tempSelfTowards = PipeTowards.Right;
                break;
            default:
                return;
        }
        if (tempPipe is not null && !_reachPipeList.Contains(tempPipe))
        {
            if(tempComponent.HaveInterface(tempTowards) && HaveInterface(tempSelfTowards))
            {
                _reachPipeList.Add(tempPipe);
                tempComponent.SetConnect(true);
                tempComponent.CheckConnectivity();
            }
        }

        LuoStaticData.MaxReach = Mathf.Max(LuoStaticData.MaxReach, _reachPipeList.Count);
        LuoStaticData.CurrentReach = _reachPipeList.Count;
    }

    public void SwitchPipePosition(GameObject pipe,PipeTowards towards)
    {
        switch (towards)
        {
            case PipeTowards.Above:
                pipe.transform.position = upP.transform.position;
                // 欧拉角(0,0,0) → 无旋转
                pipe.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case PipeTowards.Below:
                pipe.transform.position = downP.transform.position;
                // 欧拉角(0,0,180) 等价于 -180，旋转效果一致
                pipe.transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case PipeTowards.Right:
                pipe.transform.position = rightP.transform.position;
                // Z轴顺时针旋转90度（对应-90度）
                pipe.transform.rotation = Quaternion.Euler(180, 0, -90);
                break;
            case PipeTowards.Left:
                pipe.transform.position = leftP.transform.position;
                // Z轴逆时针旋转90度
                pipe.transform.rotation = Quaternion.Euler(0, 180, 90);
                break;
        }
    }
    
    

}
