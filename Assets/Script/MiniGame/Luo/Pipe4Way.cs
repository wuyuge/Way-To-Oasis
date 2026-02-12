public class Pipe4Way : Pipe
{

    private void SetTowards()
    {
        if (isStartPoint)
        {
            if (startIsVertical)
            {
                if (above is null)
                {
                    startTowards = PipeTowards.Above;
                }
                else if (below is null)
                {
                    startTowards = PipeTowards.Below;
                }

            }
            else
            {
                if (left is null)
                {
                    startTowards = PipeTowards.Left;
                }

                if (right is null)
                {
                    startTowards = PipeTowards.Right;
                }
            }
        }

        if (isDestination)
        {
            if (destinationIsVertical)
            {
                if (above is null)
                {
                    endTowards = PipeTowards.Above;
                }
                else if (below is null)
                {
                    endTowards = PipeTowards.Below;
                }

            }
            else
            {
                if (left is null)
                {
                    endTowards = PipeTowards.Left;
                }

                if (right is null)
                {
                    endTowards = PipeTowards.Right;
                }
            }
        }
    }
    public override void SetState()
    {
        SetTowards();
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
        SetTowards();
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
