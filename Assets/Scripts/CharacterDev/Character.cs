using UnityEngine;
using System.Collections;

public abstract class Character : MonoBehaviour
{
    private int health;
    public int BaseMaxHealth { get; set; }
    public int MaxHealth { get; set; }

    protected Animator animator;
    protected Rigidbody2D rb;
    [SerializeField] private float healthBarVisibleTime = 3f;
    private Coroutine hideHealthBarCoroutine;

    protected SpriteRenderer spriteRenderer;

    // 1. ✅ เพิ่มตัวแปรเก็บสีเริ่มต้น
    protected Color defaultColor = Color.white;

    [Header("Hit FX")]
    [SerializeField] private float flashDuration = 0.1f;

    [SerializeField] private HealthBar healthBar;

    public int Health
    {
        get { return health; }
        set
        {
            health = Mathf.Clamp(value, 0, MaxHealth);
            if (healthBar != null)
            {
                healthBar.UpdateBar(health, MaxHealth);
            }
        }
    }

    public virtual void Initialized(int starterHealth)
    {
        BaseMaxHealth = starterHealth;
        MaxHealth = starterHealth;
        Health = starterHealth;

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        
        if (spriteRenderer != null)
        {
            defaultColor = spriteRenderer.color;
        }
    }


    public void SetCharacterColor(Color newColor)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = newColor;
            defaultColor = newColor; 
        }
    }

    public virtual void AddMaxHealth(int amount)
    {
        if (amount <= 0) return;

        MaxHealth += amount;
        Health = Mathf.Min(Health, MaxHealth);

        ShowHealthBarThenHide();
    }

    public virtual void TakeDamage(int damageAmount)
    {
        if (health <= 0) return;

        Health -= damageAmount;

        if (health <= 0)
        {
            Die();
        }
        else
        {
            if (animator != null)
            {
                animator.SetTrigger("TakeHit"); 
            }

            ShowHealthBarThenHide();
            TriggerHitVisuals();
        }
    }

    protected virtual void TriggerHitVisuals()
    {
        if (this.gameObject.activeInHierarchy)
        {
            StartCoroutine(HitFlashRoutine());
        }
    }

    private IEnumerator HitFlashRoutine()
    {
     
        if (spriteRenderer != null)
            spriteRenderer.color = Color.red; 

        yield return new WaitForSeconds(flashDuration);

        if (spriteRenderer != null)
            spriteRenderer.color = defaultColor;
    }

    protected void ShowHealthBarThenHide()
    {
        if (healthBar == null) return;

        if (hideHealthBarCoroutine != null)
        {
            StopCoroutine(hideHealthBarCoroutine);
            hideHealthBarCoroutine = null;
        }

        healthBar.gameObject.SetActive(true);
        hideHealthBarCoroutine = StartCoroutine(HideHealthBarRoutine());
    }

    protected IEnumerator HideHealthBarRoutine()
    {
        yield return new WaitForSeconds(healthBarVisibleTime);
        if (healthBar != null)
            healthBar.gameObject.SetActive(false);
        hideHealthBarCoroutine = null;
    }

    protected abstract void Die();

    public bool IsDead()
    {
        return health <= 0;
    }
}