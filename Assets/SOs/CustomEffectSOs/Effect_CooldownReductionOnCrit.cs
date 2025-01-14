using UnityEngine;

[CreateAssetMenu(fileName = "Effect_CooldownReductionOnCrit", menuName = "Scriptable Objects/CustomEffects/Effect_CooldownReductionOnCrit")]
public class Effect_CooldownReductionOnCrit : CustomEffectSO
{
    [SerializeField]
    float reductionTime;
    [SerializeField]
    float cooldownBetweenProcs;
    public override bool OnHitEnemy(TickableTimer timer, Player player, Enemy enemy, ref HitInfo hit)
    {
        if (hit.isCrit && timer.isTimerComplete)
        {
            player.playerAbility.ModifyCooldownTimer(-reductionTime);
            return true;
            //player.customEffectsHandler.ModifyStat();
        }
        return false;
    }

    public override void OnRPCTriggerTimer(TickableTimer timer)
    {
        timer.Set(cooldownBetweenProcs).Reset();
    }
}
