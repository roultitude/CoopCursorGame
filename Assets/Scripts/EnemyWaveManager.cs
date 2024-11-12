using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using static WaveSO;

public class EnemyWaveManager : NetworkBehaviour
{
    public bool AreAllWavesSpawned() => currentWave.Value == (waves.Length);
    [SerializeField] bool debugSceneBool;
    [SerializeField] EnemySpawner spawner;
    [SerializeField] WaveSO[] waves;
    [SerializeField] float timeBetweenGroups, timeBetweenWaves;
    [SerializeField] ObjectiveUI objectiveUI;
    [SerializeField] int countdownLength;

   
    private NetworkVariable<int> currentWave = new NetworkVariable<int>(0);

    public void Awake()
    {
        if (debugSceneBool) return;
        NetworkManager.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        currentWave.OnValueChanged += UpdateUI;
        if (!debugSceneBool) return;

        StartCoroutine(WaveSpawningCoroutine());
    }

    private void UpdateUI(int previousValue, int newValue)
    {
        if(newValue == waves.Length)
        {
            objectiveUI.UpdateObjectiveText($"Clear all enemies to progress!");
        } else
        {
            objectiveUI.UpdateObjectiveText($"Wave {newValue} / {waves.Length}");
        }
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        NetworkManager.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;

        StartCoroutine(WaveSpawningCoroutine());
    }

    IEnumerator WaveSpawningCoroutine()
    {

        int countdown = countdownLength;
        while (countdown > 0)
        {
            objectiveUI.UpdateObjectiveText(countdown.ToString());
            countdown--;
            yield return new WaitForSeconds(1);
        }
        objectiveUI.UpdateObjectiveText($"Wave {currentWave.Value} / {waves.Length}");
        
        if (!IsServer) yield break;

        while (currentWave.Value < waves.Length)
        {
            
            foreach (EnemyGroup enemyGroup in waves[currentWave.Value].groups) //-1 here 
            {
                SpawnGroup(enemyGroup);
                yield return new WaitForSeconds(timeBetweenGroups);
            }
            currentWave.Value++;
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    public void SpawnGroup(EnemyGroup group)
    {
        foreach (Enemy enemy in group.enemies)
        {
            spawner.CreateEnemy(enemy);
        }
    }

}
