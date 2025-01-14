using UnityEngine;

[CreateAssetMenu(fileName = "Effect_CooldownReductionOnCrit", menuName = "Scriptable Objects/CustomEffects/Effect_CooldownReductionOnCrit")]
public class Effect_CooldownReductionOnCrit : CustomEffectSO
{
    [SerializeField]
    float reductionTime;
    [SerializeField]
    float cooldownBetweenProcs;
    public override HitInfo OnHitEnemy(TickableTimer timer, Player player, Enemy enemy, HitInfo hit)
    {
        if (hit.isCrit && timer.isTimerComplete)
        {
            player.playerAbility.ModifyCooldownTimer(-reductionTime);
            //player.customEffectsHandler.ModifyStat();
        }
        return hit;
    }

    public override void OnRPCTriggerTimer(TickableTimer timer)
    {
        timer.Set(cooldownBetweenProcs).Reset();
    }
}
