using UnityEngine;

public class FlyingEye : Enemy 
{
    [Header("FlyingEye Settings")]
    [SerializeField] private float idealDistance = 6f;
    [SerializeField] private float attackCooldown = 3f;
    
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;

    private float attackTimer;
    protected override void Start()
    {
        base.Start();
        base.Initialized(30);
        moveSpeed = 2.5f; 



      
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
        }
    }

    public override void Move()
    {

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > idealDistance)
        {

            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
        }
        else if (distanceToPlayer < idealDistance - 0.5f)
        {

            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, -moveSpeed * Time.deltaTime);
        }

        else
        {
            if (attackTimer <= 0)
            {
                ShootAtPlayer();
                attackTimer = attackCooldown; 
            }
        }

    }
    private void ShootAtPlayer()
    {
        

      
        Vector2 direction = (playerTransform.position - transform.position).normalized;

      
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);


        Instantiate(projectilePrefab, transform.position, rotation);
    }

}