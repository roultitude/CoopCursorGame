
using System.Collections.Generic;
using UnityEngine;

public class Boss_HexaHexaGone : Boss
{
    [SerializeField]
    private Enemy hexaPartPrefab;
    
    [SerializeField]
    private Vector2[] partSpawnPos;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            Invoke(nameof(SpawnParts),1f);
        }
    }

    public override void OnHurt(float num)
    {
        if (parts.Count != 0) return;
        base.OnHurt(num);
    }

    public void SpawnParts()
    {
        for (int i = 0; i < partSpawnPos.Length; i++)
        {
            Enemy part = Instantiate(hexaPartPrefab, partSpawnPos[i] + (Vector2) transform.position,Quaternion.identity);
            part.NetworkObject.Spawn();
            part.SetupRPC(this);
        }
        
    }
}
