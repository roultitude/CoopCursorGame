using UnityEngine;

[CreateAssetMenu(fileName = "Effect_IncreasedDamageBelowHPThreshold", menuName = "Scriptable Objects/CustomEffects/Effect_IncreasedDamageBelowHPThreshold")]
public class Effect_IncreasedDamageBelowHPThreshold : CustomEffectSO
{
    [SerializeField]
    float hpThreshold;
    [SerializeField]
    bool forUnderThreshold; //else for overThreshold;
    [SerializeField]
    float dmgMult;
    [SerializeField]
    bool applyOnCritOnly;

    public override bool OnHitEnemy(TickableTimer timer, Player player, Enemy enemy, ref HitInfo hit)
    {
        float hpFrac = player.GetHpFraction();
        if ((forUnderThreshold && hpFrac > hpThreshold) || (!forUnderThreshold && hpFrac < hpThreshold)) return false; // if hp criteria not fulfilled return;

        if (applyOnCritOnly)
        {
            if(hit.isCrit) hit.MutateDamageMult(dmgMult);
        } else
        {
            hit.MutateDamageMult(dmgMult);
        }

        return false;
    }
}
