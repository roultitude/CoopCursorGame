using System;
using UnityEngine;

public class HitInfo
{
    public bool isCrit { get; private set;}
    public float damage { get; private set;}
    public HitInfo(bool isCrit, float damage)
    {
        this.isCrit = isCrit;
        this.damage = damage;
    }

    public HitInfo MutateCrit(Func<bool, bool> func)
    {
        isCrit = func(isCrit);
        return this;
    }
    public HitInfo MutateDamage(Func<float, float> func)
    {
        damage = func(damage);
        return this;
    }
    public float GetDamage()
    {
        return damage;
    }
}
