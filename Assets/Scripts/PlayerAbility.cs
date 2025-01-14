using Unity.Netcode;
using UnityEngine;

public abstract class PlayerAbility : NetworkBehaviour
{
    public NetworkVariable<bool> isAbilityAvailable = new NetworkVariable<bool>(false,
    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Ability Settings")]
    [SerializeField] float baseCooldown =5f;


    protected TickableTimer cooldownTimer;
    protected Player player;

    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.Q) && cooldownTimer.isTimerComplete)
        {
            Use();
            cooldownTimer.Reset();
        }
    }
    protected virtual void Use()
    {
        Debug.Log("base PlayerAbility.Use()");
    }
    public virtual void Setup(Player player, Color color)
    {
        this.player = player;
        cooldownTimer = new CountupTimer(baseCooldown);
        cooldownTimer.Reset();
    }   
    public void ModifyCooldownTimer(float amt)
    {
        cooldownTimer.Tick(-amt);
    }

    protected virtual void FixedUpdate()
    {
        cooldownTimer.Tick(Time.fixedDeltaTime);
    }
}
