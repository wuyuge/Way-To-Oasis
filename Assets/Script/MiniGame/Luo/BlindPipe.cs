using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlindPipe : Pipe
{
    public override void SetState()
    {
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

    private void OnEnable()
    {
        GetComponent<Button>().enabled = false;
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
