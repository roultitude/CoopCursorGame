using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    private Dictionary<PlayerStatType, float> stats;

    public delegate void PlayerStatsChange();
    public event PlayerStatsChange OnPlayerStatsChangeEvent;
    public PlayerStats()
    {
        stats = new Dictionary<PlayerStatType, float>();
        ResetStats();
        CalcPrimaryStatEffects();
        OnPlayerStatsChangeEvent?.Invoke();
    }

    public void ResetStats()
    {
        stats[PlayerStatType.Curiosity] = 1;
        stats[PlayerStatType.Adaptability] = 1;
        stats[PlayerStatType.Fortitude] = 1;
        stats[PlayerStatType.MaxHealth] = 3;
        stats[PlayerStatType.ContactDamage] = 4;
        stats[PlayerStatType.CooldownReduction] = 0;
        stats[PlayerStatType.CriticalChance] = 5;
        stats[PlayerStatType.CriticalDamage] = 50;
        stats[PlayerStatType.ComboMultiplier] = 1;
        stats[PlayerStatType.AbilityDamage] = 0;
    }
    public void ApplyStatUpgrades(List<UpgradeSO> upgrades)
    {
        ResetStats();
        foreach(UpgradeSO upgrade in upgrades)
        {
            for (int i = 0; i < upgrade.statChanges.Length; i++) {
                ModifyStat(upgrade.statChanges[i].type, upgrade.statChanges[i].amt);
            }
        }
        CalcPrimaryStatEffects();
        OnPlayerStatsChangeEvent?.Invoke();
    }

    private void CalcPrimaryStatEffects()
    {
        //Curiosity
        ModifyStat(PlayerStatType.ContactDamage, GetStat(PlayerStatType.Curiosity) * 0.5f, false);
        ModifyStat(PlayerStatType.AbilityDamage, GetStat(PlayerStatType.Curiosity) * 0.5f, false);

        //Adaptability
        ModifyStat(PlayerStatType.AbilityDamage, GetStat(PlayerStatType.Adaptability) * 0.5f,false);
        ModifyStat(PlayerStatType.CooldownReduction, GetStat(PlayerStatType.Adaptability) * 0.2f,false);

        //Fortitude
        ModifyStat(PlayerStatType.ContactDamage, GetStat(PlayerStatType.Fortitude) * 0.5f, false);
        ModifyStat(PlayerStatType.MaxHealth, GetStat(PlayerStatType.Fortitude) * 0.5f,false);
    }
    public float GetStat(PlayerStatType statType)
    {
        if(statType == PlayerStatType.MaxHealth)
        {
            return stats.ContainsKey(statType) ? Mathf.Floor(stats[statType]) : 0; //floor max hp 
        }
        return stats.ContainsKey(statType) ? stats[statType] : 0;
    }

    public void SetStat(PlayerStatType statType, float value)
    {
        if (stats.ContainsKey(statType))
        {
            stats[statType] = value;
        }
        else
        {
            stats.Add(statType, value);
        }
        OnPlayerStatsChangeEvent?.Invoke();
    }

    // Methods to modify specific stats safely
    public void ModifyStat(PlayerStatType statType, float amount, bool fireModifyEvent = true)
    {
        if (stats.ContainsKey(statType))
        {
            stats[statType] += amount;
            if(fireModifyEvent) OnPlayerStatsChangeEvent?.Invoke();
        }
    }
}

public enum PlayerStatType
{
    Curiosity,
    Adaptability,
    Fortitude,
    MaxHealth,
    ContactDamage,
    CooldownReduction,
    CriticalChance,
    CriticalDamage,
    ComboMultiplier,
    AbilityDamage
}
