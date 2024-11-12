using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance;

    public List<Player> players;

    public delegate void PlayerListChange();
    public static event PlayerListChange OnPlayerListChangeEvent;
    public void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        players = new List<Player>();
    }
    public override void OnNetworkSpawn()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void AddPlayer(Player player)
    {
        if(!players.Contains(player)) {
            players.Add(player);
            OnPlayerListChangeEvent?.Invoke();
        }
    }

    public void RemovePlayer(Player player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
            OnPlayerListChangeEvent?.Invoke();
        }
    }

    public void CheckGameOver()
    {
        Debug.Log($"PlayerManager checking IsServer {IsServer}");
        if (!IsServer) return;
        Debug.Log("Checking Game over");
        bool areAllPlayersDead = true;
        foreach (Player player in players)
        {
            if (!player.isDead.Value)
            {
                Debug.Log($"Player {player.OwnerClientId} still alive");
                areAllPlayersDead = false;
                break;
            }
        }
        if (areAllPlayersDead)
        {
            Debug.Log("All players dead, restarting game");
            for (int i = players.Count - 1; i >= 0; i--)
            {
                players[i].NetworkObject.Despawn();
            }
            Invoke(nameof(RestartGame), 5);
            // restart game in 5s, might want to display some notification here??
        }
    }

    public void RestartGame()
    {
        NetworkManager.SceneManager.LoadScene("PreGameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

}
