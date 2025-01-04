using UnityEngine;

public class DebugTestUpgrades : MonoBehaviour
{
    [SerializeField]
    UpgradeSO[] upgrades;

    [ContextMenu("Add Upgrades")]
    public void ApplyUpgrades()
    {
        foreach (UpgradeSO upgrade in upgrades) {
            PlayerManager.Instance.localPlayer.upgrades.AddUpgrade(upgrade);
        }
    }

    [ContextMenu("Clear Upgrades")]
    public void ClearUpgrades()
    {
        PlayerManager.Instance.localPlayer.upgrades.DebugClearUpgrades();
    }
}
