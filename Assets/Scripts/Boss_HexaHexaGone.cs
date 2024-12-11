
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

public class Boss_HexaHexaGone : Boss
{
    [SerializeField]
    private Enemy hexaPartPrefab;
    
    public Vector2[] partSpawnPos;
    private Vector2 startPosition;

    //[SerializeField]
    //private // for particlesystem for spawning effect
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            transform.position = startPosition;
            Invoke(nameof(SpawnParts),1f); 
            // we need the main body to have spawned on all clients else SetupRPC on parts will not register to the parts list
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
            part.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue<int>("PartNumber", i);
        }
        
    }
    protected override void FixedUpdate()
    {
        if (!IsServer) return;
        //Move();
        /*
        for (int i = 0; i < Mathf.Min(partSpawnPos.Length,partPosition.Count); i++) {
            partPosition[i] = partSpawnPos[i] + (Vector2)transform.position; 
        }*/
    }

    public void RadialBulletAttack()
    {

    }
}
