using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Straight : Pipe
{
    public override bool HasOpening(Toward dir)
    {
        return curToward switch
        {
            Toward.Up or Toward.Down  => dir == Toward.Up || dir == Toward.Down,
            Toward.Left or Toward.Right => dir == Toward.Left || dir == Toward.Right,
            _ => false
        };
    }
    
    private (Toward a, Toward b) Openings => curToward switch
    {
        Toward.Up    => (Toward.Up,   Toward.Down),
        Toward.Right => (Toward.Right,   Toward.Left),
        Toward.Down  => (Toward.Down, Toward.Up),
        Toward.Left  => (Toward.Right, Toward.Left),
        _ => default
    };
    

    protected override void CalculateConnection()
    {
        bool upConn    = up    != null && up.isConnected    && up.HasOpening(Toward.Down);
        bool downConn  = down  != null && down.isConnected  && down.HasOpening(Toward.Up);
        bool leftConn  = left  != null && left.isConnected  && left.HasOpening(Toward.Right);
        bool rightConn = right != null && right.isConnected && right.HasOpening(Toward.Left);

        isConnected = curToward switch
        {
            Toward.Up or Toward.Down  => upConn || downConn,
            Toward.Left or Toward.Right => leftConn || rightConn,
            _ => false
        };
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
        if (isStart) isConnected = true;
    }


    protected override void CheckDestination()
    {
        if (HasOpening(destinationPos))
        {
            /*Debug.Log("链接终点");*/
        }
    }

    protected override bool CheckPipeBreak()
    {
        Debug.Log("检查断点",this);
        if (IsEdge(Openings.a))
        {
            breakTag = IsConnectedThrough(Openings.b);
            return breakTag;
        }

        if (IsEdge(Openings.b))
        {
            breakTag = IsConnectedThrough(Openings.a);
            return breakTag;
        }
        
        breakTag =  IsConnectedThrough(Openings.a) && IsConnectedThrough(Openings.b);
        return breakTag;
    }
}
