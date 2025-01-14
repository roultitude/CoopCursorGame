using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using BulletPro;

public class Player : NetworkBehaviour
{
    public NetworkVariable<int> health = new NetworkVariable<int>(3,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public bool isVulnerable = true;
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false,
    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public PlayerStats stats = new PlayerStats();
    public PlayerUpgrades upgrades;
    public PlayerAbility playerAbility; //create an interface for this??? rmb to assign somehow
    public Color color;

    [SerializeField]
    float hurtInvulnTime, reviveTime, moveSpeed;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    Canvas reviveCanvas;
    [SerializeField]
    Image reviveImage;
    [SerializeField]
    Sprite aliveSprite, deadSprite, invulnSprite;
    [SerializeField]
    public Collider2D playerCollider, reviveCollider;
    [SerializeField]
    Animator animator;
    [SerializeField]
    BulletReceiver bulletReceiver;
    private Vector2 targetPos;
    

    private bool canMove = true;
    private float reviveTimer = 0;
    private Coroutine InvulnVisualCoroutine;
    private float[] playerBounds = new float[4];

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log($"OnNetworkSpawn Player {OwnerClientId}");

        health.OnValueChanged += OnHealthChanged;
        isDead.OnValueChanged += OnDeathStatusChanged;

        //Setup Color
        Random.InitState((int)OwnerClientId+2);
        color = Random.ColorHSV();
        spriteRenderer.color = color;
        playerAbility.Setup(this, color);
        PlayerManager.Instance.AddPlayer(this);

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
    
    public override void OnNetworkDespawn()
    {
        health.OnValueChanged -= OnHealthChanged;
        isDead.OnValueChanged -= OnDeathStatusChanged;
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
            animator.CrossFade("OnHurtPlayer", 0);
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
        bool isCrit = Random.Range(0f, 1f) < stats.GetStat(PlayerStatType.CriticalChance);
        float damage = stats.GetStat(PlayerStatType.MouseDamage) * (isCrit ? stats.GetStat(PlayerStatType.CriticalDamageMult) : 1);
        HitInfo hit = new HitInfo(isCrit, damage);
        hit = upgrades.TriggerUpgradeOnHitEnemyEffects(enemy, hit);

        
        enemy.TakeDamage(hit.damage); //affect dmg
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isDead.Value || !collision.gameObject.CompareTag("Player") || !collision.GetComponentInParent<NetworkObject>().IsOwner) return;
        // check death n check player collision. only local player can revive a dead

        if (collision.gameObject.GetComponent<Player>().isDead.Value) return; //dead player cannot revive another dead player

        reviveTimer += Time.fixedDeltaTime * 1.5f;

        Debug.Log($"reviveTime: {reviveTimer}");
        if (reviveTimer >= reviveTime)
        {
            ReviveRPC();
            reviveTimer = 0;
        }
    }

    private void FixedUpdate()
    {
        if (isDead.Value)
        {
            //Debug.Log($"{OwnerClientId} ReviveTimer: {reviveTimer}");
            reviveTimer = Mathf.Clamp(reviveTimer - (Time.fixedDeltaTime * 0.5f), 0, reviveTime);
            reviveImage.fillAmount = reviveTimer / reviveTime;
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

    public void ModifyHealth(float amt)
    {
        health.Value = Mathf.FloorToInt(Mathf.Clamp(health.Value + amt,0,stats.GetStat(PlayerStatType.MaxHealth)));
    }

    [Rpc(SendTo.Owner)] //send to owner since isDead & health are owner auth
    public void ReviveRPC(RpcParams rpcParams = default)
    {
        if (!isDead.Value)
        {
            Debug.LogError("Trying to revive alive player??");
            return;
        }
        //revive player
        Debug.Log($"Player {rpcParams.Receive.SenderClientId} revived Player {OwnerClientId}");
        isDead.Value = false;
        health.Value = 1; //revive with 1 hp
    }

    public void OnHurt()
    {
        if (!IsOwner || !isVulnerable) return; //clientside hit
        Debug.Log("local player hurt");
        
        isVulnerable = false;

        Invoke(nameof(HurtInvuln), hurtInvulnTime);
        ModifyHealth(-1);
        if(health.Value == 0)
        {
            isDead.Value = true;
        }
    }

    private void HurtInvuln()
    {
        isVulnerable = true;
    }
    /*
    private IEnumerator HitInvulnVisual()
    {
        Debug.Log("invulnSprite");
        spriteRenderer.sprite = invulnSprite;
        yield return new WaitForSeconds(hitInvulnTime);
        Debug.Log("invuln over aliveSprite");
        spriteRenderer.sprite = aliveSprite;
    }
    */

    [ContextMenu("Log Stats")]
    private void PrintStats()
    {
        stats.PrintStatsToConsole();
    }
}
