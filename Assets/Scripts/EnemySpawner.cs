using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] NetworkObject[] enemyPrefabs;
    [SerializeField] EnemySpawnIndicator enemySpawnIndicatorPrefab;
    [SerializeField] float spawnIndicatorTime;

    private Queue<(Vector2 location, NetworkObject enemyPrefab)> spawnQueue 
        = new Queue<(Vector2 location, NetworkObject enemyPrefab)>();
    

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        Invoke(nameof(CreateEnemy), 5);
        Invoke(nameof(CreateEnemy), 6);
        Invoke(nameof(CreateEnemy), 7);


        Invoke(nameof(CreateEnemy), 25);
        Invoke(nameof(CreateEnemy), 26);
        Invoke(nameof(CreateEnemy), 27);
        Invoke(nameof(CreateEnemy), 28);
        Invoke(nameof(CreateEnemy), 29);

        Invoke(nameof(CreateEnemy), 51);
        Invoke(nameof(CreateEnemy), 52);
        Invoke(nameof(CreateEnemy), 53);
        Invoke(nameof(CreateEnemy), 54);
        Invoke(nameof(CreateEnemy), 55);
        Invoke(nameof(CreateEnemy), 56);
        Invoke(nameof(CreateEnemy), 57);
        Invoke(nameof(CreateEnemy), 58);
        //InvokeRepeating("SpawnEnemy", 10,5);
    }

    [Rpc(SendTo.Everyone)]
    public void CreateEnemySpawnIndicatorRPC(float x, float y, float spawnTime)
    {
        EnemySpawnIndicator esi = Instantiate(enemySpawnIndicatorPrefab, new Vector3(x,y),Quaternion.identity);
        esi.Setup(spawnTime);
    }
    
    public void CreateEnemy()
    {
        if (!IsServer) return; // only server can spawn enemies
        Vector2 loc = new Vector2(Random.Range(-8.5f, 8.5f), Random.Range(-5, 5));
        CreateEnemySpawnIndicatorRPC(loc.x,loc.y, spawnIndicatorTime);
        spawnQueue.Enqueue((loc, enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]));
        Invoke(nameof(SpawnEnemy),spawnIndicatorTime);
    }

    public void SpawnEnemy()
    {
        Debug.Log("Attempting to Spawn Enemy");
        //server instantiates
        (Vector2 loc, NetworkObject enemyPrefab) = spawnQueue.Dequeue();
        NetworkObject netObj = Instantiate(enemyPrefab, loc, Quaternion.identity);
        //call spawn to propagate spawn to clients
        netObj.Spawn(true);
    }
}
