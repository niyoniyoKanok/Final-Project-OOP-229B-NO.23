using UnityEngine;

public class StarProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 2f;
    private int damage;
    private Vector2 direction;

    public void Init(int dmg, Vector2 dir)
    {
        this.damage = dmg;
        this.direction = dir.normalized;
        Destroy(gameObject, lifetime);

        // หมุนภาพตามทิศทาง
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy e = other.GetComponent<Enemy>();
        if (e != null && !e.IsDead())
        {
            e.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}