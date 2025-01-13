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
        stats[PlayerStatType.Squash] = 1;
        stats[PlayerStatType.Scamper] = 1;
        stats[PlayerStatType.Squeal] = 1;
        stats[PlayerStatType.MouseDamage] = 4;
        stats[PlayerStatType.MouseDamageMult] = 1;
        stats[PlayerStatType.AttackSpeed] = 0.5f;
        stats[PlayerStatType.AttackSpeedMult] = 1;
        stats[PlayerStatType.ElementalMastery] = 0;
        stats[PlayerStatType.ElementalCritChance] = 0;
        stats[PlayerStatType.SkillPower] = 0;
        stats[PlayerStatType.MaxHealth] = 3;
        stats[PlayerStatType.CooldownReduction] = 0;
        stats[PlayerStatType.CriticalChance] = 0.05f;
        stats[PlayerStatType.CriticalDamageMult] = 2;
        stats[PlayerStatType.ComboGainMultiplier] = 1;
        stats[PlayerStatType.DodgeChance] = 0;
        stats[PlayerStatType.SizeMult] = 1;

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
        //Squash
        ModifyStat(PlayerStatType.MouseDamage, GetStat(PlayerStatType.Squash) * 1f, false);

        //Scamper
        ModifyStat(PlayerStatType.SkillPower, GetStat(PlayerStatType.Scamper) * 10f,false);
        ModifyStat(PlayerStatType.AttackSpeed, GetStat(PlayerStatType.Scamper) * 0.5f,false);

        //Squeak
        ModifyStat(PlayerStatType.ElementalMastery, GetStat(PlayerStatType.Squeal) * 10f, false);
        ModifyStat(PlayerStatType.CooldownReduction, GetStat(PlayerStatType.Squeal) * 0.01f,false);
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
    Squash = 0,
    Scamper = 1,
    Squeal = 2,
    MouseDamage = 3,
    MouseDamageMult = 4,
    AttackSpeed = 5,
    AttackSpeedMult = 6,
    ElementalMastery =7 ,
    ElementalCritChance = 8,
    SkillPower = 9,
    MaxHealth = 10,
    CooldownReduction = 11,
    CriticalChance = 12,
    CriticalDamageMult = 13,
    ComboGainMultiplier = 14,
    DodgeChance = 15,
    SizeMult =16
}
