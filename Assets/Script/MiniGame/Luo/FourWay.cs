using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourWay : Pipe
{
    public override void Check()
    {
        
        isConnected = (up?.isConnected ?? false) || (down?.isConnected ?? false) ||  (left?.isConnected ?? false) || (right?.isConnected ?? false); 
        base.Check();
    }
    private Toward[] Openings => curToward switch
    {
        Toward.Up    => new[] { Toward.Up,   Toward.Left,  Toward.Right , Toward.Down},
        Toward.Right => new[] { Toward.Up,   Toward.Left,  Toward.Right , Toward.Down },
        Toward.Down  => new[] { Toward.Up,   Toward.Left,  Toward.Right , Toward.Down },
        Toward.Left  => new[] { Toward.Up,   Toward.Left,  Toward.Right , Toward.Down },
        _ => System.Array.Empty<Toward>()
    };

    protected override void CheckInitial()
    {
        if (isStart)
        {
            isConnected =  true;
        }
    }

    public override bool HasOpening(Toward dir)
    {
        return true;
    }
    
    
    protected override void CheckDestination()
    {
        if (HasOpening(destinationPos))
        {
            /*Debug.Log("链接终点");*/
        }
    }
    
    private bool IsConnectedThroughWall(Toward dir)
    {
        Pipe neighbor = GetNeighbor(dir);
        return (neighbor != null
                && neighbor.isConnected
                && neighbor.HasOpening(GetOpposite(dir))) || IsEdge(dir);
    }

    protected override bool CheckPipeBreak()
    {
        Debug.Log("检查断点");
        breakTag = IsConnectedThroughWall(Openings[0]) && IsConnectedThroughWall(Openings[1]) &&  IsConnectedThroughWall(Openings[2]) &&  IsConnectedThroughWall(Openings[3]);
        return breakTag;
    }
}
