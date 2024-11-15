using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    private Dictionary<PlayerStatType, float> stats = new Dictionary<PlayerStatType, float>();

    public PlayerStats()
    {
        stats[PlayerStatType.Curiosity] = 1;
        stats[PlayerStatType.Adaptability] = 1;
        stats[PlayerStatType.Fortitude] = 1;
        stats[PlayerStatType.MaxHealth] = 3;
        stats[PlayerStatType.ContactDamage] = 10;
        stats[PlayerStatType.CooldownReduction] = 0;
        stats[PlayerStatType.CriticalChance] = 5;
        stats[PlayerStatType.CriticalDamage] = 50;
        stats[PlayerStatType.ComboMultiplier] = 1;
        stats[PlayerStatType.AbilityDamage] = 20;
    }
    public float GetStat(PlayerStatType statType)
    {
        return stats.ContainsKey(statType) ? stats[statType] : 0;
    }

    public void SetStat(PlayerStatType statType, int value)
    {
        if (stats.ContainsKey(statType))
        {
            stats[statType] = value;
        }
        else
        {
            stats.Add(statType, value);
        }
    }

    // Methods to modify specific stats safely
    public void IncreaseStat(PlayerStatType statType, int amount)
    {
        if (stats.ContainsKey(statType))
        {
            stats[statType] += amount;
        }
    }

    public void DecreaseStat(PlayerStatType statType, int amount)
    {
        if (stats.ContainsKey(statType))
        {
            stats[statType] = Mathf.Max(0, stats[statType] - amount); // Prevents negative values
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
