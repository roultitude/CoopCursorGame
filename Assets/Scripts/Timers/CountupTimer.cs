using UnityEngine;

public class CountupTimer : TickableTimer
{
    public override bool isTimerComplete => true;

    public CountupTimer(float timer) : base(timer)
    {
        //use base
    }

    // no need to override for now?
}
