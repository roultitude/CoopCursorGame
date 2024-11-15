using UnityEngine;

[CreateAssetMenu(fileName = "CustomEffectSO", menuName = "Scriptable Objects/CustomEffectSO")]

public abstract class CustomEffectSO : ScriptableObject
{
    public virtual void OnApply()
    {
        Debug.Log("Base.OnApply");
    }
}
