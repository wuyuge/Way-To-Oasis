using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StraightPipe : Pipe
{
    
    public bool isVertical = true;
    


    public override void SetState()
    {
        
        isVertical = !isVertical;
        if (isStartPoint)
        {
            CheckStartConnection();
        }
        base.SetState();
        
    } 
    public override void SetState(int state)
    {

        switch (state)
        {
            case 2:
            case 0:
                isVertical = true;
                break;
            case 3:
            case 1:
                isVertical = false; 
                break;
        }
        if (isStartPoint)
        {
            CheckStartConnection();
        }
        base.SetState();
        
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

    public override void CheckStartConnection()
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
