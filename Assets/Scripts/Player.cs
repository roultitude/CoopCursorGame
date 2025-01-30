using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using BulletPro;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class Player : NetworkBehaviour
{
    public NetworkVariable<int> health = new NetworkVariable<int>(3,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public bool isVulnerable = true;
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false,
    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> reviveTimer = new NetworkVariable<float>(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [HideInInspector]
    public NetworkVariable<NetworkBehaviourReference> playerAbilityRef = new NetworkVariable<NetworkBehaviourReference>(); //create an interface for this??? rmb to assign somehow
    public PlayerStats stats = new PlayerStats();
    public PlayerUpgrades upgrades;
    public PlayerAbility playerAbility;
    public PlayerComboManager playerCombo;
    public BulletEmitter bulletEmitter;
    public Color color;
    public SpriteRenderer spriteRenderer;

    public float GetHpFraction() => Mathf.Clamp(health.Value / stats.GetStat(PlayerStatType.MaxHealth),0,1);
    public float moveSpeed = 0.25f; //refactor into a stat maybe when more things modify it

    [SerializeField]
    private PlayerAbility playerAbilityPrefab, testPlayerAbilityPrefab;
    [SerializeField]
    float hurtInvulnTime, reviveTime;
    
    [SerializeField]
    Canvas reviveCanvas;
    [SerializeField]
    Image reviveImage;
    [SerializeField]
    Sprite aliveSprite, deadSprite;
    [SerializeField]
    public Collider2D playerCollider, reviveCollider;
    [SerializeField]
    Animator animator;
    [SerializeField]
    BulletReceiver bulletReceiver;
    private Vector2 targetPos;

    private bool canMove = true;
    private Coroutine InvulnVisualCoroutine;
    private float[] playerBounds = new float[4];

    public override void OnNetworkSpawn()
    {
        Debug.Log($"OnNetworkSpawn Player {OwnerClientId}");
        base.OnNetworkSpawn();
        playerAbilityRef.OnValueChanged += OnPlayerAbilityChanged;
        health.OnValueChanged += OnHealthChanged;
        isDead.OnValueChanged += OnDeathStatusChanged;
        

        //Setup Color
        Random.InitState((int)OwnerClientId+2);
        color = Random.ColorHSV();
        spriteRenderer.color = color;

        if (IsServer)
        {
            PlayerAbility abl;
            if (IsOwner) {
                abl = Instantiate(playerAbilityPrefab, transform.position, Quaternion.identity);
            } else
            {
                abl = Instantiate(testPlayerAbilityPrefab, transform.position, Quaternion.identity);
            }
            
            abl.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId, false);
            playerAbilityRef.Value = new NetworkBehaviourReference(abl);
            PlayerManager.Instance.AddPlayer(this); //done in OnSynchronizeComplete for clientside (UI needs refs to playerability)
        }
        else //client init
        {
            if(playerAbilityRef.Value.TryGet(out PlayerAbility abl)) // if PlayerAbility is spawned in (ready for setup), complete setup 
            {
                OnPlayerAbilityChanged(playerAbility, playerAbilityRef.Value);
                PlayerManager.Instance.AddPlayer(this);
            }
            else // wait till all NOs load in
            {
                NetworkManager.SceneManager.OnSynchronizeComplete += OnSynchronizeComplete; // synchronize all existing players (not local)
            }
        }
        if (IsOwner)
        {
            Vector2 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
            Vector2 topRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            playerBounds[0] = bottomLeft.x;
            playerBounds[2] = bottomLeft.y;
            playerBounds[1] = topRight.x;
            playerBounds[3] = topRight.y;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false; // Hide the real cursor
            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

    }

    private void OnSynchronizeComplete(ulong clientId) // have to ensure that all NOs are loaded in clientside (ability NO esp)
    {
        NetworkManager.SceneManager.OnSynchronizeComplete -= OnSynchronizeComplete;
        OnPlayerAbilityChanged(playerAbility, playerAbilityRef.Value);
        PlayerManager.Instance.AddPlayer(this);
    }

    private void OnPlayerAbilityChanged(NetworkBehaviourReference previousValue, NetworkBehaviourReference newValue)
    {
        Debug.Log("OnPlayerAblChanged");
        if(newValue.TryGet(out PlayerAbility abl))
        {
            playerAbility = abl;
            abl.Setup(this, color);
        } else
        {
            Debug.LogError("Invalid Player ability reference!");
        }
        
    }

    public override void OnNetworkDespawn()
    {
        health.OnValueChanged -= OnHealthChanged;
        isDead.OnValueChanged -= OnDeathStatusChanged;
        NetworkManager.SceneManager.OnSynchronizeComplete -= OnSynchronizeComplete;
        playerAbilityRef.OnValueChanged -= OnPlayerAbilityChanged;
        if (IsServer)
        {
            playerAbility.GetComponent<NetworkObject>().Despawn();
        }
        PlayerManager.Instance.RemovePlayer(this);
        Debug.Log($"Cleaning Up Player {OwnerClientId} obj");
        Destroy(gameObject);
    }

    void Update()
    {
        if (!canMove || !IsOwner || !IsSpawned) return;
        Move();
    }

    private void Move()
    {
        /*
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        transform.position = mouseWorldPos;
        */

        // Get mouse movement delta LOOK TO CHANGE TO NEW INPUT SYSTEM EVENTUALLY??
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        targetPos = new Vector2(
            Mathf.Clamp(targetPos.x + mouseX * moveSpeed, playerBounds[0], playerBounds[1]), 
            Mathf.Clamp(targetPos.y + mouseY * moveSpeed, playerBounds[2], playerBounds[3]));
        

        // Calculate the new position based on the mouse delta
        Vector3 newPosition =
                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime*20);

        // Apply the new position to the game object
        transform.position = newPosition;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!other.CompareTag("Enemy"))
        {
            return;
        }
        OnHurt();
    }

    private void OnHealthChanged(int prev, int curr)
    {

        if(curr == 0)
        {
            return; //let death networkvar handle
        }
        else if(prev > curr)
        {
            TriggerHurtVisual();
            //InvulnVisualCoroutine = StartCoroutine(HitInvulnVisual());
        }
    }

    private void OnDeathStatusChanged(bool prev, bool curr)
    {
        if (prev == curr) return; //if no change ignore
        if (curr) // affect death
        {
            animator.CrossFade("Idle", 0);
            spriteRenderer.sprite = deadSprite;
            playerCollider.enabled = false;
            reviveCollider.enabled = true;
            reviveCanvas.gameObject.SetActive(true);
            canMove = false;
            bulletReceiver.enabled = false;

            Debug.Log($"Received death of {OwnerClientId}");
            Debug.Log($"i am server: {IsServer}");
            if (IsServer) // trigger check for death
            {
                Debug.Log("Checking PlayerManager for game over");
                PlayerManager.Instance.CheckGameOver();
            }
        } else // affect revive
        {
            spriteRenderer.sprite = aliveSprite;
            playerCollider.enabled = true;
            reviveCollider.enabled = false;
            reviveCanvas.gameObject.SetActive(false);
            canMove = true;
            bulletReceiver.enabled = true;
            TriggerInvulnVisual(hurtInvulnTime);
        }

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(!IsOwner) return;
        if (isDead.Value) return; //dont attack when dead
        if (col.CompareTag("Enemy"))
        {
            Enemy enemy = col.GetComponentInParent<Enemy>();

            if (!enemy) return;

            OnHitEnemy(enemy);
            
        }
    }

    private void OnHitEnemy(Enemy enemy)
    {
        playerCombo.ModifyCombo(stats.GetStat(PlayerStatType.ComboGainMultiplier) * 10, true); //base combo gain 10?

        bool isCrit = Random.Range(0f, 1f) < stats.GetStat(PlayerStatType.CriticalChance);
        float damage = stats.GetStat(PlayerStatType.MouseDamage) * (isCrit ? stats.GetStat(PlayerStatType.CriticalDamageMult) : 1);
        HitInfo hit = new HitInfo(isCrit, damage);
        hit = upgrades.TriggerUpgradeOnHitEnemyEffects(enemy, hit);

        enemy.TakeDamage(hit.GetFinalDamage() * playerCombo.GetComboDmgMult(),transform.position); //affect dmg
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!IsOwner || !isDead.Value || !collision.gameObject.CompareTag("Player")) return;
        // check death n check player collision. only owner checks for this

        if (collision.gameObject.GetComponent<Player>().isDead.Value) return; //dead player cannot revive another dead player

        reviveTimer.Value += Time.fixedDeltaTime * 1.5f;

        Debug.Log($"reviveTime: {reviveTimer}");
        if (reviveTimer.Value >= reviveTime)
        {
            Revive();
            reviveTimer.Value = 0;
        }
    }

    private void FixedUpdate()
    {
        if (isDead.Value) //only tick if dead
        {
            //Debug.Log($"{OwnerClientId} ReviveTimer: {reviveTimer}");
            if (IsOwner) reviveTimer.Value = Mathf.Clamp(reviveTimer.Value - (Time.fixedDeltaTime * 0.5f), 0, reviveTime);
            reviveImage.fillAmount = reviveTimer.Value / reviveTime;
        }
    }
    /*
    [Rpc(SendTo.Server)]
    private void OnHitRPC()
    {
        Debug.Log($"player hit: {OwnerClientId}");
        health.Value = Mathf.Clamp(health.Value - 1,0,3);
        if (health.Value <= 0)
        {
            PlayerDeathRPC();
            Debug.LogError($"Player died: {OwnerClientId}");
        }
    }
    */

    public void PlayBulletEmission(EmitterProfile pattern, float damage = -1)
    {
        bulletEmitter.SwitchProfile(pattern, false, PlayOptions.DoNothing);
        bulletEmitter.Boot();
        if (IsOwner)
        {
            bulletEmitter.bullets[bulletEmitter.bullets.Count - 1].moduleParameters.SetInt("PlayerId", (int)OwnerClientId);
            if (damage != -1)
            {
                bulletEmitter.bullets[bulletEmitter.bullets.Count - 1].moduleParameters.SetFloat("Damage", damage);
            }
        }
        
    }

    public void ModifyHealth(float amt)
    {
        health.Value = Mathf.FloorToInt(Mathf.Clamp(health.Value + amt,0,stats.GetStat(PlayerStatType.MaxHealth)));
    }

    public void Revive()
    {
        if (!isDead.Value)
        {
            Debug.LogError("Trying to revive alive player??");
            return;
        }
        isDead.Value = false;
        
        health.Value = 1; //revive with 1 hp
        SetInvuln();
        TriggerInvulnVisual(hurtInvulnTime);
        Invoke(nameof(SetVuln), hurtInvulnTime);
    }
    [Rpc(SendTo.Owner)]
    public void ReviveRPC() //for host use on wave end
    {
        Revive();
    }

    public void OnHurt()
    {
        if (!IsOwner || !isVulnerable) return; //clientside hit
        Debug.Log("local player hurt");

        SetInvuln();
        Invoke(nameof(SetVuln), hurtInvulnTime);
        //TriggerHurtVisual();
        
        ModifyHealth(-1);
        if(health.Value == 0)
        {
            isDead.Value = true;
        }
    }
    private void SetInvuln()
    {
        isVulnerable = false;
    }
    private void SetVuln()
    {
        isVulnerable = true;
    }

    private void TriggerHurtVisual()
    {
        if (InvulnVisualCoroutine != null)
        {
            StopCoroutine(InvulnVisualCoroutine);
        }
        InvulnVisualCoroutine = StartCoroutine(OnHurtVisual());
    }
    private void TriggerInvulnVisual(float duration)
    {
        if (InvulnVisualCoroutine != null)
        {
            StopCoroutine(InvulnVisualCoroutine);
        }
        InvulnVisualCoroutine = StartCoroutine(OnInvulnVisual(duration));
    }

    IEnumerator OnHurtVisual()
    {
        float timer = 0;
        spriteRenderer.material.SetFloat("_FlickerFreq", 3);
        while (timer < hurtInvulnTime/2)
        {
            spriteRenderer.material.SetFloat("_HitEffectBlend", 1 - timer/(hurtInvulnTime/2));
            timer += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.material.SetFloat("_HitEffectBlend", 0);
        spriteRenderer.material.SetFloat("_FlickerFreq", 0);
    }

    IEnumerator OnInvulnVisual(float duration)
    {
        spriteRenderer.material.SetFloat("_FlickerFreq", 3);
        spriteRenderer.material.SetFloat("_HitEffectBlend", 0);
        yield return duration;
        spriteRenderer.material.SetFloat("_FlickerFreq", 0);
    }

    [ContextMenu("Log Stats")]
    private void PrintStats()
    {
        stats.PrintStatsToConsole();
    }
}
