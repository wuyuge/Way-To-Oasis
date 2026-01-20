using UnityEngine;

public class AnglePipe : Pipe
{
    public enum AnglePipeState
    {
        LeftAndUp,
        RightAndUp,
        RightAndDown,
        LeftAndDown,
        
    }
    
    public AnglePipeState state;

    public override void SetState()
    {
        switch (state)
        {
            case AnglePipeState.LeftAndUp:
                state = AnglePipeState.RightAndUp;
                break;
            case AnglePipeState.RightAndUp:
                state = AnglePipeState.RightAndDown;
                break;
            case AnglePipeState.RightAndDown:
                state = AnglePipeState.LeftAndDown;
                break;
            case AnglePipeState.LeftAndDown:
                state = AnglePipeState.LeftAndUp;
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
                state = AnglePipeState.RightAndUp;
                break;
            case 2:
                state = AnglePipeState.RightAndDown;
                break;
            case 3:
                state = AnglePipeState.LeftAndDown;
                break;
            case 0:
                state = AnglePipeState.LeftAndUp;
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
                return state is AnglePipeState.LeftAndUp or AnglePipeState.RightAndUp;
            case PipeTowards.Below:
                return state is AnglePipeState.LeftAndDown or AnglePipeState.RightAndDown;
            case PipeTowards.Left:
                return state is AnglePipeState.LeftAndUp or AnglePipeState.LeftAndDown;
            case PipeTowards.Right:
                return state is AnglePipeState.RightAndDown or AnglePipeState.RightAndUp;
            default:
                return false;
        }
    }

    public override void CheckStartConnection()
    {
        switch (startTowards)
        {
            case PipeTowards.Above:
                isConnected = state is AnglePipeState.RightAndUp or  AnglePipeState.LeftAndUp;
                return;
            case PipeTowards.Below:
                isConnected = state is AnglePipeState.LeftAndDown or AnglePipeState.RightAndDown;
                break;
            case PipeTowards.Left:
                isConnected = state is AnglePipeState.LeftAndUp or AnglePipeState.LeftAndDown;
                break;
            case PipeTowards.Right:
                isConnected = state is AnglePipeState.RightAndUp or AnglePipeState.RightAndDown;
                break;
        }
    }
    
    public override void CheckDestinationConnection()
    {
        switch (endTowards)
        {
            case PipeTowards.Above:
                Manager.destinationConnected = state is AnglePipeState.RightAndUp or  AnglePipeState.LeftAndUp;
                return;
            case PipeTowards.Below:
                Manager.destinationConnected = state is AnglePipeState.LeftAndDown or AnglePipeState.RightAndDown;
                break;
            case PipeTowards.Left:
                Manager.destinationConnected = state is AnglePipeState.LeftAndUp or AnglePipeState.LeftAndDown;
                break;
            case PipeTowards.Right:
                Manager.destinationConnected = state is AnglePipeState.RightAndUp or AnglePipeState.RightAndDown;
                break;
        }
    }
}
