using BulletPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect_FireBulletOnComboLevelChange", menuName = "Scriptable Objects/CustomEffects/Effect_FireBulletOnComboLevelChange")]
public class Effect_FireBulletOnComboLevelChange : CustomEffectSO
{
    [SerializeField]
    EmitterProfile toFire;
    [SerializeField]
    float baseDmg;
    [SerializeField]
    float squashMult, scamperMult, squealMult;
    public override bool OnComboLevelChange(TickableTimer timer, Player player, int prev, int curr)
    {
        if(curr >= prev)
        {
            player.PlayBulletEmission(toFire, 
                baseDmg + 
                squashMult * player.stats.GetStat(PlayerStatType.Squash) +
                scamperMult * player.stats.GetStat(PlayerStatType.Scamper) +
                squealMult * player.stats.GetStat(PlayerStatType.Squeal));
        }
        return false;
    }

}
