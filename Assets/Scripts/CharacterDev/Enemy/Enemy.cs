using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public abstract class Enemy : Character
{
    protected Transform playerTransform;
    public float moveSpeed;
    public int DamageHit { get; protected set; }
    protected float deathDelayTime = 1.0f;

    protected bool isStunned = false;
    [SerializeField] protected float stunDuration = 0.5f;

    protected virtual void Start()
    {
        Prince player = FindFirstObjectByType<Prince>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }


    void Update()
    {
        if (IsDead() || playerTransform == null)
        {
            return;
        }

        if (isStunned) return;

        FlipTowardsPlayer();
        Move();
    }

    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);

        if (!IsDead())
        {
           
            StopCoroutine(StunRoutine());
            StartCoroutine(StunRoutine());
        }
    }

    protected IEnumerator StunRoutine()
    {
        isStunned = true; 

   
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(stunDuration); 

        isStunned = false;
    }

    public abstract void Move();

    protected override void Die()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

     
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

      
        if (animator != null)
        {
            animator.SetTrigger("Dead");
        }

      
        Destroy(this.gameObject, deathDelayTime);
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    private void FlipTowardsPlayer()
    {
        if (playerTransform.position.x > transform.position.x)
        {
          
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}
