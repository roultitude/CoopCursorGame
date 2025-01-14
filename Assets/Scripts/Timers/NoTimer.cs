using UnityEngine;

public class NoTimer : TickableTimer
{
    public override bool isTimerComplete => true;

    public NoTimer() : base(0)
    {
        //nothing
    }
    public override void Tick(float amt)
    {
        //Debug.LogWarning("Tried to Tick() a NoTimer");
    }
    public override void Reset()
    {
        //Debug.LogWarning("Tried to Reset() a NoTimer");
    }
}
