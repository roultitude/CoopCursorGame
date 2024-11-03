using UnityEngine;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using System.Collections;

public class Player : NetworkBehaviour
{
    public NetworkVariable<int> health = new NetworkVariable<int>(3,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public bool isVulnerable = true;
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false,
    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField]
    float hitInvulnTime, reviveTime;
    [SerializeField]
    SpriteRenderer spriteRenderer, reviveSpriteRenderer;
    [SerializeField]
    Sprite aliveSprite, deadSprite, invulnSprite;
    [SerializeField]
    Collider2D playerCollider, reviveCollider;
    [SerializeField]
    Animator animator;
    
    private PlayerUI ui;
    private bool canMove = true;
    private float reviveTimer = 0;
    private Coroutine InvulnVisualCoroutine;

    public void Setup(PlayerUI ui)
    {
        this.ui = ui;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log($"OnNetworkSpawn Player {OwnerClientId}");
        PlayerManager.Instance.AddPlayer(this);
        health.OnValueChanged += OnHealthChanged;
        isDead.OnValueChanged += OnDeathStatusChanged;
    }
    
    public override void OnNetworkDespawn()
    {
        health.OnValueChanged -= OnHealthChanged;
        isDead.OnValueChanged -= OnDeathStatusChanged;
    }

    void Update()
    {
        if (!canMove || !IsOwner || !IsSpawned) return;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        transform.position = mouseWorldPos;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!other.CompareTag("Enemy"))
        {
            return;
        }
        if (isVulnerable)
        {
            OnHit();
        }
    }

    private void OnHealthChanged(int prev, int curr)
    {
        ui.ShowHP(curr);
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

            if (IsServer) // trigger check for death
            {
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

    [Rpc(SendTo.Owner)] //send to owner since isDead & health are owner auth
    private void ReviveRPC(RpcParams rpcParams = default)
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
        if (!IsOwner) return; //clientside hit
        Debug.Log("local player hit");
        
        isVulnerable = false;

        Invoke(nameof(HitInvuln), hitInvulnTime);
        health.Value = Mathf.Clamp(health.Value - 1, 0, 3);
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
