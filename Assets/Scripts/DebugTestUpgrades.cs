using Unity.Netcode;
using UnityEngine;

public class DebugTestUpgrades : MonoBehaviour
{
    [SerializeField]
    UpgradeSO[] upgrades;
    [SerializeField]
    int playerIndex;

    [ContextMenu("Add Upgrades")]
    public void ApplyUpgrades()
    {
        ApplyUpgradesRPC(playerIndex);
    }

    [Rpc(SendTo.Everyone)]
    public void ApplyUpgradesRPC(int playerIndex)
    {
        foreach (UpgradeSO upgrade in upgrades)
        {
            PlayerManager.Instance.players[playerIndex].upgrades.AddUpgrade(upgrade);
        }
    }

    [ContextMenu("Clear Upgrades")]
    public void ClearUpgrades()
    {
        PlayerManager.Instance.localPlayer.upgrades.DebugClearUpgrades();
    }
}
