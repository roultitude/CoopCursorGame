using UnityEngine;

public class Upgrade 
{
    readonly int id;
    UpgradeSO data;
    TickableTimer timer;

    public bool HasCustomEffect() => data.customEffect != null;
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

    public HitInfo OnHitCustomEffect(Player player, Enemy enemy, HitInfo hit)
    {
        if (HasCustomEffect()) {
            return data.customEffect.OnHitEnemy(timer, player, enemy, hit);
        }
        return hit;
    }
}
