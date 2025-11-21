using UnityEngine;
using System.Collections;

public class FinalBoss : Enemy
{
    [Header("Phase 1 Settings")]
    [SerializeField] private int phase1Health = 1500;
    [SerializeField] private float attackRange = 12f;
    [SerializeField] private float shootCooldown = 2.0f;
    [SerializeField] private float stopDistance = 4f;

    [Header("Phase 1 Projectiles")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileScale = 1.5f;
    [SerializeField] private int projectileCount = 3;
    [SerializeField] private float spreadAngle = 30f;
    [SerializeField] private float offsetFeet = -1.5f;
    [SerializeField] private float offsetChest = 0f;

    [Header("Phase 2 Settings")]
    [SerializeField] private int phase2Health = 2000;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float laserDuration = 3.0f;
    [SerializeField] private float bigLaserScale = 3.0f;
    [SerializeField] private float offsetHead = 1.5f;
    [SerializeField] private float laserLength = 30.0f;

    private float shootTimer;
    private bool isShooting = false;
    private bool isPhaseTwo = false;
    private bool isTransitioning = false;

    // ตัวแปรอมตะเฉพาะของบอส
    private bool isBossInvincible = false;

    protected override void Awake()
    {
        base.Awake();
        base.Initialized(phase1Health);
        DamageHit = 50;
        moveSpeed = 3.5f;
        if (rb != null) rb.mass = 100;
    }

    protected override void Start()
    {
        base.Start();
        transform.localScale = Vector3.one * 2.5f;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        isPhaseTwo = false;
        isTransitioning = false;
        isBossInvincible = false;
        isShooting = false;

        base.Initialized(phase1Health);
        transform.localScale = Vector3.one * 2.5f;
        xpDrop = 2000;
    }

    // 🛠️ Override TakeDamage เพื่อเช็คอมตะตอนเปลี่ยนร่าง
    public override void TakeDamage(int damageAmount)
    {
        if (isBossInvincible) return; // ถ้ากำลังแปลงร่าง ไม่โดนดาเมจ

        base.TakeDamage(damageAmount);
    }

    protected override void Die()
    {
        if (!isPhaseTwo)
        {
            // ตัดจบทุกอย่างเพื่อแปลงร่าง
            StopAllCoroutines();

            if (animator != null)
            {
                animator.ResetTrigger("Attack");
                // animator.ResetTrigger("Attack2"); // ถ้ามี
            }

            StartCoroutine(EnterPhaseTwoRoutine());
        }
        else
        {
            base.Die(); // ตายจริง
        }
    }

    public override void Move()
    {
        if (isTransitioning || playerTransform == null) return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);

        if (shootTimer > 0) shootTimer -= Time.deltaTime;

        if (dist <= attackRange && shootTimer <= 0 && !isShooting)
        {
            if (!isPhaseTwo)
            {
                StartCoroutine(AttackPhase1());
            }
            else
            {
                StartCoroutine(AttackPhase2());
            }
        }

        if (!isShooting)
        {
            if (dist > stopDistance)
            {
                PhysicsMove(false);
            }
            else
            {
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }
        }
    }

    private IEnumerator EnterPhaseTwoRoutine()
    {
        isTransitioning = true;
        isBossInvincible = true; // เปิดอมตะ
        isShooting = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // เล่นท่าแปลงร่าง
        if (animator != null)
        {
            // ใช้ Play เพื่อ Force เปลี่ยนท่าทันที
            animator.Play("PhaseDefender");
        }

        Debug.Log("BOSS ENTERING PHASE 2!");

        MaxHealth = phase2Health;
        Health = 0;

        float regenTime = 2.0f;
        float timer = 0f;
        while (timer < regenTime)
        {
            timer += Time.deltaTime;
            float progress = timer / regenTime;
            Health = Mathf.RoundToInt(Mathf.Lerp(0, phase2Health, progress));
            yield return null;
        }

        Health = phase2Health;
        isPhaseTwo = true;
        isTransitioning = false;
        isBossInvincible = false; // ปิดอมตะ

        shootTimer = 1.0f; // รอแปปนึงค่อยเริ่มยิง
    }

    private IEnumerator AttackPhase1()
    {
        isShooting = true;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (animator != null) animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);
        ShootProjectile(offsetFeet);

        yield return new WaitForSeconds(2.0f);

        if (animator != null) animator.SetTrigger("Attack");
        ShootProjectile(offsetChest);

        yield return new WaitForSeconds(1.0f);
        shootTimer = shootCooldown;
        isShooting = false;
    }

    private IEnumerator AttackPhase2()
    {
        isShooting = true;
        if (rb != null) rb.linearVelocity = Vector2.zero;

      
        if (animator != null) animator.SetTrigger("Attack2");

        yield return new WaitForSeconds(0.8f);

        // ยิงเลเซอร์ที่หัว
        FireLaser(offsetHead, bigLaserScale);

        yield return new WaitForSeconds(laserDuration + 0.5f);

        shootTimer = shootCooldown;
        isShooting = false;
    }

    private void ShootProjectile(float yOffset)
    {
        if (projectilePrefab == null) return;

        Vector2 mainDir = (playerTransform.position - transform.position).normalized;
        float angleStep = (projectileCount > 1) ? spreadAngle / (projectileCount - 1) : 0;
        float currentAngle = -spreadAngle / 2f;
        Vector3 spawnPos = transform.position + new Vector3(0, yOffset, 0);

        for (int i = 0; i < projectileCount; i++)
        {
            Vector2 shootDir = Quaternion.Euler(0, 0, currentAngle) * mainDir;
            GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            proj.transform.localScale = Vector3.one * projectileScale;

            GolemProjectile gp = proj.GetComponent<GolemProjectile>();
            if (gp != null) gp.Init(shootDir, DamageHit);

            currentAngle += angleStep;
        }
    }

    private void FireLaser(float yOffset, float thicknessMultiplier = 1f)
    {
        if (laserPrefab == null) return;

        Vector3 spawnPos = transform.position + new Vector3(0, yOffset, 0);
        GameObject laser = Instantiate(laserPrefab, spawnPos, Quaternion.identity);

      
        laser.transform.SetParent(this.transform);

      
        Vector3 newScale = laser.transform.localScale;
        newScale.x = laserLength; 
        newScale.y *= thicknessMultiplier; 
        laser.transform.localScale = newScale;

        Destroy(laser, laserDuration);
    }
}