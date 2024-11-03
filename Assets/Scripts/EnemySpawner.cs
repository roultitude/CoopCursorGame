using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] NetworkObject[] enemyPrefabs;
    [SerializeField]
    Transform[] spawnTransforms;
    
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        Invoke("SpawnEnemy", 5);
        Invoke("SpawnEnemy", 6);
        Invoke("SpawnEnemy", 7);


        Invoke("SpawnEnemy", 25);
        Invoke("SpawnEnemy", 26);
        Invoke("SpawnEnemy", 27);
        Invoke("SpawnEnemy", 28);
        Invoke("SpawnEnemy", 29);

        Invoke("SpawnEnemy", 51);
        Invoke("SpawnEnemy", 52);
        Invoke("SpawnEnemy", 53);
        Invoke("SpawnEnemy", 54);
        Invoke("SpawnEnemy", 55);
        Invoke("SpawnEnemy", 56);
        Invoke("SpawnEnemy", 57);
        Invoke("SpawnEnemy", 58);
        //InvokeRepeating("SpawnEnemy", 10,5);
    }


    
    public void SpawnEnemy()
    {
        Debug.Log("Attempting to Spawn Enemy");
        if (!IsServer) return; // only server can spawn enemies

        //server instantiates
        NetworkObject netObj = Instantiate(enemyPrefabs[Random.Range(0,enemyPrefabs.Length)], 
            spawnTransforms[Random.Range(0, spawnTransforms.Length)].position, 
            Quaternion.identity);
        //call spawn to propagate spawn to clients
        netObj.Spawn();
    }
}
