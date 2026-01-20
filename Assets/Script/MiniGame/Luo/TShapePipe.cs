using UnityEngine;

public class TShapePipe : Pipe
{
    public enum TShapePipeState
    {
        LeftUpRight,
        UpRightDown,
        RightDownLeft,
        DownLeftUp
        
    }
    
    public TShapePipeState state;

    public override void SetState()
    {
        switch (state)
        {
            case TShapePipeState.LeftUpRight:
                state = TShapePipeState.UpRightDown;
                break;
            case TShapePipeState.UpRightDown:
                state = TShapePipeState.RightDownLeft;
                break;
            case TShapePipeState.RightDownLeft:
                state = TShapePipeState.DownLeftUp;
                break;
            case TShapePipeState.DownLeftUp:
                state = TShapePipeState.LeftUpRight;
                break;
        }
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
    
    public override void SetState(int stateIndex)
    {
        switch (stateIndex)
        {
            case 1:
                state = TShapePipeState.UpRightDown;
                break;
            case 2:
                state = TShapePipeState.RightDownLeft;
                break;
            case 3:
                state = TShapePipeState.DownLeftUp;
                break;
            case 0:
                state = TShapePipeState.LeftUpRight;
                break;
        }
        if (isStartPoint)
        {
            CheckStartConnection();
        }
        if (isDestination)
        {
            CheckDestinationConnection();
        }
        base.SetState(stateIndex);
    }


    public override bool HaveInterface(PipeTowards towards)
    {
        switch (towards)
        {
            case PipeTowards.Above:
                return state is TShapePipeState.DownLeftUp or TShapePipeState.LeftUpRight
                    or TShapePipeState.UpRightDown;
            case PipeTowards.Below:
                return state is TShapePipeState.DownLeftUp or TShapePipeState.RightDownLeft 
                    or TShapePipeState.UpRightDown;
            case PipeTowards.Left:
                return state is TShapePipeState.DownLeftUp or TShapePipeState.LeftUpRight 
                    or TShapePipeState.RightDownLeft;
            case PipeTowards.Right:
                return state is TShapePipeState.RightDownLeft or TShapePipeState.LeftUpRight 
                    or TShapePipeState.UpRightDown;
            default:
                return false;
            
        }
    }

    public override void CheckStartConnection()
    {
        switch (startTowards)
        {
            case PipeTowards.Above:
                isConnected = state is TShapePipeState.DownLeftUp or TShapePipeState.LeftUpRight
                    or TShapePipeState.UpRightDown;
                return;
            case PipeTowards.Below:
                isConnected = state is TShapePipeState.DownLeftUp or TShapePipeState.RightDownLeft 
                    or TShapePipeState.UpRightDown;
                break;
            case PipeTowards.Left:
                isConnected = state is TShapePipeState.DownLeftUp or TShapePipeState.LeftUpRight 
                    or TShapePipeState.RightDownLeft;
                break;
            case PipeTowards.Right:
                isConnected = state is TShapePipeState.RightDownLeft or TShapePipeState.LeftUpRight 
                    or TShapePipeState.UpRightDown;
                break;
        }
    }
    
    public override void CheckDestinationConnection()
    {
        switch (endTowards)
        {
            case PipeTowards.Above:
                Manager.destinationConnected = state is TShapePipeState.DownLeftUp or TShapePipeState.LeftUpRight
                    or TShapePipeState.UpRightDown;
                return;
            case PipeTowards.Below:
                Manager.destinationConnected = state is TShapePipeState.DownLeftUp or TShapePipeState.RightDownLeft 
                    or TShapePipeState.UpRightDown;
                break;
            case PipeTowards.Left:
                Manager.destinationConnected = state is TShapePipeState.DownLeftUp or TShapePipeState.LeftUpRight 
                    or TShapePipeState.RightDownLeft;
                break;
            case PipeTowards.Right:
                Manager.destinationConnected = state is TShapePipeState.RightDownLeft or TShapePipeState.LeftUpRight 
                    or TShapePipeState.UpRightDown;
                break;
        }
    }
}
