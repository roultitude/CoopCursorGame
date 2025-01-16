using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class EnemyContactDamageHandler : MonoBehaviour 
{
    [SerializeField]
    Animator animator;
    [SerializeField]
    bool isManuallyControlled =false;
    [SerializeField]
    float cooldown, windupDuration, damageModeDuration;
    [SerializeField]
    Color dmgModeColor, windupModeColor;

    [SerializeField]
    Enemy enemy;

    bool isCycleActive = true;
    bool isDamageActive = false;
    Material mat;
    public void Start()
    {
        if(!isManuallyControlled) StartCoroutine(StartCycle());
        mat = GetComponent<Enemy>().sprite.material;
    }

    public void SetContactDamageState(bool isEnabled)
    {
        if (isEnabled)
        {
            //animator.CrossFade("ContactDamageMode", 0);
            mat.SetColor("_HologramStripeColor", dmgModeColor);
            mat.SetFloat("_HologramBlend", 1f);
            isDamageActive = true;
            if (enemy) enemy.ChangeVulnerable(false);
        } else
        {
            //animator.CrossFade("IdleBase", 0);
            mat.SetFloat("_HologramBlend", 0f);
            isDamageActive = false;
            if (enemy) enemy.ChangeVulnerable(true);
        }
    }

    private IEnumerator StartCycle()
    {
        while (isCycleActive)
        {
            Debug.Log($"{name} 1: starting cycle, waiting for {cooldown}");
            yield return new WaitForSeconds(cooldown);

            Debug.Log($"{name} 2: windup , waiting for {windupDuration}");
            
            mat.SetColor("_HologramStripeColor", windupModeColor);
            mat.SetFloat("_HologramBlend", 1f);
            //animator.CrossFade("ContactDamageWindup", windupDuration);
            yield return new WaitForSeconds(windupDuration);

            Debug.Log($"{name} 3: damage active , waiting for {damageModeDuration}");
            SetContactDamageState(true);
            yield return new WaitForSeconds(damageModeDuration);

            Debug.Log($"{name} 4: done, resetting animator");
            SetContactDamageState(false);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Player player = col.GetComponent<Player>();

            if (!isDamageActive || !player) return; // if not damage mode or if no player found
            
            Debug.Log($"Player {col.GetComponent<NetworkObject>().OwnerClientId} hit by {name}");
            player.OnHurt();

        }
    }

}
