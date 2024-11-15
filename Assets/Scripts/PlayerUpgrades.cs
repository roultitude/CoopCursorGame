using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerUpgrades : NetworkBehaviour
{
    [SerializeField]
    List<UpgradeSO> activeUpgrades;
    [SerializeField]
    Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public void AddUpgrade(UpgradeSO upgrade)
    {
        activeUpgrades.Add(upgrade);
        Debug.Log($"Player {OwnerClientId} gets {upgrade.upgradeName}");
        player.stats.ApplyStatUpgrades(activeUpgrades);
    }
}
