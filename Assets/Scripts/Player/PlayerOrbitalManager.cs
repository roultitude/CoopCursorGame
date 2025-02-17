using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerOrbitalManager : NetworkBehaviour
{
    [SerializeField]
    PlayerOrbital orbitalPrefabTriangle, orbitalPrefabSquare;
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
    public void RegisterOrbital(PlayerOrbital newOrb)
    {
        orbitals.Add(newOrb);
        RefreshOrbitalPositions();
    }

    public void RefreshOrbitalPositions()
    {

        for (int i = 0; i < orbitals.Count; i++)
        {
            orbitals[i].Setup(player, ((float) i / orbitals.Count) * Mathf.PI*2, orbitRadius, orbitSpeed);
        }
    }
    [ContextMenu("testSpawnOrb1")]
    public void TestSpawnTriangleOrb()
    {
        SpawnOrbitalRPC(0);
    }

    [ContextMenu("testSpawnOrb2")]
    public void TestSpawnSquareOrb()
    {
        SpawnOrbitalRPC(1);
    }


    [Rpc(SendTo.Server)]
    public void SpawnOrbitalRPC(int idx)
    {
        PlayerOrbital orb = Instantiate(idx == 0 ? orbitalPrefabTriangle : orbitalPrefabSquare);
        orb.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
    }


}
