using UnityEngine;
using System.Collections;

public class VampireShield : MonoBehaviour
{
    private Prince owner;
    private int damagePerTick;
    private float tickRate = 0.1f;
    private float radius = 3f; 

    public void Init(Prince prince, int damage, float duration, float sizeRadius)
    {
        this.owner = prince;
        this.damagePerTick = damage;
        this.radius = sizeRadius;

        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;


        Destroy(gameObject, duration);

        StartCoroutine(ShieldLoop());
    }

    private IEnumerator ShieldLoop()
    {
        while (true)
        {
            DealDamageAndHeal();
            yield return new WaitForSeconds(tickRate);
        }
    }

    private void DealDamageAndHeal()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        int totalHeal = 0;
        bool hitAny = false;

        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead())
            {
                
                enemy.TakeDamage(damagePerTick);


                totalHeal += damagePerTick;
                hitAny = true;
            }
        }

        
        if (hitAny && owner != null && totalHeal > 0)
        {
            owner.Ability3Heal(totalHeal);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f); 
        Gizmos.DrawSphere(transform.position, radius);
    }
}