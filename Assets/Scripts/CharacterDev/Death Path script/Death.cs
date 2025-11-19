using UnityEngine;
using System.Collections;
public class Death : MonoBehaviour
{
    private Enemy target;
    private float originalSpeed;
    private float duration = 3f;
    private float slowPercent = 0.5f;
    private int explosionDamage = 10;


    private GameObject explosionVFX;
    private AudioClip explosionSound;

    public void Initialize(Enemy enemy, float dur, float slow, int dmg, GameObject vfx, AudioClip sound)
    {
        target = enemy;
        duration = dur;
        slowPercent = slow;
        explosionDamage = dmg;
        explosionVFX = vfx;
        explosionSound = sound;

        StartCoroutine(StatusRoutine());
    }

    private IEnumerator StatusRoutine()
    {
             if (target != null)
        {
            originalSpeed = target.moveSpeed;
            target.moveSpeed *= (1f - slowPercent);
        }

 
        SpriteRenderer sr = target.GetComponentInChildren<SpriteRenderer>();

      
        float timer = 0f;
        while (timer < duration)
        {
            if (target == null || target.IsDead()) break; 


            if (sr != null && sr.color != Color.red)
            {
                sr.color = Color.magenta;
            }

            timer += Time.deltaTime;
            yield return null; 
        }

   
        if (target != null && !target.IsDead())
        {
            
            target.moveSpeed = originalSpeed;

         
            if (sr != null) sr.color = Color.white;

           
            target.TakeDamage(explosionDamage);

            
            if (explosionVFX != null) Instantiate(explosionVFX, target.transform.position, Quaternion.identity);
            if (explosionSound != null) AudioSource.PlayClipAtPoint(explosionSound, target.transform.position);
        }

        Destroy(this);
    }
}
