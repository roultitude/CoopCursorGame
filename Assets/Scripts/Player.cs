using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public NetworkVariable<int> health = new NetworkVariable<int>(3,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public bool isVulnerable = true;
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false,
    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public PlayerStats stats = new PlayerStats();
    public PlayerUpgrades upgrades;
    public PlayerAbilitySwipe playerAbility; //create an interface for this??? rmb to assign somehow
    public Color color;

    [SerializeField]
    float hitInvulnTime, reviveTime, moveSpeed;
    [SerializeField]
    SpriteRenderer spriteRenderer, reviveSpriteRenderer;
    [SerializeField]
    Sprite aliveSprite, deadSprite, invulnSprite;
    [SerializeField]
    public Collider2D playerCollider, reviveCollider;
    [SerializeField]
    Animator animator;
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
        Random.InitState((int)OwnerClientId);
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
            //Cursor.lockState = CursorLockMode.Locked;
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
        OnHit();
    }

    private void OnHealthChanged(int prev, int curr)
    {

        if(curr == 0)
        {
            return; //let death networkvar handle
        }
        else if(prev > curr)
        {
            animator.CrossFade("OnHitPlayer", 0);
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
            reviveSpriteRenderer.enabled = true;
            canMove = false;

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
            reviveSpriteRenderer.enabled = false;
            canMove = true;
        }

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(!IsOwner) return;
        if (col.CompareTag("Enemy"))
        {
            Enemy enemy = col.GetComponentInParent<Enemy>();

            if (!enemy) return;

            enemy.OnHit(stats.GetStat(PlayerStatType.ContactDamage)); //affect dmg
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isDead.Value || !collision.gameObject.CompareTag("Player") || !collision.GetComponentInParent<NetworkObject>().IsOwner) return; 
        // check death n check player collision. only local player can revive a dead

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
            reviveSpriteRenderer.transform.localScale = Vector3.one * reviveTimer / reviveTime;
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

    public void OnHit()
    {
        if (!IsOwner || !isVulnerable) return; //clientside hit
        Debug.Log("local player hit");
        
        isVulnerable = false;

        Invoke(nameof(HitInvuln), hitInvulnTime);
        ModifyHealth(-1);
        if(health.Value == 0)
        {
            isDead.Value = true;
        }
    }

    private void HitInvuln()
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
}
