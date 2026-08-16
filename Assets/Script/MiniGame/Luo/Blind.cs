using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blind : Pipe
{
    public override void Check()
    {
        isConnected = IsConnectedThrough(curToward);
        base.Check();
        
    }
    
    private bool IsConnectedThrough(Toward dir)
    {
        Pipe neighbor = GetNeighbor(dir);
        return neighbor != null 
               && neighbor.isConnected 
               && neighbor.HasOpening(GetOpposite(dir));
    }

    protected override void CheckInitial()
    {
        
    }


    public override bool HasOpening(Toward dir)
    {
        return curToward == dir;
    }

    protected override void CheckDestination()
    {
        
    }

    protected override bool CheckPipeBreak()
    {
        Debug.Log("检查断点");
        breakTag = true;
        return true;
    }
}
