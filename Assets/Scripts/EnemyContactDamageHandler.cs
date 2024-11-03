using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class EnemyContactDamageHandler : MonoBehaviour 
{
    [SerializeField]
    Animator animator;

    [SerializeField]
    float contactDamage, cooldown, windupDuration, damageModeDuration;

    [SerializeField]
    Enemy enemy;

    bool isCycleActive = true;
    bool isDamageActive = false;
    public void Start()
    {
        StartCoroutine(StartCycle());
    }

    private IEnumerator StartCycle()
    {
        while (isCycleActive)
        {
            Debug.Log($"{name} 1: starting cycle, waiting for {cooldown}");
            yield return new WaitForSeconds(cooldown);

            Debug.Log($"{name} 2: windup , waiting for {windupDuration}");
            animator.CrossFade("ContactDamageWindup", windupDuration);
            yield return new WaitForSeconds(windupDuration);

            Debug.Log($"{name} 3: damage active , waiting for {damageModeDuration}");
            animator.CrossFade("ContactDamageMode", 0);
            isDamageActive = true;
            if (enemy) enemy.ChangeVulnerable(false);
            yield return new WaitForSeconds(damageModeDuration);

            Debug.Log($"{name} 4: done, resetting animator");
            animator.CrossFade("IdleBase", 0);
            isDamageActive = false;
            if (enemy) enemy.ChangeVulnerable(true);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Player player = col.GetComponent<Player>();

            if (!isDamageActive || !player) return; // if not damage mode or if no player found
            
            Debug.Log($"Player {col.GetComponent<NetworkObject>().OwnerClientId} hit by {name}");
            player.OnHit();

        }
    }

}
