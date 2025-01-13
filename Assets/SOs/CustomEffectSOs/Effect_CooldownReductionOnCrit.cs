using UnityEngine;

[CreateAssetMenu(fileName = "Effect_CooldownReductionOnCrit", menuName = "Scriptable Objects/CustomEffects/Effect_CooldownReductionOnCrit")]
public class Effect_CooldownReductionOnCrit : CustomEffectSO
{
    [SerializeField]
    float reductionTime;

    public override void OnHitEnemy(Player player, Enemy enemy, bool isCrit)
    {
        base.OnHitEnemy(player, enemy, isCrit);
        if (isCrit)
        {
            player.playerAbility.ModifyCooldownTimer(-reductionTime);
            //player.customEffectsHandler.ModifyStat();
        }
    }
}
