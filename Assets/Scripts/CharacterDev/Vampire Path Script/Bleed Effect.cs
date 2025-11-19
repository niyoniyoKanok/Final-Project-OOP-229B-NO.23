using UnityEngine;
using System.Collections;
public class BleedEffect : MonoBehaviour
{
    private Character target;
    private int damagePerTick;
    private int ticks = 3;
    private float duration = 2f;

    private GameObject vfxPrefab;
    private AudioClip soundClip;
    public void Initialize(Character enemy, int totalDamage, GameObject vfx, AudioClip clip)
    {
        target = enemy;
        damagePerTick = Mathf.CeilToInt((float)totalDamage / ticks);

        
        vfxPrefab = vfx;
        soundClip = clip;

     
        StopAllCoroutines();
        StartCoroutine(BleedRoutine());
    }

    private IEnumerator BleedRoutine()
    {
        float timeBetweenTicks = duration / ticks;

        for (int i = 0; i < ticks; i++)
        {
            if (target != null && !target.IsDead())
            {
            
                target.TakeDamage(damagePerTick);


              
                if (vfxPrefab != null)
                {
                 
                    Vector3 spawnPos = target.transform.position + new Vector3(0, 0.5f, 0);
                    Instantiate(vfxPrefab, spawnPos, Quaternion.identity);
                }

          
                if (soundClip != null)
                {
                   
                    AudioSource.PlayClipAtPoint(soundClip, target.transform.position);
                }
            }
            else
            {
                break;
            }

            yield return new WaitForSeconds(timeBetweenTicks);
        }

        Destroy(this); 
    }
}