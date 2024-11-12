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
        DontDestroyOnLoad(gameObject);
        Debug.Log("Awake PlayerManager, added to DDOL");

    }

    public void AddPlayer(Player player)
    {
        if(!players.Contains(player)) {
            players.Add(player);
        }
        OnPlayerListChangeEvent?.Invoke();
    }

    public void CheckGameOver()
    {
        if (!IsServer) return;

        bool areAllPlayersDead = true;
        foreach (Player player in players)
        {
            if (!player.isDead.Value)
            {
                areAllPlayersDead = false;
                break;
            }
        }
        if (areAllPlayersDead)
        {
            Debug.Log("All players dead, restarting game");
            foreach(Player player in players)
            {
                player.NetworkObject.Despawn();
            }
            NetworkManager.SceneManager.LoadScene("PreGameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
            // restart game
        }
    }

}
