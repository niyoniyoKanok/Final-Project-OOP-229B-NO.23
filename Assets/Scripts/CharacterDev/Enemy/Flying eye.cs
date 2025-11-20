using UnityEngine;
using System.Collections;

public class FlyingEye : Enemy
{
    [Header("FlyingEye Settings")]
    [SerializeField] private float idealDistance = 6f;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float flyHeight = 2.0f;

    [SerializeField] private float attackAnimDelay = 0.4f;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;

    private float attackTimer;
    private bool isAttacking = false;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void ResetState()
    {
        base.ResetState();

        // Flying unit specifics
        canFly = true;

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f; // no gravity for flyers
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        isAttacking = false;
        attackTimer = 0f;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        base.Initialized(30);
        moveSpeed = 2.5f;

        // do not change gravity here (ResetState already set it)
    }

    public override void Move()
    {
        if (attackTimer > 0) attackTimer -= Time.deltaTime;
        if (isAttacking) return;
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        Vector3 targetPos = playerTransform.position;
        targetPos.y += flyHeight;
        Vector2 direction = (targetPos - transform.position).normalized;

        if (distanceToPlayer > idealDistance)
        {
            PhysicsMove(true);
        }
        else if (distanceToPlayer < idealDistance - 0.5f)
        {
            // move away a little
            if (rb != null)
            {
                Vector2 newPos = rb.position - (direction * moveSpeed * Time.fixedDeltaTime);
                rb.MovePosition(newPos);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.position - (Vector3)direction, moveSpeed * Time.deltaTime);
            }
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

        if (animator != null) animator.SetTrigger("Attack");

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
