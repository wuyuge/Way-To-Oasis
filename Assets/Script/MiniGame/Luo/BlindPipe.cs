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
        base.SetState();
    }
    public override void SetState(int state)
    {
        if (isStartPoint)
        {
            CheckStartConnection();
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
}
