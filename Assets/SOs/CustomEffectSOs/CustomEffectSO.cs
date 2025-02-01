using UnityEngine;

[CreateAssetMenu(fileName = "CustomEffectSO", menuName = "Scriptable Objects/CustomEffectSO")]

public abstract class CustomEffectSO : ScriptableObject
{
    public bool hasTimer;

    /// <summary>
    /// Returns bool indicating whether to trigger timer reset (cooldown usually)
    /// </summary>
    public virtual bool OnHitEnemy(TickableTimer timer, Player player, Enemy enemy, ref HitInfo hit)
    {
        return false;

    }

    public virtual bool OnComboLevelChange(TickableTimer timer, Player player, int prev, int curr)
    {
        return false;
    }

    public virtual bool OnAdded(TickableTimer timer, Player player)
    {
        return false;
    }

    public virtual void OnRPCTriggerTimer(TickableTimer timer)
    {
       //set timer info here
    }
}
