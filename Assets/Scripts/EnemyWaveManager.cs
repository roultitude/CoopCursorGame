using Unity.Netcode;
using UnityEngine;

public class EnemyWaveManager : NetworkBehaviour
{
    [SerializeField] EnemySpawner spawner;
    [SerializeField]
    WaveSO[] waves;


    private int currentWave = 0;

    public void SpawnNextWave()
    {
        foreach(WaveSO.EnemyGroup enemyGroup in waves[currentWave].groups)
        {
            //foreach()
        }
    }

}
