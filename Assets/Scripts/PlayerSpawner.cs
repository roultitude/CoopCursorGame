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
        if (!isDebugScene) NetworkManager.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
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
        Player player = Instantiate(playerPrefab);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
    }
}
