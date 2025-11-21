using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurseShout : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float radius = 10f;
    [SerializeField] private float pullForce = 15f;
    [SerializeField] private float stunDuration = 2.0f;
    [SerializeField] private float shoutDuration = 0.5f; 

    [Header("Visuals")]
    [SerializeField] private GameObject shoutVFX; 
    [SerializeField] private AudioClip shoutSound;

    private Prince princeOwner;
    private int damage;


    public void InitWeapon(int dmg, Prince owner)
    {
        this.damage = dmg;
        this.princeOwner = owner;

        StartCoroutine(ShoutRoutine());
    }

    private IEnumerator ShoutRoutine()
    {
        // 1. เล่นเสียงและ Effect
        if (shoutSound != null) AudioSource.PlayClipAtPoint(shoutSound, transform.position);
        if (shoutVFX != null)
        {
            GameObject vfx = Instantiate(shoutVFX, transform.position, Quaternion.identity);
            vfx.transform.SetParent(this.transform); 
            Destroy(vfx, 2f);
        }

        float timer = 0f;
        List<Enemy> affectedEnemies = new List<Enemy>();

   
        while (timer < shoutDuration)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

            foreach (var hit in hits)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null && !enemy.IsDead())
                {
                    
                    Vector2 direction = (transform.position - enemy.transform.position).normalized;

                 
                  
                    Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                    if (enemyRb != null)
                    {
                    
                        enemyRb.AddForce(direction * pullForce * Time.deltaTime, ForceMode2D.Impulse);
                    }
                    else
                    {
                 
                        enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, transform.position, pullForce * Time.deltaTime);
                    }

                  
                    if (!affectedEnemies.Contains(enemy))
                    {
                        affectedEnemies.Add(enemy);
                        ApplyCurse(enemy);
                    }
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }


        Destroy(gameObject);
    }

    private void ApplyCurse(Enemy enemy)
    {

        enemy.TakeDamage(damage);

        enemy.ApplyStun(stunDuration);


        Death status = enemy.GetComponent<Death>();
        if (status == null)
        {
            status = enemy.gameObject.AddComponent<Death>();
      
            status.Initialize(enemy, 5f, 0.5f, damage, null, null);
        }
        else
        {
     
           
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}