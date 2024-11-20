using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneUIManager : MonoBehaviour
{
    [SerializeField] PlayerUI playerUIPrefab;
    [SerializeField] Transform playerUIHolder;

    public void Awake()
    {
        if(NetworkManager.Singleton.SceneManager != null) NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
        PlayerManager.OnPlayerListChangeEvent += Setup;
    }

    public void OnDisable()
    {
        PlayerManager.OnPlayerListChangeEvent -= Setup;
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        Debug.Log("OnSceneLoaded GameSceneUIManager");
        Setup();
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;
    }

    public void Setup()
    {
        foreach (Transform transform in playerUIHolder.transform) {
            Destroy(transform.gameObject);
        }
        foreach(Player player in PlayerManager.Instance.players) {
            PlayerUI ui = Instantiate(playerUIPrefab, playerUIHolder);
            ui.Setup($"Player {player.OwnerClientId}", player);
        }
    }
}
