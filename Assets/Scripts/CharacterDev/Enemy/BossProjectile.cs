using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector2 direction;
    private int damage;
    private float speed = 10f;
    private float lifeTime = 5f;

    public void Init(Vector2 dir, int dmg)
    {
        direction = dir;
        damage = dmg;

        // หมุนหัวกระสุนไปทางทิศทางนั้น
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ชนผู้เล่น (Prince)
        Prince p = other.GetComponent<Prince>();
        if (p != null)
        {
            p.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}