using UnityEngine;
using System.Collections; 

abstract public class Character : MonoBehaviour
{
    private int health;
    private int maxHealth;

    protected Animator animator;
    [SerializeField] private float healthBarVisibleTime = 3f;
    private Coroutine hideHealthBarCoroutine;

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
    }

    public void TakeDamage(int damageAmount)
    {
        if (health <= 0) return;

        Health -= damageAmount;

    
        ShowHealthBarThenHide();

        if (health <= 0)
        {
            Die();
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