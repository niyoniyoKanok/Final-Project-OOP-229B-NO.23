using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeathSlice : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip chainSound; 

    private int damage;
    private Prince princeOwner;
    private float chainRadius = 100.0f;
    private HashSet<Enemy> visitedEnemies = new HashSet<Enemy>(); 

    public void Init(Enemy target, int dmg, Prince owner)
    {
        this.damage = dmg;
        this.princeOwner = owner;

      
        StartCoroutine(ProcessSlice(target));
    }

    private IEnumerator ProcessSlice(Enemy initialTarget)
    {
       
        if (initialTarget == null || initialTarget.IsDead())
        {
            Destroy(gameObject);
            yield break;
        }

      
        Death deathStatus = initialTarget.GetComponent<Death>();

        if (deathStatus != null)
        {
          
            yield return StartCoroutine(ChainReactionRoutine(initialTarget));
        }
        else
        {
           
            DealDamage(initialTarget, false);
            Destroy(gameObject, 0.2f); 
        }
    }

    private IEnumerator ChainReactionRoutine(Enemy startTarget)
    {
    
        Queue<Enemy> targetsToSlash = new Queue<Enemy>();
        targetsToSlash.Enqueue(startTarget);

        while (targetsToSlash.Count > 0)
        {
            Enemy currentTarget = targetsToSlash.Dequeue();

      
            if (currentTarget == null || currentTarget.IsDead() || visitedEnemies.Contains(currentTarget)) continue;


            visitedEnemies.Add(currentTarget);

            transform.position = currentTarget.transform.position;

            DealDamage(currentTarget, true);

            FindNextTargets(currentTarget.transform.position, targetsToSlash);

          
            yield return new WaitForSeconds(0.15f);
        }

        Destroy(gameObject);
    }

    private void FindNextTargets(Vector3 center, Queue<Enemy> queue)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, chainRadius);

        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();

           
            if (enemy != null && !enemy.IsDead() && enemy.GetComponent<Death>() != null && !visitedEnemies.Contains(enemy))
            {
             
                if (!queue.Contains(enemy))
                {
                    queue.Enqueue(enemy);
                }
            }
        }
    }

    private void DealDamage(Enemy target, bool isExecute)
    {
        if (target == null) return;

       

        if (isExecute)
        {
       
            Debug.Log($"<color=red>DEATH SLICE EXECUTE: {target.name}</color>");

       
            if (chainSound != null) AudioSource.PlayClipAtPoint(chainSound, transform.position);
            else if (hitSound != null) AudioSource.PlayClipAtPoint(hitSound, transform.position);


            target.TakeDamage(99999);

           
            if (princeOwner != null)
            {
                int healAmount = Mathf.RoundToInt(damage / 2f);
                princeOwner.Ability3Heal(healAmount);
            }
        }
        else
        {
           
            if (hitSound != null) AudioSource.PlayClipAtPoint(hitSound, transform.position);
            target.TakeDamage(damage);
        }
    }
}