using UnityEngine;
using System.Collections;

public abstract class Character : MonoBehaviour
{
    private int health;
    private int currentShield;
    public int BaseMaxHealth { get; set; }
    public int MaxHealth { get; set; }

    protected Animator animator;
    protected Rigidbody2D rb;

    protected SpriteRenderer spriteRenderer;

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
            UpdateHealthBar();
        }
    }

    public int CurrentShield
    {
        get { return currentShield; }
        set
        {
            bool hadShield = currentShield > 0;
            currentShield = Mathf.Max(value, 0);

           
            UpdateHealthBar();
        }
    }

    protected void UpdateHealthBar()
    {
        if (healthBar != null)
        {

            healthBar.UpdateBar(health, MaxHealth, currentShield);
        }
    }

    public virtual void Initialized(int starterHealth)
    {
        BaseMaxHealth = starterHealth;
        MaxHealth = starterHealth;
        Health = starterHealth;
        currentShield = 0;

        if (healthBar != null)
        {
            healthBar.UpdateBar(health, MaxHealth, currentShield);
            healthBar.gameObject.SetActive(true);
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

       
    }

    public virtual void TakeDamage(int damageAmount)
    {
        if (health <= 0) return;

        int remainingDamage = damageAmount;

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
        }

        if (remainingDamage > 0)
        {
            Health -= remainingDamage;
        }

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
       
        Color currentEliteColor = defaultColor;

     
        if (spriteRenderer != null)
            spriteRenderer.color = Color.red; 

        yield return new WaitForSeconds(flashDuration);

       
        if (spriteRenderer != null)
            spriteRenderer.color = defaultColor;
    }


    protected abstract void Die();

    public bool IsDead()
    {
        return health <= 0;
    }
}