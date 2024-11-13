using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] EnemyWaveManager waveManager;
    [SerializeField] EnemySpawnIndicator enemySpawnIndicatorPrefab;
    [SerializeField] float spawnIndicatorTime;
    [SerializeField] List<Enemy> activeEnemies; // locally maintained list by enemy SetupRPC and DeathRPC

    private Queue<(Vector2 location, Enemy enemyPrefab)> spawnQueue 
        = new Queue<(Vector2 location, Enemy enemyPrefab)>();

    public void TrackEnemy(bool isTracking, Enemy enemy)
    {
        if (isTracking && !activeEnemies.Contains(enemy))
        { //enemy spawned
            activeEnemies.Add(enemy);
        }
        else if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy); //enemy died
            if(IsServer && activeEnemies.Count == 0)
            {
                if (waveManager.AreAllWavesSpawned()) //done
                {
                    foreach (Player player in PlayerManager.Instance.players)
                    {
                        if (player.isDead.Value)
                        {
                            Debug.Log($"Autorevived player {player.OwnerClientId}");
                            player.ReviveRPC();

                        }
                    }
                    GameManager.Instance.OnScreenClear();
                } else
                {
                    waveManager.EndWaveEarly();
                }
                
            }
        }
        //Debug.Log($"Changing Enemy List {enemy}, length after: {activeEnemies.Count}");
    }
    [Rpc(SendTo.Everyone)]
    public void CreateEnemySpawnIndicatorRPC(float x, float y, float spawnTime)
    {
        EnemySpawnIndicator esi = Instantiate(enemySpawnIndicatorPrefab, new Vector3(x,y),Quaternion.identity);
        esi.Setup(spawnTime);
    }
    
    public void CreateEnemy(Enemy enemy, Vector2 loc)
    {
        if (!IsServer) return; // only server can spawn enemies
        CreateEnemySpawnIndicatorRPC(loc.x,loc.y, spawnIndicatorTime);

        spawnQueue.Enqueue((loc, enemy));
        //spawnQueue.Enqueue((loc, enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]));
        Invoke(nameof(SpawnEnemy),spawnIndicatorTime);
    }

    public void SpawnEnemy()
    {
        //Debug.Log("Attempting to Spawn Enemy");
        //server instantiates
        (Vector2 loc, Enemy enemyPrefab) = spawnQueue.Dequeue();
        Enemy enemy = Instantiate(enemyPrefab, loc, Quaternion.identity);
        //call spawn to propagate spawn to clients
        enemy.NetworkObject.Spawn(true);
        enemy.SetupRPC(this);
    }
}
