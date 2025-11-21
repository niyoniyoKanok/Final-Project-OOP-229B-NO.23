using UnityEngine;
using System.Collections;

public class FinalBoss : Enemy
{
    [Header("Final Boss Mechanics")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float attackRange = 12f;
    [SerializeField] private float shootCooldown = 2.0f;
    [SerializeField] private float stopDistance = 4f;

    [Header("Animation Timing")]
    [SerializeField] private float windUpTime = 0.5f;

    [Tooltip("เวลารอระหว่างการโจมตีครั้งที่ 1 และ 2")]
    [SerializeField] private float intervalTime = 2.0f;

    [Header("Projectile Settings")]
    [SerializeField] private int projectileCount = 1;
    [SerializeField] private float spreadAngle = 30f;

    [SerializeField] private float offsetFeet = -1.5f;
    [SerializeField] private float offsetChest = 0f;

    private float shootTimer;
    private bool isShooting = false;

   
    protected override void Awake() { base.Awake(); DamageHit = 50; moveSpeed = 3.5f; if (rb != null) rb.mass = 100; }
    protected override void Start() { base.Start(); transform.localScale = Vector3.one * 2.5f; }
    public override void Move()
    {
        if (playerTransform == null) return;
        float dist = Vector2.Distance(transform.position, playerTransform.position);
        if (shootTimer > 0) shootTimer -= Time.deltaTime;
        if (dist <= attackRange && shootTimer <= 0 && !isShooting) StartCoroutine(AttackRoutine());
        if (!isShooting) { if (dist > stopDistance) PhysicsMove(false); else if (rb != null) rb.linearVelocity = Vector2.zero; }
    }
    protected override void OnEnable() { base.OnEnable(); base.Initialized(2000); xpDrop = 2000; transform.localScale = Vector3.one * 2.5f; isShooting = false; }
    private IEnumerator AttackRoutine()
    {
        isShooting = true;
        if (rb != null) rb.linearVelocity = Vector2.zero;

       
        if (animator != null) animator.SetTrigger("Attack");


        yield return new WaitForSeconds(windUpTime);

        ShootProjectile(offsetFeet); 

     
        yield return new WaitForSeconds(intervalTime);

       
        if (animator != null) animator.SetTrigger("Attack");

        yield return new WaitForSeconds(windUpTime);

        ShootProjectile(offsetChest); 

        yield return new WaitForSeconds(1.0f);

        shootTimer = shootCooldown;
        isShooting = false;
    }


    private void ShootProjectile(float yOffset)
    {
        if (projectilePrefab != null && playerTransform != null)
        {
            Vector2 mainDir = (playerTransform.position - transform.position).normalized;

            float angleStep = (projectileCount > 1) ? spreadAngle / (projectileCount - 1) : 0;
            float currentAngle = -spreadAngle / 2f;

           
            Vector3 spawnPos = transform.position + new Vector3(0, yOffset, 0);

            for (int i = 0; i < projectileCount; i++)
            {
                Vector2 shootDir = Quaternion.Euler(0, 0, currentAngle) * mainDir;

                GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
             

                GolemProjectile gp = proj.GetComponent<GolemProjectile>();
                if (gp != null)
                {
                    gp.Init(shootDir, DamageHit);
                }

                currentAngle += angleStep;
            }
        }
    }
}