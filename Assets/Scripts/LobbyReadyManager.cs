using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyReadyManager : NetworkBehaviour
{
    public static LobbyReadyManager Instance;

    public delegate void ReadinessChangeEvent(ulong playerId, bool isReady);
    public ReadinessChangeEvent OnAnyReadinessChange;
    public Dictionary<ulong, bool> PlayerReadinessDict;

    [SerializeField]
    Button readyButton;
    [SerializeField]
    Button startGameButton;

    private bool isReady = false;

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            return;
        }
        Debug.LogError("LobbyReadyManager singleton already exists!");
        Destroy(this);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        PlayerReadinessDict = new Dictionary<ulong, bool>();

        NetworkManager.OnClientDisconnectCallback += OnDisconnected;
        NetworkManager.OnClientConnectedCallback += OnConnected;
        NetworkManager.ConnectionApprovalCallback+= OnConnecting;
        readyButton.onClick.AddListener(ToggleReadiness);
        startGameButton.onClick.AddListener(StartGame);
    }

    private void OnConnecting(NetworkManager.ConnectionApprovalRequest arg1, NetworkManager.ConnectionApprovalResponse arg2)
    {
        Debug.Log("OnConnecting!");
        if (IsServer)
        {
            startGameButton.interactable = false;
            arg2.Approved = true;
        }
    }

    private bool AreAllPlayersReady()
    {
        foreach(bool val in PlayerReadinessDict.Values)
        {
            if (!val) return false;
        }
        
        return true;
    }

    private void OnDisconnected(ulong obj)
    {
        if (IsServer)
        {
            PlayerReadinessDict.Remove(obj);
            AreAllPlayersReady();
            return;
        }
        readyButton.interactable = false;
        ColorBlock btnColors = readyButton.colors;
        btnColors.normalColor = Color.white;
        btnColors.selectedColor = Color.white;
        btnColors.highlightedColor = Color.white;
        readyButton.colors = btnColors;
    }

    private void OnConnected(ulong obj)
    {
        if (IsServer){
            PlayerReadinessDict[obj] = false;
            startGameButton.interactable = AreAllPlayersReady();
        }
        Debug.Log("OnConnected");
        readyButton.interactable = true;
        ColorBlock btnColors = readyButton.colors;
        btnColors.normalColor = isReady ? Color.green : Color.red;
        btnColors.selectedColor = btnColors.normalColor;
        btnColors.highlightedColor = btnColors.normalColor;
        readyButton.colors = btnColors;
    }

    public void ToggleReadiness()
    {
        isReady = !isReady;
        ColorBlock btnColors = readyButton.colors;
        btnColors.normalColor = isReady? Color.green : Color.red;
        btnColors.selectedColor = btnColors.normalColor;
        btnColors.highlightedColor = btnColors.normalColor;
        readyButton.colors = btnColors;
        UpdateReadinessRPC(NetworkManager.LocalClientId, isReady);
    }

    [Rpc(SendTo.Everyone)]
    public void UpdateReadinessRPC(ulong id, bool isReady)
    {
        OnAnyReadinessChange?.Invoke(id, isReady);
        if (IsServer) //host 
        {
            PlayerReadinessDict[id] = isReady;
            startGameButton.interactable = AreAllPlayersReady();
        }
    }

    public void StartGame()
    {
        if(!IsServer) return;
        NetworkManager.SceneManager.LoadScene("PreGameScene",UnityEngine.SceneManagement.LoadSceneMode.Single);
        Debug.Log("Start Game");
    }
}
