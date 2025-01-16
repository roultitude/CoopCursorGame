using System;
using UnityEngine;

public class HitInfo
{
    public bool isCrit { get; private set;}
    public float damage { get; private set;}
    public float flatDmgBonus;
    public float damageMult;
    public HitInfo(bool isCrit, float damage)
    {
        this.isCrit = isCrit;
        this.damage = damage;
        flatDmgBonus = 0;
        damageMult = 1;
    }

    public HitInfo MutateCrit(Func<bool, bool> func)
    {
        isCrit = func(isCrit);
        return this;
    }
    public HitInfo MutateFlatDamageBonus(float amt)
    {
        flatDmgBonus += amt;
        return this;
    }
    public HitInfo MutateDamageMult(float amt)
    {
        damageMult += amt;
        return this;
    }
    public float GetFinalDamage()
    {
        return damage * damageMult + flatDmgBonus;
    }
}
