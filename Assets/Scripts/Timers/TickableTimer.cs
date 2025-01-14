using UnityEngine;

public abstract class TickableTimer
{
    protected float timerLength;
    protected float timerTime;
    public virtual bool isTimerComplete => timerTime >= timerLength;
    public TickableTimer(float timerLength)
    {
        this.timerLength = timerLength;
        timerTime = 0;
    }

    public virtual TickableTimer Set(float timerLength)
    {
        this.timerLength = timerLength;
        return this;
    }

    public virtual void Reset()
    {
        timerTime = 0;
    }
    public virtual void Tick(float amt)
    {
        timerTime += amt;
    }
}
