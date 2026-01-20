public class Pipe4Way : Pipe
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
    public override void SetState(int stateIndex)
    {
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
        return true;
    }

    public override void CheckStartConnection()
    {
        isConnected = true;
    }

    public override void CheckDestinationConnection()
    {
        Manager.destinationConnected = true;
    }
}
