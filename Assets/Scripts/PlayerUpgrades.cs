using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerUpgrades : NetworkBehaviour
{
    [SerializeField]
    List<UpgradeSO> activeUpgrades;

    public void AddUpgrade(UpgradeSO upgrade)
    {
        activeUpgrades.Add(upgrade);
        Debug.Log($"{OwnerClientId} gets {upgrade.upgradeName}");
    }
}
