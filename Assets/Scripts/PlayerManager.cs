using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance;
    public List<Player> players;
    public PlayerUI playerUIPrefab;
    public Transform playerUIHolder;

    public void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        players = new List<Player>();
        Debug.Log("Awake PlayerManager");
    }

    public void AddPlayer(Player player)
    {
        if(!players.Contains(player)) {
            players.Add(player);
        }
        PlayerUI ui = Instantiate(playerUIPrefab, playerUIHolder);

        ui.Setup($"Player {player.OwnerClientId}", player.health.Value);
        player.Setup(ui);
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
            NetworkManager.SceneManager.LoadScene("NGO_Setup", UnityEngine.SceneManagement.LoadSceneMode.Single);
            // end game
        }
    }

}
