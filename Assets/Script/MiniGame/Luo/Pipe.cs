using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class Pipe : MonoBehaviour
{
    public bool isConnected;
    public bool isStartPoint,isDestination;
    [SerializeField]
    protected GameObject above,below,left,right;
    public int pipeNumber;
    protected Coroutine CurrentCoroutine;
    protected Pipe AboveComponent, LeftComponent,BelowComponent,RightComponent;
    private Animator _animator;
    protected Image ObjectImage;
    protected PipeSearchList SearchList;
    public PipeTowards startTowards;
    protected static List<GameObject> ReachPipeList = new List<GameObject>();
    protected static GameObject StartPoint;
    protected static Pipe StartPipe;
    private void Awake()
    {
        ObjectImage = GetComponent<Image>();
        SearchList = transform.parent.gameObject.GetComponent<PipeSearchList>();
        pipeNumber = transform.GetSiblingIndex();
        above = pipeNumber - 4 >= 0  ? transform.parent.GetChild(pipeNumber - 4).gameObject : null;
        below = pipeNumber + 4 < transform.parent.childCount ? gameObject.transform.parent.GetChild(pipeNumber + 4).gameObject : null;
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
                AboveComponent = above.GetComponent<Pipe>();
                
            }
            if (below is not null)
            {
                BelowComponent = below.GetComponent<Pipe>();
                
            }
            if (left is not null)
            {
                LeftComponent = left.GetComponent<Pipe>();
                
            }
            if (right is not null)
            {
                RightComponent = right.GetComponent<Pipe>();
                
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e,this);
            throw;
        }
        
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 初始化管道组件。如果该管道是起点，则将其颜色设置为蓝色，并开始尝试链接其他管道形成通路；如果该管道是终点，则将其颜色设置为红色。
    /// </summary>
    public virtual void Start()
    {
        if (isStartPoint)
        {
            ObjectImage.color = Color.blue;
            StartPoint = gameObject;
            StartPipe = StartPoint.GetComponent<Pipe>();
        }   
        if (isDestination)
        {
            ObjectImage.color = Color.red;
        }
        
    }
    
    
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
                AboveComponent = above.GetComponent<Pipe>();
                break;
            case PipeTowards.Below:
                BelowComponent = below.GetComponent<Pipe>();
                break;
            case PipeTowards.Right:
                RightComponent = right.GetComponent<Pipe>();
                break;
            case PipeTowards.Left:
                LeftComponent = left.GetComponent<Pipe>();
                break;
        }
    }

    public void RestConnection()
    {
        for (int index = 0; index < transform.parent.childCount; index++)
        {
            Pipe tempPipe = transform.parent.GetChild(index).gameObject.GetComponent<Pipe>();
            if (!tempPipe.isStartPoint)
            {
                tempPipe.isConnected = false;
            }
            
        }
        ReachPipeList.Clear();
    }

    public virtual void SetState()
    {
        RestConnection();
        if (isStartPoint && isConnected)
        {
            CheckConnectivity();
        }
        else
        {
            StartPoint.GetComponent<Pipe>().CheckConnectivity();
        }
    }

    public virtual void CheckConnectivity()
    {
        if (!isConnected && isStartPoint) return;
        DepthFSearch(PipeTowards.Above);
        DepthFSearch(PipeTowards.Below);
        DepthFSearch(PipeTowards.Left);
        DepthFSearch(PipeTowards.Right);

    }

    public abstract bool HaveInterface(PipeTowards towards);

    private void DepthFSearch(PipeTowards towards)
    {
        GameObject tempPipe;
        Pipe tempComponent;
        PipeTowards tempTowards,tempSelfTowards;
        switch (towards)
        {
            case PipeTowards.Above:
                tempPipe = above;
                tempComponent = AboveComponent;
                tempTowards = PipeTowards.Below;
                tempSelfTowards = PipeTowards.Above;
                break;
            case PipeTowards.Below:
                tempPipe = below;
                tempComponent = BelowComponent;
                tempTowards = PipeTowards.Above;
                tempSelfTowards = PipeTowards.Below;
                break;
            case PipeTowards.Left:
                tempPipe = left;
                tempComponent = LeftComponent;
                tempTowards = PipeTowards.Right;
                tempSelfTowards = PipeTowards.Left;
                break;
            case PipeTowards.Right:
                tempPipe = right;
                tempComponent = RightComponent;
                tempTowards = PipeTowards.Left;
                tempSelfTowards = PipeTowards.Right;
                break;
            default:
                return;
        }
        if (tempPipe is not null && !ReachPipeList.Contains(tempPipe))
        {
            if(RightComponent.HaveInterface(tempTowards) && HaveInterface(tempSelfTowards))
            {
                ReachPipeList.Add(tempPipe);
                tempComponent.isConnected = true;
                tempComponent.CheckConnectivity();
            }
        }
    }
    

}
