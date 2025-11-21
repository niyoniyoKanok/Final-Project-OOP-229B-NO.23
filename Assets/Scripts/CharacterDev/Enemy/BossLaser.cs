using UnityEngine;

public class BossLaser : MonoBehaviour
{
    [SerializeField] private int damage = 30;
    [SerializeField] private float tickRate = 0.2f;

    private float nextDamageTime = 0f;

    private void OnTriggerStay2D(Collider2D other)
    {
        Prince p = other.GetComponent<Prince>();

        if (p != null)
        {

            if (Time.time >= nextDamageTime)
            {
                p.TakeDamage(damage);

                nextDamageTime = Time.time + tickRate;
            }
        }
    }
}