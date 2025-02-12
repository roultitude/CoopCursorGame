using BulletPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerAbilityShockwave : PlayerAbility
{

    [Header("Shockwave Settings")]
    [SerializeField]
    BulletEmitter bulletEmitter;
    [SerializeField, Tooltip("Base Damage dealt by the shockwave")] float baseShockwaveDamage = 5f;
    [SerializeField, Tooltip("Additional Damage Scaling based on Skill Power")] float scalingShockwaveDamageMult = 0.5f;
    [SerializeField, Tooltip("Modifies movespeed during slam")] float moveSpeedMult;//refactor when more things modify movespeed
    protected override void Use()
    {
        player.moveSpeed = player.moveSpeed * moveSpeedMult;
        EmitShockwaveRPC();
        Invoke(nameof(RestoreMoveSpeed), 0.5f);
    }

    private void RestoreMoveSpeed()
    {
        player.moveSpeed = player.moveSpeed / moveSpeedMult;
    }
    [Rpc(SendTo.Everyone)]
    private void EmitShockwaveRPC()
    {
        Debug.Log("FireShockwave!");
        bulletEmitter.Kill();
        bulletEmitter.Play();
        if (IsOwner)
        {
            bulletEmitter.bullets[bulletEmitter.bullets.Count - 1].moduleParameters.SetInt("PlayerId", (int)OwnerClientId);
            bulletEmitter.bullets[bulletEmitter.bullets.Count - 1].moduleParameters.SetFloat("Damage", baseShockwaveDamage  + scalingShockwaveDamageMult * player.stats.GetStat(PlayerStatType.SkillPower));
        }

    }
}
