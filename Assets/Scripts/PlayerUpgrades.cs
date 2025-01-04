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

    public void DebugClearUpgrades()
    {
        activeUpgrades.Clear();
        player.stats.ApplyStatUpgrades(activeUpgrades);
    }

    public void TriggerUpgradeEnemyHitEffects(Enemy enemy)
    {
        foreach (UpgradeSO upgrade in activeUpgrades)
        {
            if (upgrade.customEffect)
            {
                upgrade.customEffect.OnHitEnemy(enemy);
            }
        }
    }

    public List<UpgradeSO> GetUpgrades()
    {
        return activeUpgrades; // might want to format as some kind of dict eventually
    }
}
