using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeSelection : NetworkBehaviour
{
    [SerializeField]
    List<UpgradeSO> upgrades;

    [SerializeField]
    UpgradeSelectionTile[] upgradeTiles;

    [SerializeField] 
    float countdownTime;

    [SerializeField] 
    ObjectiveUI ui;

    [SerializeField]
    ReadyZone readyZone;

    float timer = 0;

    private void Awake()
    {
        NetworkManager.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        NetworkManager.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;
        if (!IsServer) return;
        SetupUpgrades();
    }


    public void FixedUpdate()
    {
        // check if all players have selected an item
        if (!ArePlayersSelectionValid())
        {
            timer = 0;
        } else
        {
            timer += Time.fixedDeltaTime;
            ui.UpdateObjectiveText(Mathf.Ceil(countdownTime - timer).ToString());
        }
        if (timer >= countdownTime)
        {
            if (!IsServer) return;
            TriggerUpgradeSelection();
        }
    }

    public void TriggerUpgradeSelection()
    {
        foreach (UpgradeSelectionTile tile in upgradeTiles)
        {
            // there should be only one player per tile touching
            List<Player> playersTouching = tile.GetPlayersTouching();
            if (playersTouching.Count == 0) continue;
            AcquireUpgradeRPC(upgrades.IndexOf(tile.data),playersTouching[0]);
            tile.gameObject.SetActive(false);
        }

    }

    [Rpc(SendTo.Everyone)]
    public void AcquireUpgradeRPC(int upgradeIndex, NetworkBehaviourReference playerRef)
    {
        Debug.Log("AcquireUpgradeRPC");
        if (playerRef.TryGet(out Player player))
        {
            player.upgrades.AddUpgrade(upgrades[upgradeIndex]);
            this.enabled = false; // disable this;
            readyZone.gameObject.SetActive(true);
        }
    }

    private bool ArePlayersSelectionValid()
    {
        int playersSelected = 0;
        foreach (UpgradeSelectionTile tile in upgradeTiles)
        {
            List<Player> selectingPlayers = tile.GetPlayersTouching();

            if (selectingPlayers.Count > 1)
            {
                ui.UpdateObjectiveText("Only one per person!");
                return false;
            }
            if (selectingPlayers.Count == 1 ) {
                playersSelected++;
            }
        }
        if (playersSelected < PlayerManager.Instance.players.Count) {
            ui.UpdateObjectiveText("Select an upgrade!");
            return false;
        }
        return true;
    }

    private void SetupUpgrades()
    {
        if(!IsServer) return;
        if(upgrades.Count <= upgradeTiles.Length)
        {
            Debug.LogError("Tried to setup upgrades without enough upgrades??");
            return;
        }
        List<int> upgradeIndexes = new List<int>(new int[upgradeTiles.Length]);
        for(int i = 0; i < upgradeTiles.Length; i++)
        {
            upgradeIndexes[i] = i;
        }
        List<int> selectedUpgradeIndexes = GetRandomElements(upgradeIndexes, upgradeIndexes.Count); //use indexes to sync easily across network

        ShowUpgradesRPC(selectedUpgradeIndexes.ToArray());
    }

    [Rpc(SendTo.Everyone)]
    public void ShowUpgradesRPC(int[] selectedUpgradeIndexes)
    {
        for (int i = 0; i < upgradeTiles.Length; i++)
        {
            upgradeTiles[i].Setup(upgrades[selectedUpgradeIndexes[i]]);
        }
    }

    public List<T> GetRandomElements<T>(List<T> list, int count)
    {
        System.Random random = new System.Random();
        List<T> result = new List<T>();
        HashSet<int> selectedIndices = new HashSet<int>();

        while (result.Count < count && result.Count < list.Count)
        {
            int index = random.Next(list.Count);

            // Ensure we don't pick the same element twice
            if (selectedIndices.Add(index))
            {
                result.Add(list[index]);
            }
        }

        return result;
    }

}
