public class Pipe4Way : Pipe
{

    public override void SetState()
    {
        if (isStartPoint)
        {
            CheckStartConnection();
        }
        base.SetState();
    }

    public override bool HaveInterface(PipeTowards towards)
    {
        return true;
    }

    public override void CheckStartConnection()
    {
        isConnected = true;
    }
}
