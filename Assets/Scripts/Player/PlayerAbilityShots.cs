using BulletPro;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerAbilityShots : PlayerAbility
{
    [Header("Shots Settings")]
    [SerializeField]
    BulletEmitter bulletEmitter;
    [SerializeField, Tooltip("Base Damage dealt by the shots")] float baseShotsDamage = 5f;
    [SerializeField, Tooltip("Additional Damage Scaling based on Skill Power")] float scalingShotsDamageMult = 0.5f;
    [SerializeField, Tooltip("Modifies movespeed during slam")] float moveSpeedMult;//refactor when more things modify movespeed
    [SerializeField, Tooltip("Modified MovespeedDuration")] float moveSpeedDuration;
    [SerializeField, Tooltip("Cooldown to turn off")] float toggleCooldown;
    [SerializeField, Tooltip("Cooldown to next turn on")] float shotsCooldown;
    private Coroutine shotsCoroutine;
    protected override void Use()
    {
        if (shotsCoroutine !=null) // currently on
        {
            Debug.Log("TurningOff");
            StopCoroutine(shotsCoroutine);
            shotsCoroutine = null;
            cooldownTimer.Set(shotsCooldown); //go on full cooldown
            player.moveSpeed = player.moveSpeed / moveSpeedMult;
            Debug.Log($"toggle off: {player.moveSpeed}");
            EmitShotsRPC(false);
        } else // currently off
        {
            Debug.Log("TurningOn");
            shotsCoroutine = StartCoroutine(TriggerShots());
            cooldownTimer.Set(toggleCooldown);

        }
        
        
    }
    protected override void Update()
    {
        if (player) { transform.rotation = player.spriteRenderer.transform.rotation; }
        base.Update();

    }
    private IEnumerator TriggerShots()
    {
        player.moveSpeed = player.moveSpeed * moveSpeedMult;
        Debug.Log($"toggle on: {player.moveSpeed}");
        EmitShotsRPC(true);
        yield return new WaitForSeconds(moveSpeedDuration);
        player.moveSpeed = player.moveSpeed / moveSpeedMult;
        Debug.Log($"timed out: {player.moveSpeed}");
        EmitShotsRPC(false);
        shotsCoroutine = null;
        cooldownTimer.Set(shotsCooldown);
        cooldownTimer.Reset();
    }

    [Rpc(SendTo.Everyone)]
    private void EmitShotsRPC(bool isOn)
    {
        bulletEmitter.Stop();
        if (isOn)
        {
            bulletEmitter.Boot();
            if (IsOwner)
            {
                bulletEmitter.bullets[bulletEmitter.bullets.Count - 1].moduleParameters.SetInt("PlayerId", (int)OwnerClientId);
                bulletEmitter.bullets[bulletEmitter.bullets.Count - 1].moduleParameters.SetFloat("Damage", baseShotsDamage + scalingShotsDamageMult * player.stats.GetStat(PlayerStatType.SkillPower));
            }
        }

    }
}
    