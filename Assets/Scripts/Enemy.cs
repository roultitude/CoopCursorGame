using System;
using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    [SerializeField]
    protected float moveSpeed, rotationSpeed;
    [SerializeField]
    protected float maxRotAngle = 10f;

    [SerializeField]
    private EnemyMovementType movementType;

    [SerializeField]
    private int baseHealth;

    [SerializeField]
    private NetworkVariable<float> health;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private bool isFacingPlayer;

    [SerializeField]
    private SpriteRenderer sprite;

    [SerializeField]
    private float roamChangeInterval = 2f; // Time between direction changes

    private Vector2 roamDirection;
    private float roamChangeTimer = 0f;

    private Rigidbody2D rb;
    private EnemySpawner spawner;
    private Boss bossParent;
    public bool isVulnerable = true;

  

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    [Rpc(SendTo.Everyone)]
    public void SetupRPC(NetworkBehaviourReference enemySpawnerRef)
    {
        if (enemySpawnerRef.TryGet(out EnemySpawner enemySpawner))
        {
           
            spawner = enemySpawner;
            spawner.TrackEnemy(true, this);
        }
        if(enemySpawnerRef.TryGet(out Boss boss))
        {
            Debug.Log($"Spawned enemy, parent: {boss.name}");
            bossParent = boss;
            boss.TrackPart(true, this);
        }
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
    protected virtual void FixedUpdate()
    {
        MoveAndRotate(); //curently running on both client and server
    }

    public void Rotate(Vector2 faceDir)
    {
        sprite.flipY = faceDir.x < 0;
        float deg = Mathf.Atan2(faceDir.y , faceDir.x) * Mathf.Rad2Deg;
        if (deg >= maxRotAngle && deg <= 90) deg = maxRotAngle;
        else if (deg >= 90 && deg <= 180-maxRotAngle) deg = 180-maxRotAngle;
        else if (deg >= maxRotAngle - 180 && deg <= -90) deg = maxRotAngle - 180;
        else if (deg <= -maxRotAngle && deg >= -90) deg = -maxRotAngle;
        rb.SetRotation(Mathf.LerpAngle(rb.rotation, deg ,Time.deltaTime * rotationSpeed));
    }

    private void OnHealthChange(float prev, float curr)
    {
        Debug.Log($"{NetworkObjectId} Enemy health: {curr}");
        if(prev > curr)
        {
            DynamicTextManager.CreateText2D(transform.position, $"{(curr-prev)}", DynamicTextManager.defaultData);
            animator.CrossFade("OnHurtEnemy", 0);
        }
    }

    public virtual void OnHurt(float num)
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
        if (spawner)
        {
            spawner.TrackEnemy(false, this);
        }
        if (bossParent)
        {
            bossParent.TrackPart(false, this);
        }

    }


    private void MoveAndRotate()
    {
        Vector2 dir = new Vector2(0,0);
        switch (movementType)
        {
            case EnemyMovementType.TowardNearestPlayer:
                //Vector2 vecToPlayer = FindClosestPlayer().vecToPlayer;
                //dir = vecToPlayer.normalized;
                //if (isFacingPlayer)
                //{
                //    Rotate(dir);
                //}
                break;
            case EnemyMovementType.RandomRoam:
                roamChangeTimer += Time.fixedDeltaTime;
                if(roamChangeTimer > roamChangeInterval)
                {
                    roamChangeTimer = 0;
                    roamDirection = UnityEngine.Random.insideUnitCircle.normalized;
                }
                dir = roamDirection; //TODO: STOP THEM FROM ROAMING OFF SCREEN
                break;

            case EnemyMovementType.Stationary:
                return;
                
            default:
                Debug.LogError("Invalid Enemy Movement Type!");
                return;
        }
        if (isFacingPlayer)
        {
            //Debug.Log(rb.rotation);
            //Debug.Log(new Vector3(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad)));
            transform.position += new Vector3(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad)) * moveSpeed * Time.fixedDeltaTime;
        }
        else
        {
            transform.position += (Vector3)dir * moveSpeed * Time.fixedDeltaTime;
        }
    }




    public void ChangeVulnerable(bool isVuln)
    {
        isVulnerable = isVuln;
    }

    public enum EnemyMovementType
    {
        TowardNearestPlayer, RandomRoam, Stationary
    }
}
