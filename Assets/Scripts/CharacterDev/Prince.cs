using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Prince : Character, IShootable
{
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
    [SerializeField] private float minX = -25.23f;
    [SerializeField] private float maxX = 50.27f;
    [SerializeField] private float minY = -1.56f;
    [SerializeField] private float maxY = 15f;

    [Header("Game Dependencies")]
    public Camera PlayerCamera;
    public GameManager GameManager;

    [Header("Ability 1")]
    public Image AbilityImage1;
    public Text AbilityText1;
    public KeyCode Ability1Key;
    public float Ability1Cooldown = 7f; 
    private Vector2 teleportTarget;

    [Header("Ability 2")]
    public Image AbilityImage2;
    public Text AbilityText2;
    public KeyCode Ability2Key;
    public float Ability2Cooldown = 5f;
    [field: SerializeField] public GameObject Bullet { get; set; }

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

    [Header("AttackPanel")]
    public Transform AttackPoint;
    public float AttackRadius = 1f;
    public LayerMask AttackLayer;

    void Start()
    {
        base.Initialized(100);

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

    public void OnHitWith(Enemy enemy)
    {
        if (enemy == null) return;
        Debug.Log("Prince hit by Enemy. Enemy's DamageHit is: " + enemy.DamageHit);
        TakeDamage(enemy.DamageHit);
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

        Health += healAmount;
        ShowHealthBarThenHide();
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

        // call base to handle HP reduction, animator, healthbar, etc.
        base.TakeDamage(damageAmount);

        // start invincibility (single coroutine)
        if (!isInvincible)
        {
            if (invincibilityCoroutine != null)
                StopCoroutine(invincibilityCoroutine);

            invincibilityCoroutine = StartCoroutine(InvincibilityRoutine());
        }
    }

   
    protected override void TriggerHitVisuals()
    {
        
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        float timer = 0f;
        bool flashOn = true;

        while (timer < invincibilityDuration)
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
}
