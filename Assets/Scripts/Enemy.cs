using System;
using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    [SerializeField]
    private float moveSpeed, rotationSpeed;

    [SerializeField]
    private int baseHealth;

    [SerializeField]
    private NetworkVariable<float> health;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private bool isFacingPlayer;

    private Rigidbody2D rb;
    public bool isVulnerable = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer) health.Value = baseHealth;
        health.OnValueChanged += OnHealthChange;
    }

    public override void OnNetworkDespawn()
    {
        health.OnValueChanged -= OnHealthChange;
        base.OnNetworkDespawn();
    }
    private void FixedUpdate()
    {
        Vector2 vecToPlayer = FindClosestPlayer().vecToPlayer;
        Vector2 moveDir = vecToPlayer.normalized;
        Move(moveDir);
        if(isFacingPlayer)
        {
            Rotate(moveDir);
        }
    }

    private void Rotate(Vector2 faceDir)
    {
        float rad = Mathf.Atan2(faceDir.y , faceDir.x);
        rb.SetRotation(Mathf.LerpAngle(rb.rotation, rad * Mathf.Rad2Deg,Time.fixedDeltaTime * rotationSpeed));
    }

    private void OnHealthChange(float prev, float curr)
    {
        Debug.Log($"{NetworkObjectId} Enemy health: {curr}");
        if(prev > curr)
        {
            DynamicTextManager.CreateText2D(transform.position, $"{(curr-prev)}", DynamicTextManager.defaultData);
            animator.CrossFade("OnHitEnemy", 0);
        }
    }

    public void OnDamage(float num)
    {
        ChangeHealthRPC(-num);
    }

    [Rpc(SendTo.Server)]
    private void ChangeHealthRPC(float amt)
    {
        if (amt < 0 && !isVulnerable) return;

        if (health.Value + amt <= 0) //network variable cannot sync on same frame as despawn, so pass the last hit in onDeath.
        {
            OnDeathRPC(amt);
            NetworkObject.Despawn();
            return;
        }
        health.Value += amt;

    }

    [Rpc(SendTo.Everyone)]
    private void OnDeathRPC(float lastHpChangeAmt)
    {
        DynamicTextManager.CreateText2D(transform.position, $"{lastHpChangeAmt}", DynamicTextManager.defaultData);
        Debug.Log($"{NetworkObjectId} Enemy DeathRPC");
    }


    private void Move(Vector2 dir)
    {
        if(isFacingPlayer)
        {
            //Debug.Log(rb.rotation);
            //Debug.Log(new Vector3(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad)));
            transform.position += new Vector3(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad))* moveSpeed * Time.fixedDeltaTime;
        } else
        {
            transform.position += (Vector3)dir * moveSpeed * Time.fixedDeltaTime;
        }
        
    }

    private (Player closestPlayer, Vector2 vecToPlayer) FindClosestPlayer()
    {
        Player closestPlayer = null;
        Vector2 closestVec = Vector2.zero;
        foreach (Player player in PlayerManager.Instance.players)
        {
            if (player.health.Value == 0) continue;
            if (!closestPlayer) // if first alive player
            {
                closestPlayer = player;
                closestVec = player.transform.position - transform.position;
            }
            else //check against current closest player
            {
                Vector2 relPos = (player.transform.position - transform.position);
                if (relPos.sqrMagnitude < closestVec.sqrMagnitude) //new closest
                {
                    closestPlayer = player;
                    closestVec = relPos;
                }
            }
        }

        if (!closestPlayer) // if all players dead
        {
            Debug.Log("enemy cannot find alive player");
        }
        return (closestPlayer, closestVec);
    }


    public void ChangeVulnerable(bool isVuln)
    {
        isVulnerable = isVuln;
    }
}
