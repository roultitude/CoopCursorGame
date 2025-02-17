using Unity.Netcode;
using UnityEngine;

public class PlayerOrbital : NetworkBehaviour
{
    [SerializeField]
    protected float orbitRadius;
    [SerializeField]
    protected float orbitSpeed;

    protected Player player;
    protected float orbitAngle;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        PlayerManager.Instance.GetPlayerWithId(OwnerClientId).playerOrbitals.RegisterOrbital(this);
    }

    public void Setup(Player player, float startAngle, float orbitRadius, float orbitSpeed)
    {
        if (!IsServer) return;
        UpdateOrbitalPlayerRPC(player, startAngle, orbitRadius, orbitSpeed);
    }

    protected virtual void FixedUpdate()
    {
        OrbitMove();
    }

    protected virtual void OrbitMove()
    {
        if (player == null) return; // Ensure there's a valid center
        orbitAngle += orbitSpeed * Time.fixedDeltaTime;
        float x = Mathf.Cos(orbitAngle) * orbitRadius;
        float y = Mathf.Sin(orbitAngle) * orbitRadius;
        transform.position = new Vector3(player.transform.position.x + x, player.transform.position.y + y, 0);
    }

    [Rpc(SendTo.Everyone)]
    public void UpdateOrbitalPlayerRPC(NetworkBehaviourReference playerRef, float startAngle, float orbitRadius, float orbitSpeed)
    {
        if(playerRef.TryGet(out Player player)){
            this.player = player;
            this.orbitAngle = startAngle;
            this.orbitRadius = orbitRadius;
            this.orbitSpeed = orbitSpeed;
            
        } else
        {
            Debug.LogError("Orbital: Invalid PlayerReference!");
        }
    }

}
