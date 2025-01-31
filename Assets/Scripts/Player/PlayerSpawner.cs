using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField]
    bool isDebugScene;
    [SerializeField]
    Player playerPrefab;


    private void Awake()
    {
        Debug.Log("PlayerSpawner Awake");
        if (!isDebugScene) NetworkManager.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        Debug.Log("PlayerSpawner OnSceneLoaded Awake");
        NetworkManager.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;
        SpawnPlayerRPC();
    }

    public override void OnNetworkSpawn()
    {
        if(isDebugScene) SpawnPlayerRPC();
    }

    [Rpc(SendTo.Server)]
    private void SpawnPlayerRPC(RpcParams rpcParams = default)
    {
        Debug.Log($"PlayerSpawner SpawnRPC sent from {rpcParams.Receive.SenderClientId}");
        Player player = Instantiate(playerPrefab);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
    }
}
