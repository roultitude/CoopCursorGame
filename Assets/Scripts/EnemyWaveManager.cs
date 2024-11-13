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
    private float[] screenBounds = new float[4];
    private float waveTimer;
    private bool isNextWaveReady;

    public void Awake()
    {
        Vector2 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        Vector2 topRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        screenBounds[0] = bottomLeft.x;
        screenBounds[2] = bottomLeft.y;
        screenBounds[1] = topRight.x;
        screenBounds[3] = topRight.y;
        if (debugSceneBool) return;
        NetworkManager.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
    }

    public void SetEnemyWaves(WaveSO[] waves)
    {
        Debug.Log($"Set EnemyWaveSpawner to {waves.Length} waves");
        this.waves = waves;
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
        float radius = 2.5f;
        int countdown = countdownLength;
        while (countdown > 0)
        {
            objectiveUI.UpdateObjectiveText(countdown.ToString());
            countdown--;
            yield return new WaitForSeconds(1);
        }
        objectiveUI.UpdateObjectiveText($"Wave {currentWave.Value} / {waves.Length}");
        
        if (!IsServer) yield break;

        while (currentWave.Value < waves.Length) //wave spawn loop
        {
            EnemyGroup[] groups = waves[currentWave.Value].groups;
            List<Vector2> groupCenters = GenerateGroupCenters(groups.Length, radius);
            for(int i = 0; i < groups.Length; i++)
            {
                SpawnGroup(groups[i], groupCenters[i], radius);
                yield return new WaitForSeconds(timeBetweenGroups);
            }
            currentWave.Value++; //wave done
            isNextWaveReady = false;
            waveTimer = 0;
            // wave wait loop
            while (!isNextWaveReady)
            {
                yield return new WaitForSeconds(1);
                waveTimer += 1;
                if(waveTimer > timeBetweenWaves)
                {
                    isNextWaveReady = true;
                }
            }
            
        }
    }

    public void EndWaveEarly()
    {
        isNextWaveReady = true;
    }

    List<Vector2> GenerateGroupCenters(int numberOfGroups, float radius) //find center of groups bounded by screen without overlap
    {

        List<Vector2> centers = new List<Vector2>();
        int attempts = 0;

        while (centers.Count < numberOfGroups)
        {
            Vector2 newCenter = new Vector2(Random.Range(screenBounds[0] + radius, screenBounds[1] - radius), 
                Random.Range(screenBounds[2] + radius, screenBounds[3] - radius));
            bool isOverlapping = false;

            foreach (Vector2 center in centers)
            {
                if (Vector2.Distance(newCenter, center) < 2 * radius)
                {
                    isOverlapping = true;
                    break;
                }
            }

            if (!isOverlapping || attempts > 50) //50 tries per group
            {
                centers.Add(newCenter);
                attempts = 0;
            }

            attempts++;
        }

        return centers;
    }

    public void SpawnGroup(EnemyGroup group, Vector2 center, float radius) // spawn enemies in group randomly radially from center
    {
        for (int i = 0; i < group.enemies.Length; i++)
        {
            float angle = Random.Range(0, 2 * Mathf.PI);
            float distance = Random.Range(0, radius);
            Vector2 point = center + new Vector2(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle));
            spawner.CreateEnemy(group.enemies[i], point);

        }
    }

}
