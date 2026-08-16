using UnityEngine;

public class Angle : Pipe
{
    // 两个开口方向
    private (Toward a, Toward b) Openings => curToward switch
    {
        Toward.Up    => (Toward.Up,   Toward.Left),
        Toward.Right => (Toward.Up,   Toward.Right),
        Toward.Down  => (Toward.Down, Toward.Right),
        Toward.Left  => (Toward.Down, Toward.Left),
        _ => default
    };

    public override bool HasOpening(Toward dir)
    {
        var (a, b) = Openings;
        return dir == a || dir == b;
    }

    protected override void CalculateConnection()
    {
        var (a, b) = Openings;
        isConnected = IsConnectedThrough(a) || IsConnectedThrough(b);
    }

    // 核心：双向校验 —— 我有开口 + 邻居存在 + 邻居连通 + 邻居也对着我有开口
    private bool IsConnectedThrough(Toward dir)
    {
        Pipe neighbor = GetNeighbor(dir);
        return neighbor != null 
               && neighbor.isConnected 
               && neighbor.HasOpening(GetOpposite(dir));
    }


    private bool IsConnectedWall(Toward dir)
    {
        if (dir == GetEdge().a || dir == GetEdge().b)
        {
            return true;
        }

        return false;
    }
    

    protected override void CheckInitial()
    {
        if (isStart)
        {
            var (a, b) = Openings;
            isConnected = startPos == a || startPos == b;
        }
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

        breakTag = IsConnectedThrough(Openings.a) && IsConnectedThrough(Openings.b);
        return breakTag;
        

    }
}