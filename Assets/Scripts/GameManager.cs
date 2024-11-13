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

    public NetworkVariable<int> currentStage = new NetworkVariable<int>(-1);
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
            FindAnyObjectByType<EnemyWaveManager>().SetEnemyWaves(stageWaves[currentStage.Value].waves);
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
        currentStage.Value = -1;
        NetworkManager.SceneManager.LoadScene("PreGameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);

    }

    public void LoadNextGameScene()
    {
        
        if (!IsServer) return;
        currentStage.Value = (currentStage.Value + 1) % stageWaves.Length;
        NetworkManager.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
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
