using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField]
    Player playerPrefab;


    private void Awake()
    {
        NetworkManager.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        SpawnPlayerRPC();
        NetworkManager.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;
    }

    /*
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SpawnPlayerRPC();
        
    }
    */

    [Rpc(SendTo.Server)]
    private void SpawnPlayerRPC(RpcParams rpcParams = default)
    {
        Player player = Instantiate(playerPrefab);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId,true);
    }
}
