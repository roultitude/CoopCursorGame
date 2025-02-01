using Unity.Netcode;
using UnityEngine;

public class DebugTestUpgrades : NetworkBehaviour
{
    [SerializeField]
    public UpgradeSO[] upgradeLibrary;
    [SerializeField]
    UpgradeSO[] upgradesToAdd;
    [SerializeField]
    int playerIndex;

    [ContextMenu("Add Upgrades")]
    public void ApplyUpgrades()
    {
        int[] indxs = new int[upgradesToAdd.Length];
        for (int i =0; i< upgradesToAdd.Length; i++)
        {
            indxs[i] = upgradeToIndex(upgradesToAdd[i]);
        }
        ApplyUpgradesRPC(playerIndex,indxs);
    }

    [Rpc(SendTo.Everyone)]
    public void ApplyUpgradesRPC(int playerIndex, int[] upgradeIndexes)
    {
        Debug.Log($"Applying {upgradeIndexes.Length} upgrades to {playerIndex}");
        foreach (int idx in upgradeIndexes)
        {
            PlayerManager.Instance.players[playerIndex].upgrades.AddUpgrade(upgradeLibrary[idx]);
        }
    }

    [ContextMenu("Clear Upgrades")]
    public void ClearUpgrades()
    {
        PlayerManager.Instance.localPlayer.upgrades.DebugClearUpgrades();
    }

    private int upgradeToIndex(UpgradeSO upg)
    {
        for (int i =0; i< upgradeLibrary.Length; i++)
        {
            if (upg == upgradeLibrary[i])
            {
                return i;
            }
        }
        Debug.LogError("REFRESH UPGRADE LIBRARY!");
        return -1;
    }
}
