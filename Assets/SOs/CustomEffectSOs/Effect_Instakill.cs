using UnityEngine;

[CreateAssetMenu(fileName = "Effect_Instakill", menuName = "Scriptable Objects/CustomEffects/Effect_Instakill")]
public class Effect_Instakill : CustomEffectSO
{
    [SerializeField]
    float chance;
    public override void OnHitEnemy(Enemy e)
    {
        base.OnHitEnemy(e);
        if(Random.Range(0f, 1f) < chance)
        {
            if (e.GetComponent<BossMinionController>() || e is Boss)
            {
                Debug.Log($"Cant Instakill Boss / BossMinion {e.gameObject.name}");
            }
            else
            {
                Debug.Log($"Instakill: {e.gameObject.name}");
                e.TakeDamage(9999);
            }
            
        }
        
    }
}
