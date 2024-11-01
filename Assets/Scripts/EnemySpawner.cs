using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] NetworkObject enemyPrefab;
    [SerializeField]
    Transform[] spawnTransforms;
    
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        Invoke("SpawnEnemy", 10);
        Invoke("SpawnEnemy", 10);
        Invoke("SpawnEnemy", 10);

        //InvokeRepeating("SpawnEnemy", 10,5);
    }


    
    public void SpawnEnemy()
    {
        Debug.Log("Attempting to Spawn Enemy");
        if (!IsServer) return; // only server can spawn enemies

        //server instantiates
        NetworkObject netObj = Instantiate(enemyPrefab, 
            spawnTransforms[Random.Range(0, spawnTransforms.Length)].position, 
            Quaternion.identity);
        //call spawn to propagate spawn to clients
        netObj.Spawn();
    }
}
