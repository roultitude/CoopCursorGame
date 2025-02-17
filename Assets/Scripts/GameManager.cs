using BulletPro;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    [SerializeField] private StageWaves[] stageWaves;
    [SerializeField] private bool isDebugBoss;

    public NetworkVariable<int> currentStage = new NetworkVariable<int>(0);
    public void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public override void OnNetworkSpawn()
    {
        DontDestroyOnLoad(gameObject);
        NetworkManager.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName == "GameScene")
        {
            FindAnyObjectByType<EnemyWaveManager>().SetEnemyWaves(stageWaves[currentStage.Value-1].waves); //currentStage starts at 1
        } else if (sceneName == "BossScene")
        {
            if (!stageWaves[currentStage.Value - 1].isBoss)
            {
                Debug.LogError("Tried to spawn boss when wave is not boss wave!");
                return;
            }
            FindAnyObjectByType<EnemyWaveManager>().SetEnemyWaves(stageWaves[currentStage.Value - 1].waves); //currentStage starts at 1
        }
    }


    public void OnAllPlayersDead()
    {
        Invoke(nameof(RestartGame), 5);
    }

    public void OnScreenClear()
    {
        Invoke(nameof(LoadNextInterimScene), 5);
    }
    public void RestartGame()
    {
        
        if (!IsServer) return;
        currentStage.Value = 0;
        LoadScene("PreGameScene");

    }

    [ContextMenu("LoadNextScene")]
    public void LoadNextGameScene()
    {
        
        if (!IsServer) return;
        if (isDebugBoss) {
            currentStage.Value = currentStage.Value + 1;
            LoadScene("BossScene");
        } else
        {
            currentStage.Value = currentStage.Value + 1;
            if (stageWaves[currentStage.Value - 1].isBoss) {
                LoadScene("BossScene");
            } else
            {
                LoadScene("GameScene");
            }
            
        }
    }

    private void LoadScene(string sceneName)
    {
        foreach(Bullet bul in BulletPoolManager.instance.pool)
        {
            bul.Die();
        }
        Debug.Log($"Killed {BulletPoolManager.instance.pool.Length} bullets");
        NetworkManager.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void LoadNextInterimScene()
    {
        if (!IsServer) return;
        LoadScene("InterimScene");
    }
}

[Serializable]
public struct StageWaves
{
    public bool isBoss;
    public WaveSO[] waves;
}
