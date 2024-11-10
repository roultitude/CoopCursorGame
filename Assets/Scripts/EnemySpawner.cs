using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] NetworkObject[] enemyPrefabs;
    [SerializeField] EnemySpawnIndicator enemySpawnIndicatorPrefab;
    [SerializeField] float spawnIndicatorTime;

    private Queue<(Vector2 location, Enemy enemyPrefab)> spawnQueue 
        = new Queue<(Vector2 location, Enemy enemyPrefab)>();
    

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
    }

    [Rpc(SendTo.Everyone)]
    public void CreateEnemySpawnIndicatorRPC(float x, float y, float spawnTime)
    {
        EnemySpawnIndicator esi = Instantiate(enemySpawnIndicatorPrefab, new Vector3(x,y),Quaternion.identity);
        esi.Setup(spawnTime);
    }
    
    public void CreateEnemy(Enemy enemy)
    {
        if (!IsServer) return; // only server can spawn enemies
        Vector2 loc = new Vector2(Random.Range(-8.5f, 8.5f), Random.Range(-5, 5));
        CreateEnemySpawnIndicatorRPC(loc.x,loc.y, spawnIndicatorTime);

        spawnQueue.Enqueue((loc, enemy));
        //spawnQueue.Enqueue((loc, enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]));
        Invoke(nameof(SpawnEnemy),spawnIndicatorTime);
    }

    public void SpawnEnemy()
    {
        Debug.Log("Attempting to Spawn Enemy");
        //server instantiates
        (Vector2 loc, Enemy enemyPrefab) = spawnQueue.Dequeue();
        Enemy enemy = Instantiate(enemyPrefab, loc, Quaternion.identity);
        //call spawn to propagate spawn to clients
        enemy.NetworkObject.Spawn(true);
    }
}
