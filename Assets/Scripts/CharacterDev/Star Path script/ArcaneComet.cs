using UnityEngine;

public class ArcaneComet : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float aoeRadius = 3.5f;

    private int damage;
    private bool hasExploded = false;

    public void Init(int dmg)
    {
        this.damage = dmg;
        Destroy(gameObject, 4f);
    }

    void Update()
    {
        if (!hasExploded)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasExploded) return;

     
        Enemy enemy = other.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            Explode();
        }
    }

    private void Explode()
    {
        hasExploded = true;

     
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aoeRadius);

        foreach (var hit in hits)
        {
            Enemy hitEnemy = hit.GetComponentInParent<Enemy>();
            if (hitEnemy != null && !hitEnemy.IsDead())
            {
                hitEnemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}