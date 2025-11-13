using UnityEngine;
using System.Collections; 

abstract public class Character : MonoBehaviour
{
    private int health;
    private int maxHealth;

    protected Animator animator;
    protected Rigidbody2D rb;
    [SerializeField] private float healthBarVisibleTime = 3f;
    private Coroutine hideHealthBarCoroutine;


    protected SpriteRenderer spriteRenderer;
    [Header("Hit FX")]
    [SerializeField] private float flashDuration = 0.1f; 
    public int Health
    {
        get { return health; }
        protected set
        {
            health = Mathf.Clamp(value, 0, maxHealth);

            if (healthBar != null)
            {
                healthBar.UpdateBar(health, maxHealth);
            }
        }
    }

    [SerializeField] private HealthBar healthBar;

    public void Initialized(int starterHealth)
    {
        maxHealth = starterHealth;
        Health = starterHealth;

   
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void TakeDamage(int damageAmount)
    {
        if (health <= 0) return;

        Health -= damageAmount;

       
        if (this.gameObject.activeInHierarchy)
        {
            StartCoroutine(HitFlashRoutine());
        }
   
        if (animator != null)
        {
            animator.SetTrigger("TakeHit");
        }

        ShowHealthBarThenHide();

        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitFlashRoutine()
    {
      
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }

      
        yield return new WaitForSeconds(flashDuration);

       
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }
    protected void ShowHealthBarThenHide()
    {
        if (healthBar == null) return;


        if (hideHealthBarCoroutine != null)
        {
            StopCoroutine(hideHealthBarCoroutine);
        }

      
        healthBar.gameObject.SetActive(true);

       
        hideHealthBarCoroutine = StartCoroutine(HideHealthBarRoutine());
    }

   
    protected IEnumerator HideHealthBarRoutine()
    {
        yield return new WaitForSeconds(healthBarVisibleTime);
        healthBar.gameObject.SetActive(false);
        hideHealthBarCoroutine = null;
    }

    protected abstract void Die();

    public bool IsDead()
    {
        return health <= 0;
    }
}