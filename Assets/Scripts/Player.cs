using UnityEngine;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using System.Collections;

public class Player : NetworkBehaviour
{
    public NetworkVariable<int> health = new NetworkVariable<int>(3,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public bool isVulnerable = true;


    [SerializeField]
    float hitInvulnTime;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    Sprite aliveSprite, deadSprite, invulnSprite;
    [SerializeField]
    Collider2D playerCollider;
    [SerializeField]
    Animator animator;
    
    private PlayerUI ui;
    private bool canMove = true;
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
    }
    
    public override void OnNetworkDespawn()
    {
        health.OnValueChanged -= OnHealthChanged;
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
            OnPlayerDeath();
        }
        else if(prev > curr)
        {
            animator.CrossFade("OnHitPlayer", 0);
            //InvulnVisualCoroutine = StartCoroutine(HitInvulnVisual());
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

    private void OnPlayerDeath()
    {
        animator.CrossFade("Idle",0);
        Debug.Log("deadSprite");
        spriteRenderer.sprite = deadSprite;
        playerCollider.enabled = false;
        canMove = false;
    }

    private void OnHit()
    {
        if (!IsOwner) return; //clientside hit
        Debug.Log("local player hit");
        
        isVulnerable = false;

        Invoke(nameof(HitInvuln), hitInvulnTime);
        health.Value = Mathf.Clamp(health.Value - 1, 0, 3);
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
