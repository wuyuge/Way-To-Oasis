using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlindPipe : Pipe
{
    public override void SetState()
    {
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

    public override bool HaveInterface(PipeTowards towards)
    {
        return false;
    }

    public override void CheckStartConnection()
    {
        isConnected = false;
    }
    
    public override void CheckDestinationConnection()
    {
        Manager.SetDestinationConnect(false);
    }
}
