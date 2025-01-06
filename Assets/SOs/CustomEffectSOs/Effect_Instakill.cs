using UnityEngine;

[CreateAssetMenu(fileName = "Effect_Instakill", menuName = "Scriptable Objects/CustomEffects/Effect_Instakill")]
public class Effect_Instakill : CustomEffectSO
{
    [SerializeField]
    float chance;
    public override void OnHitEnemy(Player player, Enemy enemy, bool isCrit)
    {
        base.OnHitEnemy(player,enemy,isCrit);
        if(Random.Range(0f, 1f) < chance)
        {
            if (enemy.GetComponent<BossMinionController>() || enemy is Boss)
            {
                Debug.Log($"Cant Instakill Boss / BossMinion {enemy.gameObject.name}");
            }
            else
            {
                Debug.Log($"Instakill: {enemy.gameObject.name}");
                enemy.TakeDamage(9999);
            }
            
        }
        
    }
}
