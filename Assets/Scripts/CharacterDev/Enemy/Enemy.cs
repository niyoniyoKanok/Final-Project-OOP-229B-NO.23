using System.Collections;
using UnityEngine;

public abstract class Enemy : Character
{
    [Header("Sprite Settings")]
   
    [SerializeField] protected bool spriteFacesLeft = false;

    protected Transform playerTransform;
    public float moveSpeed = 2f;

    protected PlayerLevel playerLevel;

    [Header("Rewards")]
    public int xpDrop = 10;

    public AudioClip hitSound;
    protected AudioSource audioSourceRef;
    public int DamageHit { get; set; }
    protected float deathDelayTime = 1.0f;

    protected SpriteRenderer sr;
    protected Color originalColor = Color.white;

    protected bool isStunned = false;
    [SerializeField] protected float stunDuration = 0.5f;

    protected Vector3 originalScale;
    protected int originalMaxHealth;
    protected int originalDamage;
    protected int originalXP;

    // physics refs
    
    protected Collider2D colRef;

    // store original gravity/bodyType
    protected float originalGravityScale = 1f;
    protected RigidbodyType2D originalBodyType = RigidbodyType2D.Dynamic;

    // flying / grounded flag (child can override)
    protected bool canFly = false;

    protected virtual void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            originalColor = sr.color;
        }

        originalScale = transform.localScale;

        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            originalGravityScale = rb.gravityScale;
            originalBodyType = rb.bodyType;
        }

        colRef = GetComponent<Collider2D>();
    }

    protected virtual void Start()
    {
        // try to find player (your project used Prince)
        var playerObj = FindFirstObjectByType<Prince>();
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerLevel = playerObj.GetComponent<PlayerLevel>();
        }

        audioSourceRef = GetComponent<AudioSource>();
        if (audioSourceRef == null)
        {
            audioSourceRef = gameObject.AddComponent<AudioSource>();
        }
    }

    protected virtual void Update()
    {
        if (IsDead() || playerTransform == null) return;
        if (isStunned) return;

        FlipTowardsPlayer();
    }

    protected virtual void FixedUpdate()
    {
        if (IsDead() || playerTransform == null) return;
        if (isStunned) return;

        Move();
    }

    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);

        if (FloatingTextManager.Instance != null)
        {
            bool isCrit = damageAmount > 1000;
            FloatingTextManager.Instance.ShowDamage(damageAmount, transform.position, isCrit);
        }

        if (hitSound != null && audioSourceRef != null)
        {
            audioSourceRef.PlayOneShot(hitSound);
        }

        if (!IsDead())
        {
            StopCoroutine(StunRoutine());
            StartCoroutine(StunRoutine());
        }
    }

    public void SetEliteColor(Color color)
    {
        if (sr != null)
        {
            sr.color = color;
            originalColor = color;
        }
    }

    protected IEnumerator StunRoutine()
    {
        isStunned = true;

        if (rb != null) rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
    }

    // ----------------------
    // Reset state for pooling
    // ----------------------
    public virtual void ResetState()
    {
        // transform
        transform.localScale = originalScale;

        // renderer
        if (sr != null) sr.color = originalColor;

        // physics
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = originalBodyType;
            rb.gravityScale = originalGravityScale;
        }

        // collider
        if (colRef == null) colRef = GetComponent<Collider2D>();
        if (colRef != null) colRef.enabled = true;

        // animator
        if (animator != null)
        {
            animator.Rebind();
   
        }

        isStunned = false;

        xpDrop = 10;


        canFly = false;
    }

    protected void PhysicsMove(bool allowFly)
    {
        if (playerTransform == null) return;

        Vector2 direction = Vector2.zero;

        if (allowFly)
        {
            direction = (playerTransform.position - transform.position).normalized;
        }
        else
        {
            float dirX = playerTransform.position.x - transform.position.x;
            direction = new Vector2(dirX, 0f).normalized;
        }

        if (rb != null)
        {
            Vector2 newPos = rb.position + (direction * moveSpeed * Time.fixedDeltaTime);

            if (!allowFly)
            {
               
                newPos.y = rb.position.y;
            }

            rb.MovePosition(newPos);
        }
        else
        {
            Vector3 targetPos;
            if (!allowFly)
                targetPos = new Vector3(playerTransform.position.x, transform.position.y, transform.position.z);
            else
                targetPos = playerTransform.position;

            Vector3 newPos = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (!allowFly)
            {
                newPos = StickToGround(newPos);
            }

            transform.position = newPos;
        }
    }

   
    protected Vector3 StickToGround(Vector3 pos)
    {
       
        RaycastHit2D hit = Physics2D.Raycast(pos + Vector3.up * 0.5f, Vector2.down, 3f, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            pos.y = hit.point.y + (colRef != null ? colRef.bounds.extents.y : 0.5f);
        }
        return pos;
    }

    protected bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up * 0.1f, Vector2.down, 0.2f, LayerMask.GetMask("Ground"));
        return hit.collider != null;
    }

    public abstract void Move();

    protected override void Die()
    {
        if (playerLevel != null) playerLevel.AddXP(xpDrop);

        if (colRef != null) colRef.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (animator != null)
        {
            animator.SetTrigger("Dead");
        }

        StartCoroutine(ReturnToPoolRoutine());
    }

    private IEnumerator ReturnToPoolRoutine()
    {
        yield return new WaitForSeconds(deathDelayTime);

        if (EnemyPool.Instance != null)
            EnemyPool.Instance.ReturnToPool(this);
        else
            Destroy(gameObject);
    }

    public void DestroySelf()
    {
        gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
      
        transform.localScale = originalScale;
        if (sr != null) sr.color = originalColor;
        isStunned = false;

        if (colRef == null) colRef = GetComponent<Collider2D>();
        if (colRef != null) colRef.enabled = true;

        xpDrop = 10;
    }

    private void FlipTowardsPlayer()
    {
        if (playerTransform == null) return;


        bool playerIsOnRight = playerTransform.position.x > transform.position.x;

      
        float scaleX = Mathf.Abs(transform.localScale.x);

      
        if (playerIsOnRight)
        {
            
            scaleX = spriteFacesLeft ? -scaleX : scaleX;
        }
        else
        {
           
            scaleX = spriteFacesLeft ? scaleX : -scaleX;
        }

    
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
    }

    public void ApplyStun(float duration)
    {
       
        if (!isStunned)
        {
            stunDuration = duration;
            StartCoroutine(StunRoutine());
        }
    }

  
}
