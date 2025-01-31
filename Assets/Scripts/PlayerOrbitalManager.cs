using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerOrbitalManager : NetworkBehaviour
{
    [SerializeField]
    PlayerOrbital orbitalPrefab;
    [SerializeField]
    float orbitRadius;
    [SerializeField]
    float orbitSpeed;
    [SerializeField]
    Player player;

    private List<PlayerOrbital> orbitals;

    public void Awake()
    {
        orbitals = new List<PlayerOrbital>();
    }
    public void ReigsterOrbital(PlayerOrbital newOrb)
    {
        orbitals.Add(newOrb);
        RefreshOrbitalPositions();
    }

    public void RefreshOrbitalPositions()
    {
        if(orbitals.Count == 1) { 
            orbitals[0].Setup(player, 0, orbitRadius, orbitSpeed); //avoid dividebyzero
            return;                                                                            
        }

        for (int i = 0; i < orbitals.Count; i++)
        {
            orbitals[i].Setup(player, ((float) i / orbitals.Count-1) * 360, orbitRadius, orbitSpeed);
        }
    }
    [ContextMenu("testSpawnOrb")]
    private void TestSpawn()
    {
        SpawnOrbitalRPC();
    }

    [Rpc(SendTo.Server)]
    public void SpawnOrbitalRPC()
    {
        PlayerOrbital orb = Instantiate(orbitalPrefab);
        orb.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
    }
}
