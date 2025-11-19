using UnityEngine;
using System.Collections;

public class VampireScratch : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private float timeBetweenTicks = 0.5f;

  

    [Header("Effects")]
    [SerializeField] private GameObject bleedVFX; 
    [SerializeField] private AudioClip scratchSound; 
  

    private int damage;
    private int healAmount;
    private Prince shooter;

    public void Init(int damagePerTick, int healPerTick, Prince princeScript)
    {
        this.damage = damagePerTick;
        this.healAmount = healPerTick;
        this.shooter = princeScript;

        StartCoroutine(ScratchRoutine());
    }

    private IEnumerator ScratchRoutine()
    {
       
        DealDamageAndHeal();

        yield return new WaitForSeconds(timeBetweenTicks);

        DealDamageAndHeal();

        Destroy(gameObject, 0.5f);
    }

    private void DealDamageAndHeal()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        bool didHitEnemy = false;

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead())
            {
              
                enemy.TakeDamage(damage);
                didHitEnemy = true;

              
                if (bleedVFX != null)
                {
                   
                    Instantiate(bleedVFX, enemy.transform.position, Quaternion.identity);
                }

                if (scratchSound != null)
                {
                
                    AudioSource.PlayClipAtPoint(scratchSound, enemy.transform.position);
                }
               
            }
        }

        if (didHitEnemy && shooter != null)
        {
            shooter.Ability3Heal(healAmount);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}