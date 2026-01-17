using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StraightPipe : Pipe
{
    public bool startIsVertical;
    public bool isVertical = true;
    private Coroutine _coroutine;

    public override void Start()
    {
        base.Start();
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
    }

    private void Update()
    {
        if (isStartPoint)
        {
            CheckStartConnection();
        }
        ObjectImage.color = isConnected ? Color.green : Color.red;
    }


    public override void SetState()
    {
        
        isVertical = !isVertical;
        if (isStartPoint)
        {
            CheckStartConnection();
        }
        base.SetState();
        
    }

    public override void CheckConnectivity()
    {
        base.CheckConnectivity();
        
    }

    public override bool HaveInterface(PipeTowards towards)
    {
        switch (towards)
        {
            case PipeTowards.Above:
            case PipeTowards.Below:
                return isVertical;
            case PipeTowards.Left:
            case PipeTowards.Right:
                return !isVertical;
            default:
                return false;
        }
    }

    private void CheckStartConnection()
    {
        switch (startTowards)
        {
            case PipeTowards.Below:
            case PipeTowards.Above:
                isConnected = isVertical;
                break;
            case PipeTowards.Left:
            case PipeTowards.Right:
                isConnected = !isVertical;
                break;
        }
    }
    
    
}
