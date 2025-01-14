using UnityEngine;

[CreateAssetMenu(fileName = "CustomEffectSO", menuName = "Scriptable Objects/CustomEffectSO")]

public abstract class CustomEffectSO : ScriptableObject
{
    public bool hasTimer;
    public virtual HitInfo OnHitEnemy(TickableTimer timer, Player player, Enemy enemy, HitInfo hit)
    {
        return hit;
    }

    public virtual void OnAdded(TickableTimer timer)
    {

    }

    public virtual void OnRPCTriggerTimer(TickableTimer timer)
    {
       //set timer info here
    }
}
