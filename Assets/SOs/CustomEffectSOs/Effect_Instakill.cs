using UnityEngine;

[CreateAssetMenu(fileName = "Effect_Instakill", menuName = "Scriptable Objects/CustomEffects/Effect_Instakill")]
public class Effect_Instakill : CustomEffectSO
{
    [SerializeField]
    float chance;
    public override HitInfo OnHitEnemy(TickableTimer timer, Player player, Enemy enemy, HitInfo hit)
    {
        if(Random.Range(0f, 1f) < chance)
        {
            if (enemy.GetComponent<BossMinionController>() || enemy is Boss)
            {
                Debug.Log($"Cant Instakill Boss / BossMinion {enemy.gameObject.name}");
            }
            else
            {
                Debug.Log($"Instakill: {enemy.gameObject.name}");
                hit.MutateDamage((float dmg) => dmg = 9999f);
            }
        }

        return hit;
    }
}
