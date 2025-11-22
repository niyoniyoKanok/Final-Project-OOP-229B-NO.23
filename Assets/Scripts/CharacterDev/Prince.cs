using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public enum PathType
{
    None,       
    Vampire,    
    Death,      
    Star    
}
public class Prince : Character, IShootable
{
    [Header("Dependencies")]
    public PlayerLevel playerLevel;
    public GameManager GameManager;
    public Timer timer;

    [Header("--- Star Path ---")]
    public AudioClip starRepeatSound;

    public AudioClip darkStarSound;

    [Header("Star Path: Passive 2")]
    public GameObject darkStarPrefab;
    public float darkStarChance = 0.4f;
    private GameObject activeDarkStar;

    [Header("--- Star Path Spammer Upgrades ---")]
    public GameObject starFragmentPrefab;

    [Header("Skill 1: Star Impact")]
    public GameObject starImpactPrefab;
    public float starImpactInterval = 3f;
    public float starImpactRadius = 4f;

    [Header("Skill 2: Star Falling")]
    public GameObject starFallingPrefab;
    public float starFallingInterval = 4f;
    public float starFallingHeight = 5f;

    [Header("Skill 3: Arcane Comet")]
    public GameObject arcaneCometPrefab;
    public float arcaneCometInterval = 6f;

    [Header("--- Death Path ---")]

    [Header("Skill 1: Death Circle")]
    public GameObject deathCirclePrefab;
    public float deathCircleInterval = 4f;
    public float deathCircleRange = 6f;

    [Header("Skill 2: Death Slice")]
    public GameObject deathSlicePrefab;
    public float deathSliceInterval = 3f;

    [Header("Skill 3: Curse Shout")]
    public GameObject curseShoutPrefab;
    public float curseShoutInterval = 5f;
    public Transform shoutPoint;

    [Header("--- Vampire Path ---")]

    [Header("Passive 1: Normal Attack Bleed")]
    public GameObject normalAttackBleedVFX;
    public AudioClip normalAttackBleedSound;

    public GameObject vampireShieldPrefab; 
    [Range(0f, 1f)] public float vampireShieldChance = 0.2f;
    public float vampireShieldDuration = 5f;
    public float vampireShieldRadius = 3f;
    private GameObject activeVampireShield; 

    [Header("Skill 1: Bat")]
    public GameObject batPrefab;
    public float batSpawnInterval = 2f;

    [Header("Skill 2: Vampire Blade")]
    public GameObject vampireBladePrefab;
    public float vampireBladeInterval = 3f;

    [Header("Skill 3: Vampire Scratch")]
    public GameObject vampireScratchPrefab;
    public float vampireScratchInterval = 5f;
    public float scratchSearchRadius = 8f;

    [Header("Player Stats (Bonuses)")]
    public int BonusAttackDamage = 0;
    public float BonusCooldownReduction = 0f;
    public int BonusPotionHeal = 0;
    public float BonusAttackSpeed = 1f;
    public int BonusSwordWaveDamage = 0;
    public int BonusMaxHealth = 0;

    [Header("Player Stats (Bases)")]
    public int BaseAttackDamage = 20;
    public int BasePotionHeal = 10;

    [field: SerializeField] public Transform ShootPoint { get; set; }

    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 1f;
    private bool isInvincible = false;
    private Coroutine invincibilityCoroutine;

    public int SwordWaveDamage = 30;
    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 10f;

    [Header("Map Boundaries")]
    [SerializeField] private float minX = -66.44f;
    [SerializeField] private float maxX = 74.63f;
    [SerializeField] private float minY = -3.31f;
    [SerializeField] private float maxY = 10.65f;

    [Header("Game Dependencies")]
    public Camera PlayerCamera;

    [Header("Ability 1")]
    public Image AbilityImage1;
    public Text AbilityText1;
    public KeyCode Ability1Key;
    public float Ability1Cooldown = 7f;
    private Vector2 teleportTarget;
    public GameObject teleportVFX;

    [Header("Ability 2")]
    public Image AbilityImage2;
    public Text AbilityText2;
    public KeyCode Ability2Key;
    public float Ability2Cooldown = 5f;
    [field: SerializeField] public GameObject Bullet { get; set; }
    private PathType currentPathType = PathType.None;
    [Header("Ability 3")]
    public Image AbilityImage3;
    public Text AbilityText3;
    public KeyCode Ability3Key;
    public float Ability3Cooldown = 15f;

    [Header("AudioPanel")]
    public AudioSource AudioSource;
    public AudioClip HitSound;
    public AudioClip HealSound;
    public AudioClip TeleportSound;
    public AudioClip SwordWaveSound;

    private bool isAbility1Cooldown = false;
    private bool isAbility2Cooldown = false;
    private bool isAbility3Cooldown = false;

    private float currentAbility1Cooldown;
    private float currentAbility2Cooldown;
    private float currentAbility3Cooldown;

    private float effectiveAbility1Max;
    private float effectiveAbility2Max;
    private float effectiveAbility3Max;

    public EnemySpawner enemySpawner;

    [Header("AttackPanel")]
    public Transform AttackPoint;
    public float AttackRadius = 1f;
    public LayerMask AttackLayer;

    void Start()
    {
        Initialized(100);

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();


        if (AbilityImage1 != null) AbilityImage1.fillAmount = 0f;
        if (AbilityImage2 != null) AbilityImage2.fillAmount = 0f;
        if (AbilityImage3 != null) AbilityImage3.fillAmount = 0f;

        if (AbilityText1 != null) AbilityText1.text = "";
        if (AbilityText2 != null) AbilityText2.text = "";
        if (AbilityText3 != null) AbilityText3.text = "";

    }

    public void SetPathData(PathData data)
    {
        currentPathType = data.pathType;
        Debug.Log($"Prince selected path: {currentPathType}");


        switch (currentPathType)
        {
            case PathType.Vampire:
                StartCoroutine(BatSpawnerRoutine());
                StartCoroutine(VampireBladeSpawnerRoutine());
                StartCoroutine(VampireScratchSpawnerRoutine());
                break;

            case PathType.Death:
                StartCoroutine(DeathCircleSpawner());
                StartCoroutine(DeathSliceSpawner());
                StartCoroutine(CurseShoutSpawner());
                break;

            case PathType.Star:
                StartCoroutine(StarImpactSpawner());
                StartCoroutine(StarFallingSpawner());
                StartCoroutine(ArcaneCometSpawner());
                break;
        }
    }
    private bool CheckStarPassive()
    {

        bool isRepeat = Random.value <= 0.1f;
        if (isRepeat && AudioSource != null && starRepeatSound != null)
        {
            AudioSource.PlayOneShot(starRepeatSound);
            Debug.Log("Star Passive Triggered! Skill Repeated.");
        }
        return isRepeat;
    }

    private void TriggerStarPassives()
    {

        if (Random.value <= darkStarChance)
        {
            SpawnDarkStar();
        }
    }
    // -------Star Path---------------
    private void SpawnDarkStar()
    {

        if (activeDarkStar != null)
        {
            return;
        }


        if (darkStarPrefab != null)
        {
            if (darkStarSound != null && AudioSource != null)
            {
                AudioSource.PlayOneShot(darkStarSound);
            }
            activeDarkStar = Instantiate(darkStarPrefab, ShootPoint.position, Quaternion.identity);


            int dmg = Mathf.RoundToInt((SwordWaveDamage + BonusSwordWaveDamage) / 2f);

            DarkStar dsScript = activeDarkStar.GetComponent<DarkStar>();
            if (dsScript != null)
            {
                dsScript.InitWeapon(dmg, this);
            }

            Debug.Log("Passive Dark Star Activated!");
        }
    }


    private IEnumerator StarImpactSpawner()
    {
        while (true)
        {
            float actualInterval = GetReducedCooldown(starImpactInterval);
            yield return new WaitForSeconds(actualInterval);

            if (IsDead()) continue;

            if (playerLevel != null && playerLevel.CurrentLevel < 3) continue;

            CastStarImpact();


            TriggerStarPassives();


            if (CheckStarPassive())
            {
                yield return new WaitForSeconds(0.2f);
                CastStarImpact();

            }
        }
    }
    private void CastStarImpact()
    {


        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, starImpactRadius);


        int dmg = Mathf.RoundToInt((SwordWaveDamage + BonusSwordWaveDamage) * 0.25f);

        bool hitAny = false;

        foreach (var hit in hits)
        {

            if (hit == null || hit.gameObject == null) continue;

            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead())
            {
                hitAny = true;


                if (starImpactPrefab != null)
                {
                    Instantiate(starImpactPrefab, enemy.transform.position, Quaternion.identity);
                }


                enemy.TakeDamage(dmg);

                if (starFragmentPrefab != null)
                {
                    for (int i = 0; i < 2; i++) 
                    {
                        GameObject frag = Instantiate(starFragmentPrefab, enemy.transform.position, Quaternion.identity);
                        StarProjectile fragScript = frag.GetComponent<StarProjectile>();
                        if (fragScript != null)
                        {
                            
                            Vector2 randomDir = Random.insideUnitCircle.normalized;
                            fragScript.Init(Mathf.CeilToInt(dmg * 0.5f), randomDir);
                        }
                    }
                }
            }
        }


        if (!hitAny)
        {
            Debug.Log("No targets for Star Impact");
        }
    }

    private IEnumerator StarFallingSpawner()
    {
        while (true)
        {
            float actualInterval = GetReducedCooldown(starFallingInterval);
            yield return new WaitForSeconds(actualInterval);

            if (IsDead()) continue;

            if (playerLevel != null && playerLevel.CurrentLevel < 7) continue;

            CastStarFalling();
            TriggerStarPassives();

            if (CheckStarPassive())
            {
                yield return new WaitForSeconds(0.3f);
                CastStarFalling();
            }
        }
    }

    private void CastStarFalling()
    {
        if (starFallingPrefab == null) return;

        // 1. หาศัตรูรอบๆ (เพิ่มระยะค้นหาหน่อยจาก 10 เป็น 12-15)
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 12f);
        System.Collections.Generic.List<Transform> targets = new System.Collections.Generic.List<Transform>();

        foreach (var hit in hits)
        {
            // เช็คว่าเป็น Enemy และยังไม่ตาย
            if (hit.GetComponent<Enemy>() && !hit.GetComponent<Enemy>().IsDead())
                targets.Add(hit.transform);
        }

      
        int spamCount = 8;

        for (int i = 0; i < spamCount; i++)
        {
            Vector3 targetPos;

         
            if (targets.Count > 0)
            {
                Transform t = targets[Random.Range(0, targets.Count)];
                targetPos = t.position;
            }
            else
            {
                
                targetPos = transform.position;
            }

            
            float randomOffset = Random.Range(-2f, 2f);
            Vector3 spawnPos = targetPos + new Vector3(randomOffset, starFallingHeight, 0);

            GameObject star = Instantiate(starFallingPrefab, spawnPos, Quaternion.identity);

            
            int dmg = Mathf.RoundToInt((SwordWaveDamage + BonusSwordWaveDamage) * 0.25f);

            if (star.GetComponent<StarFalling>())
            {
                star.GetComponent<StarFalling>().Init(dmg);
            }
        }
    }

    private IEnumerator ArcaneCometSpawner()
    {
        while (true)
        {
            float actualInterval = GetReducedCooldown(arcaneCometInterval);
            yield return new WaitForSeconds(actualInterval);


            if (IsDead()) continue;

            if (playerLevel != null && playerLevel.CurrentLevel < 11) continue;

            CastArcaneComet();
            TriggerStarPassives();

            if (CheckStarPassive())
            {
                yield return new WaitForSeconds(0.5f);
                CastArcaneComet();
            }
        }
    }


    private void CastArcaneComet()
    {
        if (arcaneCometPrefab == null) return;

        Enemy target = FindRandomEnemy(10f);

        if (target != null)
        {

            Vector3 spawnPos = target.transform.position + new Vector3(0, 7f, 0);

            GameObject comet = Instantiate(arcaneCometPrefab, spawnPos, Quaternion.identity);


            comet.transform.localScale = new Vector3(2f, 2f, 1f);


            int dmg = Mathf.RoundToInt((SwordWaveDamage + BonusSwordWaveDamage) * 0.5f);
            comet.GetComponent<ArcaneComet>().Init(dmg);
        }
    }

    // ----------Death Path---------------
    private IEnumerator DeathCircleSpawner()
    {
        while (true)
        {
            float actualInterval = GetReducedCooldown(deathCircleInterval);
            yield return new WaitForSeconds(actualInterval);

            if (!IsDead() && deathCirclePrefab != null)
            {
                if (playerLevel != null && playerLevel.CurrentLevel < 3) continue;

                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, deathCircleRange);

                int dmg = Mathf.RoundToInt((SwordWaveDamage + BonusSwordWaveDamage) / 4f);
                int explodeDmg = dmg;

                foreach (var hit in hits)
                {

                    if (hit == null || hit.gameObject == null)
                    {
                        continue;
                    }


                    Enemy enemy = hit.GetComponent<Enemy>();


                    if (enemy != null && !enemy.IsDead() && enemy.GetComponent<Death>() == null)
                    {
                        GameObject circle = Instantiate(deathCirclePrefab, enemy.transform.position, Quaternion.identity);
                        circle.GetComponent<DeathCircle>().Init(enemy, dmg, explodeDmg);


                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
        }
    }

    private IEnumerator DeathSliceSpawner()
    {
        while (true)
        {
            float actualInterval = GetReducedCooldown(deathSliceInterval);
            yield return new WaitForSeconds(actualInterval);

            if (playerLevel != null && playerLevel.CurrentLevel < 7) continue;


            Enemy target = FindPriorityEnemy(30f);

            if (!IsDead() && target != null && deathSlicePrefab != null)
            {
                GameObject slice = Instantiate(deathSlicePrefab, target.transform.position, Quaternion.identity);

                int dmg = Mathf.RoundToInt((SwordWaveDamage + BonusSwordWaveDamage) / 2f);

                slice.GetComponent<DeathSlice>().Init(target, dmg, this);
            }
        }
    }


    private Enemy FindPriorityEnemy(float range)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

        System.Collections.Generic.List<Enemy> deathTargets = new System.Collections.Generic.List<Enemy>();
        System.Collections.Generic.List<Enemy> normalTargets = new System.Collections.Generic.List<Enemy>();

        foreach (var hit in hits)
        {
            Enemy e = hit.GetComponent<Enemy>();


            if (e != null && !e.IsDead())
            {

                if (e.GetComponent<Death>() != null)
                {
                    deathTargets.Add(e);
                }
                else
                {
                    normalTargets.Add(e);
                }
            }
        }

        if (deathTargets.Count > 0)
        {
            return deathTargets[Random.Range(0, deathTargets.Count)];
        }


        if (normalTargets.Count > 0)
        {
            return normalTargets[Random.Range(0, normalTargets.Count)];
        }

        return null;
    }

    private IEnumerator CurseShoutSpawner()
    {
        while (true)
        {
            float actualInterval = GetReducedCooldown(curseShoutInterval);
            yield return new WaitForSeconds(actualInterval);

            if (playerLevel != null && playerLevel.CurrentLevel < 11) continue;

            if (!IsDead() && curseShoutPrefab != null)
            {
                GameObject shout = Instantiate(curseShoutPrefab, ShootPoint.position, Quaternion.identity);


                shout.transform.SetParent(this.transform);


                CurseShout shoutScript = shout.GetComponent<CurseShout>();

                if (shoutScript != null)
                {
                    int dmg = Mathf.RoundToInt((SwordWaveDamage + BonusSwordWaveDamage) / 2f);
                    shoutScript.InitWeapon(dmg, this);
                }
            }
        }
    }

    private Enemy FindRandomEnemy(float range)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
        System.Collections.Generic.List<Enemy> enemies = new System.Collections.Generic.List<Enemy>();
        foreach (var hit in hits)
        {
            Enemy e = hit.GetComponent<Enemy>();
            if (e != null && !e.IsDead()) enemies.Add(e);
        }
        if (enemies.Count > 0) return enemies[Random.Range(0, enemies.Count)];
        return null;
    }

    // ----------Vampire Path--------------


   
    private void TryTriggerVampireShield()
    {
        if (currentPathType != PathType.Vampire || activeVampireShield != null) return;

        if (Random.value <= vampireShieldChance)
        {
            if (vampireShieldPrefab != null)
            {
                activeVampireShield = Instantiate(vampireShieldPrefab, transform.position, Quaternion.identity);

                
                activeVampireShield.transform.SetParent(this.transform);

              
                activeVampireShield.transform.localPosition = Vector3.zero;

                VampireShield shieldScript = activeVampireShield.GetComponent<VampireShield>();
                if (shieldScript != null)
                {
                    int shieldDmg = Mathf.RoundToInt((SwordWaveDamage + BonusSwordWaveDamage) / 3f);
                    if (shieldDmg < 1) shieldDmg = 1;

                    shieldScript.Init(this, shieldDmg, vampireShieldDuration, vampireShieldRadius);
                }
                Debug.Log("Vampire Shield Activated!");
            }
        }
    }
    private IEnumerator VampireScratchSpawnerRoutine()
    {
        while (true)
        {
            float actualInterval = GetReducedCooldown(vampireScratchInterval);
            yield return new WaitForSeconds(actualInterval);

            if (playerLevel != null && playerLevel.CurrentLevel < 11) continue;

            if (!IsDead() && vampireScratchPrefab != null)
            {
                SpawnVampireScratch();
            }
        }
    }

    private void SpawnVampireScratch()
    {

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, scratchSearchRadius);


        Transform targetEnemy = null;


        System.Collections.Generic.List<Transform> validEnemies = new System.Collections.Generic.List<Transform>();

        foreach (var col in enemies)
        {
            Enemy e = col.GetComponent<Enemy>();
            if (e != null && !e.IsDead())
            {
                validEnemies.Add(e.transform);
            }
        }


        if (validEnemies.Count > 0)
        {
            targetEnemy = validEnemies[Random.Range(0, validEnemies.Count)];
        }


        if (targetEnemy != null)
        {
            GameObject scratchObj = Instantiate(vampireScratchPrefab, targetEnemy.position, Quaternion.identity);

            VampireScratch scratchScript = scratchObj.GetComponent<VampireScratch>();
            if (scratchScript != null)
            {

                int totalSwordDamage = SwordWaveDamage + BonusSwordWaveDamage;

                int scratchDamage = Mathf.RoundToInt(totalSwordDamage / 2f);


                int scratchHeal = Mathf.RoundToInt(totalSwordDamage / 4f);

                scratchScript.Init(scratchDamage, scratchHeal, this);
            }
            TryTriggerVampireShield();
        }
    }

    private IEnumerator VampireBladeSpawnerRoutine()
    {
        while (true)
        {
            float actualInterval = GetReducedCooldown(vampireBladeInterval);
            yield return new WaitForSeconds(actualInterval);

            if (playerLevel != null && playerLevel.CurrentLevel < 7) continue;

            if (!IsDead() && vampireBladePrefab != null)
            {
                SpawnVampireBlade();
            }
        }
    }

    private void SpawnVampireBlade()
    {


        GameObject bladeObj = Instantiate(vampireBladePrefab, ShootPoint.position, Quaternion.identity);

        bladeObj.transform.SetParent(this.transform);


        VampireBlade bladeScript = bladeObj.GetComponent<VampireBlade>();
        if (bladeScript != null)
        {
            int bladeDamage = Mathf.RoundToInt((SwordWaveDamage + BonusSwordWaveDamage) / 2f);

            bladeScript.InitWeapon(bladeDamage, this);
        }

        TryTriggerVampireShield();
    }
    private IEnumerator BatSpawnerRoutine()
    {
        while (true)
        {
            float actualInterval = GetReducedCooldown(batSpawnInterval);
            yield return new WaitForSeconds(actualInterval);

            if (playerLevel != null && playerLevel.CurrentLevel < 3) continue;

            if (!IsDead() && batPrefab != null)
            {
                SpawnBat();
            }
        }
    }

    private void SpawnBat()
    {

        GameObject batObj = Instantiate(batPrefab, transform.position, Quaternion.identity);


        Bat batScript = batObj.GetComponent<Bat>();
        if (batScript != null)
        {

            int batDamage = Mathf.RoundToInt((SwordWaveDamage + BonusSwordWaveDamage) / 2f);


            batScript.InitWeapon(batDamage, this);
        }

        TryTriggerVampireShield();
    }
    public void OnHitWith(Enemy enemy)
    {
        if (enemy == null) return;
        TakeDamage(enemy.DamageHit);
    }

    public override void Initialized(int starterHealth)
    {

        base.Initialized(starterHealth);
        AddMaxHealth(BonusMaxHealth);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            OnHitWith(enemy);

            if (AudioSource != null && HitSound != null)
                AudioSource.PlayOneShot(HitSound);

            Vector2 knockbackDirection = (transform.position - other.transform.position).normalized;
            if (rb != null)
                rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
         
            OnHitWith(enemy);
        }
    }

    void Update()
    {
        Ability1Input();
        Ability2Input();
        Ability3Input();

        AbilityCooldownTick(ref currentAbility1Cooldown, Ability1Cooldown, ref isAbility1Cooldown, AbilityImage1, AbilityText1, ref effectiveAbility1Max);
        AbilityCooldownTick(ref currentAbility2Cooldown, Ability2Cooldown, ref isAbility2Cooldown, AbilityImage2, AbilityText2, ref effectiveAbility2Max);
        AbilityCooldownTick(ref currentAbility3Cooldown, Ability3Cooldown, ref isAbility3Cooldown, AbilityImage3, AbilityText3, ref effectiveAbility3Max);
    }

    public void Ability1Input()
    {
        if (Input.GetKeyDown(Ability1Key) && !isAbility1Cooldown)
        {

            float cdr = Mathf.Clamp(BonusCooldownReduction / 100f, 0f, 0.99f);
            effectiveAbility1Max = Ability1Cooldown * (1f - cdr);

            if (effectiveAbility1Max <= 0f)
            {

                Ability1Teleport();
                return;
            }

            isAbility1Cooldown = true;
            currentAbility1Cooldown = effectiveAbility1Max;
            if (AbilityImage1 != null) AbilityImage1.fillAmount = 1f;

            Ability1Teleport();
        }
    }

    public void Ability1Teleport()
    {
        if (animator != null)
            animator.SetTrigger("Teleport");

        if (TeleportSound != null && AudioSource != null)
        {
            AudioSource.PlayOneShot(TeleportSound);
        }

        if (teleportVFX != null)
        {

            Instantiate(teleportVFX, transform.position, Quaternion.identity);
        }

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = PlayerCamera != null ? Mathf.Abs(PlayerCamera.transform.position.z) : Camera.main != null ? Mathf.Abs(Camera.main.transform.position.z) : 10f;
        Vector3 worldPos = (PlayerCamera != null ? PlayerCamera : Camera.main).ScreenToWorldPoint(mouseScreenPos);

        worldPos.x = Mathf.Clamp(worldPos.x, minX, maxX);
        worldPos.y = Mathf.Clamp(worldPos.y, minY, maxY);

        teleportTarget = new Vector2(worldPos.x, worldPos.y);


        if (rb != null)
            rb.MovePosition(teleportTarget);
        else
            transform.position = teleportTarget;


        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    public void Ability2Input()
    {
        if (Input.GetKeyDown(Ability2Key) && !isAbility2Cooldown)
        {
            float cdr = Mathf.Clamp(BonusCooldownReduction / 100f, 0f, 0.99f);
            effectiveAbility2Max = Ability2Cooldown * (1f - cdr);

            if (effectiveAbility2Max <= 0f)
            {
                Shoot();
                return;
            }

            isAbility2Cooldown = true;
            currentAbility2Cooldown = effectiveAbility2Max;
            if (AbilityImage2 != null) AbilityImage2.fillAmount = 1f;

            if (SwordWaveSound != null && AudioSource != null)
            {
                AudioSource.PlayOneShot(SwordWaveSound);
            }

            Shoot();
        }
    }

    public void Shoot()
    {
        if (Bullet == null || ShootPoint == null) return;

        GameObject newWaveObject = Instantiate(Bullet, ShootPoint.position, ShootPoint.rotation);
        Weapon weaponScript = newWaveObject.GetComponent<Weapon>();

        if (weaponScript != null)
        {
            int totalSwordWaveDamage = SwordWaveDamage + BonusSwordWaveDamage;
            weaponScript.InitWeapon(totalSwordWaveDamage, this);
        }
    }

    public void Ability3Input()
    {
        if (Input.GetKeyDown(Ability3Key) && !isAbility3Cooldown)
        {
            float cdr = Mathf.Clamp(BonusCooldownReduction / 100f, 0f, 0.99f);
            effectiveAbility3Max = Ability3Cooldown * (1f - cdr);

            int totalHeal = BasePotionHeal + BonusPotionHeal;
            Ability3Heal(totalHeal);

            if (effectiveAbility3Max <= 0f)
            {

                return;
            }

            isAbility3Cooldown = true;
            currentAbility3Cooldown = effectiveAbility3Max;
            if (AbilityImage3 != null) AbilityImage3.fillAmount = 1f;

            if (AudioSource != null && HealSound != null)
                AudioSource.PlayOneShot(HealSound);
        }
    }

    public void Ability3Heal(int healAmount)
    {
        if (healAmount <= 0 || IsDead()) return;

        if (Health >= MaxHealth)
        {
            if (currentPathType == PathType.Vampire)
            {
                int shieldLimit = Mathf.RoundToInt(MaxHealth * 0.7f);
             
                CurrentShield += healAmount;

                if (CurrentShield > shieldLimit)
                    CurrentShield = shieldLimit;

                Debug.Log($"Overheal converted to Shield: {CurrentShield} / {shieldLimit}");
            }
        }
        else
        {
            Health += healAmount;
            if (Health > MaxHealth) Health = MaxHealth;

            if (FloatingTextManager.Instance != null)
                FloatingTextManager.Instance.ShowHeal(healAmount, transform.position);
        }
    }


    private void AbilityCooldownTick(ref float currentCooldown, float baseMaxCooldown, ref bool isCooldown, Image skillImage, Text skillText, ref float effectiveMax)
    {
        if (!isCooldown) return;


        if (effectiveMax <= 0f)
        {
            float cdr = Mathf.Clamp(BonusCooldownReduction / 100f, 0f, 0.99f);
            effectiveMax = baseMaxCooldown * (1f - cdr);
            if (effectiveMax <= 0f)
            {

                isCooldown = false;
                currentCooldown = 0f;
                if (skillImage != null) skillImage.fillAmount = 0f;
                if (skillText != null) skillText.text = "";
                return;
            }
        }

        currentCooldown -= Time.deltaTime;

        if (currentCooldown <= 0f)
        {
            currentCooldown = 0f;
            isCooldown = false;

            if (skillImage != null) skillImage.fillAmount = 0f;
            if (skillText != null) skillText.text = "";
            return;
        }

        if (skillImage != null)
            skillImage.fillAmount = currentCooldown / effectiveMax;

        if (skillText != null)
            skillText.text = Mathf.Ceil(currentCooldown).ToString();
    }

    protected override void Die()
    {
        if (animator != null)
            animator.SetTrigger("Dead");

        PlayerController prince = GetComponent<PlayerController>();
        if (prince != null)
        {
            prince.SetMovementLock(true);
        }

        if (GameManager != null)
            GameManager.Instance.ShowGameOverScreen();
    }

    public void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackPoint.position, AttackRadius, AttackLayer);

        foreach (Collider2D collInfo in hitEnemies)
        {
            Enemy enemy = collInfo.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                int totalDamage = BaseAttackDamage + BonusAttackDamage;
                enemy.TakeDamage(totalDamage);

             
                if (currentPathType == PathType.Vampire)
                {
                    BleedEffect existingBleed = enemy.GetComponent<BleedEffect>();
                   
                    if (existingBleed == null)
                    {
                        BleedEffect bleed = enemy.gameObject.AddComponent<BleedEffect>();
                      
                        bleed.Initialize(enemy, totalDamage, normalAttackBleedVFX, normalAttackBleedSound);
                    }
                    else
                    {
         
                        existingBleed.Initialize(enemy, totalDamage, normalAttackBleedVFX, normalAttackBleedSound);
                    }
                }
              
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (AttackPoint == null) return;
        Gizmos.DrawWireSphere(AttackPoint.position, AttackRadius);
    }

    public override void TakeDamage(int damageAmount)
    {
        if (isInvincible) return;

        int remainingDamage = damageAmount;

        // ใช้ CurrentShield แทน currentBloodShield
        if (CurrentShield > 0)
        {
            if (CurrentShield >= remainingDamage)
            {
                CurrentShield -= remainingDamage;
                remainingDamage = 0;
            }
            else
            {
                remainingDamage -= CurrentShield;
                CurrentShield = 0;
            }
            Debug.Log("Blood Shield Absorbed Damage!");
        }


        if (remainingDamage > 0)
        {
            base.TakeDamage(remainingDamage);
            
        }
    


    bool isOvertime = (timer != null && timer.IsOvertime);
        bool isFlooding = (enemySpawner != null && enemySpawner.IsFlooding);
        bool isDangerMode = isOvertime || isFlooding;

        
        float currentIframeDuration = isDangerMode ? 0.5f : invincibilityDuration;

        if (invincibilityCoroutine != null)
            StopCoroutine(invincibilityCoroutine);

        invincibilityCoroutine = StartCoroutine(InvincibilityRoutine(currentIframeDuration));
    }


    protected override void TriggerHitVisuals()
    {

    }

    private IEnumerator InvincibilityRoutine(float duration)
    {
        isInvincible = true;

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        float timer = 0f;
        bool flashOn = true;

       
        while (timer < duration)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = flashOn ? Color.red : Color.white;
                flashOn = !flashOn;
            }

            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        isInvincible = false;
        invincibilityCoroutine = null;
    }

    public int CalculateVampireDamage(Enemy enemy, int baseDamage)
    {
       
        if (currentPathType != PathType.Vampire) return baseDamage;

        
        BleedEffect bleed = enemy.GetComponent<BleedEffect>();

        if (bleed != null)
        {
          
            float bonus = baseDamage * 1.5f;

          

            return Mathf.RoundToInt(bonus);
        }

        return baseDamage;
    }
    public void ApplyUpgrade(UpgradeData data, float value)
    {
        switch (data.type)
        {
            case UpgradeType.AttackDamage:
                
                BonusAttackDamage += Mathf.RoundToInt(value);
                Debug.Log($"Upgraded Damage: +{value}");
                break;

            case UpgradeType.MaxHealth:
                int hpAdd = Mathf.RoundToInt(value);
                BonusMaxHealth += hpAdd;
                AddMaxHealth(hpAdd);
                Debug.Log($"Upgraded Max HP: +{value}");
                break;

            case UpgradeType.CooldownReduction:
             
                BonusCooldownReduction += value;
                Debug.Log($"Upgraded CDR: +{value}%");
                break;

            case UpgradeType.PotionHeal:
                BonusPotionHeal += Mathf.RoundToInt(value);
                Debug.Log($"Upgraded Potion Heal: +{value}");
                break;

            case UpgradeType.SwordWaveDamage:
                BonusSwordWaveDamage += Mathf.RoundToInt(value);
                Debug.Log($"Upgraded Sword Wave Dmg: +{value}");
                break;

            case UpgradeType.AttackSpeed:
                BonusAttackSpeed += value;
                Debug.Log($"Upgraded Attack Speed: +{value * 100}%");
                break;
                
            case UpgradeType.XPMultiplier:
               
                if (playerLevel != null)
                {
                    playerLevel.BonusXPMultiplier += value;
                    Debug.Log($"Upgraded XP Gain: +{value}%");
                }
                break;
        }
    }


    private float GetReducedCooldown(float baseCooldown)
    {
        float reduction = Mathf.Clamp(BonusCooldownReduction, 0f, 90f);
        return baseCooldown * (1f - (reduction / 100f));
    }
}
