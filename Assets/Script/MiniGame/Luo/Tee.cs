using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tee : Pipe
{
    private Toward[] Openings => curToward switch
    {
        Toward.Up    => new[] { Toward.Up,   Toward.Left,  Toward.Right },
        Toward.Right => new[] { Toward.Up,   Toward.Down,  Toward.Right },
        Toward.Down  => new[] { Toward.Down, Toward.Left,  Toward.Right },
        Toward.Left  => new[] { Toward.Up,   Toward.Down,  Toward.Left },
        _ => System.Array.Empty<Toward>()
    };

    
    private bool IsConnectedThroughWall(Toward dir)
    {
        Pipe neighbor = GetNeighbor(dir);
        return (neighbor != null
                && neighbor.isConnected
                && neighbor.HasOpening(GetOpposite(dir))) || IsEdge(dir);
    }
    
    
    public override bool HasOpening(Toward dir) => Openings.Contains(dir);

    protected override void CalculateConnection()
    {
        isConnected = Openings.Any(dir =>
        {
            Pipe neighbor = GetNeighbor(dir);
            return neighbor != null && neighbor.isConnected && neighbor.HasOpening(GetOpposite(dir));
        });
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
        Debug.Log("检查断点");
        breakTag = IsConnectedThroughWall(Openings[0]) && IsConnectedThroughWall(Openings[1]) && IsConnectedThroughWall(Openings[2]);
        return breakTag;
    }
}
