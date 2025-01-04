using UnityEngine;

[CreateAssetMenu(fileName = "CustomEffectSO", menuName = "Scriptable Objects/CustomEffectSO")]

public abstract class CustomEffectSO : ScriptableObject
{
    public virtual void OnHitEnemy(Enemy enemy)
    {
        Debug.Log("Base.OnHitEnemy");
    }

    public virtual void OnAdded()
    {

    }
}
