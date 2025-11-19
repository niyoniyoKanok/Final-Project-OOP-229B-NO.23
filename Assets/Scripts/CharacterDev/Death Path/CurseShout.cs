using UnityEngine;

public class CurseShout : Weapon
{
    [Header("Settings")]
    [SerializeField] private float knockbackForce = 15f;
    [SerializeField] private float lifeTime = 0.5f; 

    [Header("Death Status")]
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private int explosionDamage = 20;


    private Vector3 initialScale;
    [SerializeField] private float finalScaleMultiplier = 2f; 

    void Start()
    {
        initialScale = transform.localScale;
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        Move();
    }

    public override void Move()
    {
     
        float progress = 1f - (lifeTime / 0.5f); 

        transform.localScale = Vector3.Lerp(initialScale, initialScale * finalScaleMultiplier, 0.1f);
    }

    public override void OnHitWith(Character character)
    {
        if (character is Enemy enemy)
        {
           
            enemy.TakeDamage(this.damage);

          
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
                enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            }

            Death status = enemy.GetComponent<Death>();
            if (status == null)
            {
                status = enemy.gameObject.AddComponent<Death>();
                status.Initialize(enemy, 3f, 0.5f, explosionDamage, explosionVFX, explosionSound);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Character character = other.GetComponent<Character>();
        if (character != null && character is Enemy)
        {
            OnHitWith(character);
        }
    }
}