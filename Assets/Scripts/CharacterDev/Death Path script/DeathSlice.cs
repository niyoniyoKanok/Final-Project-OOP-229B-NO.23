using UnityEngine;
using System.Collections;

public class DeathSlice : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;       
    [SerializeField] private AudioClip doubleHitSound;

    private int damage;

    public void Init(Enemy target, int dmg)
    {
        this.damage = dmg;
        StartCoroutine(SliceRoutine(target));
    }

    private IEnumerator SliceRoutine(Enemy target)
    {
        if (target == null || target.IsDead())
        {
            Destroy(gameObject);
            yield break;
        }

       
        bool isBonusActive = false;

        Death deathStatus = target.GetComponent<Death>();
        DeathSliceTracker tracker = target.GetComponent<DeathSliceTracker>();

      
        if (deathStatus != null && tracker != null && (Time.time - tracker.lastHitTime < 5f))
        {
            isBonusActive = true;
        }

      
        if (tracker == null) tracker = target.gameObject.AddComponent<DeathSliceTracker>();
        tracker.lastHitTime = Time.time;


        if (isBonusActive)
        {
            
            if (target != null && !target.IsDead())
            {
                target.TakeDamage(damage);
                target.TakeDamage(damage); 


                if (doubleHitSound != null)
                {
                    AudioSource.PlayClipAtPoint(doubleHitSound, transform.position);
                }

                Debug.Log("Death Slice: INSTANT DOUBLE HIT!");
            }
        }
        else
        {

            if (target != null && !target.IsDead())
            {
                target.TakeDamage(damage);

                
                if (hitSound != null)
                {
                    AudioSource.PlayClipAtPoint(hitSound, transform.position);
                }
            }
        }

     
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}