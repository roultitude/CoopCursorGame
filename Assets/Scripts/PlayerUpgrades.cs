using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerUpgrades : NetworkBehaviour
{
    [SerializeField]
    List<Upgrade> activeUpgrades;
    [SerializeField]
    Player player;

    int nextId; //for network serialization on upgradeTriggers
    private void Awake()
    {
        player = GetComponent<Player>();
        activeUpgrades = new List<Upgrade>();
        nextId = 0;
    }

    public void AddUpgrade(UpgradeSO upgrade)
    {
        Debug.Log($"Player {OwnerClientId} gets {upgrade.upgradeName}, {nextId}");
        activeUpgrades.Add(new Upgrade(upgrade, nextId++));
        player.stats.ApplyStatUpgrades(activeUpgrades);
    }

    public void DebugClearUpgrades()
    {
        activeUpgrades.Clear();
        player.stats.ApplyStatUpgrades(activeUpgrades);
    }

    public void FixedUpdate()
    {
        foreach (Upgrade upgrade in activeUpgrades) { 
            upgrade.TickTimer(Time.fixedDeltaTime); //tick all timers
        }
    }

    public HitInfo TriggerUpgradeOnHitEnemyEffects(Enemy enemy, HitInfo hit)
    {
        foreach (Upgrade upgrade in activeUpgrades)
        {
            hit = upgrade.OnHitCustomEffect(player, enemy, hit);
        }
        return hit;
    }

    public List<Upgrade> GetUpgrades()
    {
        return activeUpgrades; // might want to format as some kind of dict eventually
    }
}
