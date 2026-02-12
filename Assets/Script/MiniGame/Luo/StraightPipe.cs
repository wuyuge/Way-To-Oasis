using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StraightPipe : Pipe
{
    
    public bool isVertical = true;
    
    private void SetTowards()
    {
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
            else
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
        }
    }

    public override void SetState()
    {
        
        isVertical = !isVertical;
        SetTowards();
        if (isStartPoint)
        {
            CheckStartConnection();
        }
        if (isDestination)
        {
            CheckDestinationConnection();
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
        SetTowards();
        if (isStartPoint)
        {
            CheckStartConnection();
        }
        if (isDestination)
        {
            CheckDestinationConnection();
        }
        base.SetState(state);
        
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
    
    public override void CheckDestinationConnection()
    {
        switch (endTowards)
        {
            case PipeTowards.Below:
            case PipeTowards.Above:
                Manager.destinationConnected = isVertical;
                break;
            case PipeTowards.Left:
            case PipeTowards.Right:
                Manager.destinationConnected = !isVertical;
                break;
        }
    }
}
