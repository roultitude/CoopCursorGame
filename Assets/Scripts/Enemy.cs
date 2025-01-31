using BulletPro;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    [SerializeField]
    protected float rotationSpeed;
    [SerializeField]
    protected float maxRotAngle = 10f;

    [SerializeField]
    private int baseHealth;

    [SerializeField]
    private NetworkVariable<float> health;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private bool isFacingPlayer;

    [SerializeField]
    public SpriteRenderer sprite;

    private Rigidbody2D rb;
    private EnemySpawner spawner;
    public Boss bossParent;
    public bool isVulnerable = true;
    public BulletEmitter[] bulletEmitters;
  

    protected virtual void Awake()
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
        bulletEmitters = GetComponentsInChildren<BulletEmitter>();
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
        //MoveAndRotate(); //curently running on both client and server
    }

    public void Rotate(Vector2 faceDir) // MOVE
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
            
            animator.CrossFade("OnHurtEnemy", 0);
        }
    }


    public virtual void TakeDamage(float num)
    {
        TakeDamage(num,transform.position);
    }

    public virtual void TakeDamage(float num, Vector2 pos)
    {
        NetworkedDynamicTextManager.Instance.CreateText2DSynced(pos, $"{-num}");
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

    public void OnHitBullet(Bullet bullet, Vector3 loc) //called by the bulletreceiver on enemy gameobject
    {
        Debug.Log(bullet.moduleParameters.GetInt("PlayerId"));
        if(bullet.moduleParameters.GetInt("PlayerId") == (int) NetworkManager.LocalClientId) //local bullet
        {
            TakeDamage(bullet.moduleParameters.GetFloat("Damage"), loc);
        }
    }

    public float GetHealthFraction() //change to getStat?
    {
        return health.Value/baseHealth;
    }


    [Rpc(SendTo.Everyone)]
    private void OnDeathRPC(float lastHpChangeAmt)
    {
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

    public void ChangeVulnerable(bool isVuln)
    {
        isVulnerable = isVuln;
    }

    public void TriggerEmitter(BulletEmitter emitter, bool isOn = true)
    {
        TriggerEmitterRPC(emitterToIndex(emitter), isOn);
    }

    [Rpc(SendTo.Everyone)]
    public void TriggerEmitterRPC(int emitterIndex, bool isOn)
    {
        BulletEmitter emitter = indexToEmitter(emitterIndex);
        if (isOn)
        {
            emitter.Boot();
        } else
        {
            emitter.Stop();
        }
        
    }

    private BulletEmitter indexToEmitter(int index)
    {
        if (bulletEmitters.Length <= index)
        {
            Debug.LogError("Boss: Invalid index for bulletEmitter!");
            return null;
        }
        return bulletEmitters[index];
    }

    private int emitterToIndex(BulletEmitter emitter)
    {
        for (int i = 0; i < bulletEmitters.Length; i++)
        {
            if (bulletEmitters[i] == emitter)
            {
                return i;
            }
        }
        Debug.LogError("Cannot index an emitter not on Boss object!");
        return -1;
    }

}
