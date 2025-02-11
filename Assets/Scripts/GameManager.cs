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
    [SerializeField] private StageWaves[] bossWaves;
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
            FindAnyObjectByType<EnemyWaveManager>().SetEnemyWaves(bossWaves[(currentStage.Value)/4 - 1].waves); //currentStage starts at 1
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
        NetworkManager.SceneManager.LoadScene("PreGameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);

    }

    [ContextMenu("LoadNextScene")]
    public void LoadNextGameScene()
    {
        
        if (!IsServer) return;
        if (isDebugBoss) {
            NetworkManager.SceneManager.LoadScene("BossScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        } else
        {
            currentStage.Value = currentStage.Value + 1;
            if (currentStage.Value% 4 == 0) {
                NetworkManager.SceneManager.LoadScene("BossScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
            } else
            {
                NetworkManager.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
            
        }
    }

    public void LoadNextInterimScene()
    {
        if (!IsServer) return;
        NetworkManager.SceneManager.LoadScene("InterimScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}

[Serializable]
public struct StageWaves
{
    public WaveSO[] waves;
}
