using UnityEngine;
using System.Collections; // จำเป็นสำหรับการใช้ IEnumerator

public class FlyingEye : Enemy
{
    [Header("FlyingEye Settings")]
    [SerializeField] private float idealDistance = 6f;
    [SerializeField] private float attackCooldown = 3f;

   
    [SerializeField] private float attackAnimDelay = 0.4f;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;

    private float attackTimer;
    private bool isAttacking = false; 

    protected override void Awake()
    {
        base.Awake();
        base.Initialized(30);
        moveSpeed = 2.5f;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void Move()
    {
      
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

       
        if (isAttacking) return;

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
                
                StartCoroutine(AttackRoutine());
                attackTimer = attackCooldown;
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true; 

        
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

   yield return new WaitForSeconds(attackAnimDelay);

 
        SpawnProjectile();

   
        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }

    private void SpawnProjectile()
    {
        if (projectilePrefab == null || playerTransform == null) return;

        Vector2 direction = (playerTransform.position - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Instantiate(projectilePrefab, transform.position, rotation);
    }
}