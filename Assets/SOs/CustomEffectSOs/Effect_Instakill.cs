using UnityEngine;

[CreateAssetMenu(fileName = "Effect_Instakill", menuName = "Scriptable Objects/CustomEffects/Effect_Instakill")]
public class Effect_Instakill : CustomEffectSO
{
    public override void OnApply()
    {
        base.OnApply();
        Debug.Log("Instakill");
    }
}
