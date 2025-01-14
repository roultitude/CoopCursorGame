using UnityEngine;

public class Upgrade 
{
    readonly int id;
    UpgradeSO data;
    TickableTimer timer;

    public bool HasCustomEffect() => data.customEffect != null;
    public float GetTimerFraction() => timer.GetCompletionFraction();
    public UpgradeSO GetData() { return data; }

    public CustomEffectSO GetCustomEffect()
    {
        return data.customEffect;
    }

    public Upgrade(UpgradeSO data, int id)
    {
        this.data = data;
        this.id = id;
        if (data.customEffect && data.customEffect.hasTimer) timer = new CountupTimer(0);
        else
        {
            timer = new NoTimer();
        }
    }

    public void TickTimer(float amt)
    {
        timer.Tick(amt);
    }

    /// <summary>
    /// Returns bool indicating whether to trigger timer reset (cooldown usually)
    /// </summary>
    public bool OnHitCustomEffect(Player player, Enemy enemy, ref HitInfo hit)
    {
        if (HasCustomEffect()) {
            return data.customEffect.OnHitEnemy(timer, player, enemy, ref hit);
        }
        return false;
    }

    public void ResetTimer()
    {
        if (HasCustomEffect())
        {
            data.customEffect.OnRPCTriggerTimer(timer);
        }
        else
        {
            Debug.LogWarning("Timer Reset Triggered for upgrade without customEffect! Should never occur!");
        }
    }

}
