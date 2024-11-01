using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance;
    public List<Player> Players;
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
        Players = new List<Player>();
        Debug.Log("Awake PlayerManager");
    }

    public void AddPlayer(Player player)
    {
        if(!Players.Contains(player)) {
            Players.Add(player);
        }
        PlayerUI ui = Instantiate(playerUIPrefab, playerUIHolder);

        ui.Setup($"Player {player.OwnerClientId}", player.health.Value);
        player.Setup(ui);
    }


}
