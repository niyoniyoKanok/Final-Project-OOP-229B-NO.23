using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;

    [Header("Jump & Float Settings")]
    public float jumpHeight = 15f;

    [Tooltip("ความเร็วในการตกเมื่อร่อน (ยิ่งน้อยยิ่งลอยนาน)")]
    public float floatingFallSpeed = 2f;
    private bool isFloatingInput = false;

    [Header("Attack Settings (Manual)")]
    public GameObject slashPrefab;        
    public Transform slashSpawnPoint;    
    [SerializeField] private float attackAnimLength = 0.4f; 

    private Prince prince;
    private Character princeCharacter;

    private bool isMovementLocked = false;

    private bool canDash = true;
    private bool isDashing;
    [SerializeField] private float dashingPower = 100f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCooldown = 1f;
    [SerializeField] private TrailRenderer trailRenderer;
    private float movement;
    private float moveSpeed = 5f;
    bool facingRight = true;

    public bool IsGround = true;
    private bool jumpPressed;

    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.1f;

    public AudioSource audioSource;
    public AudioClip JumpSound;

    public GameObject attackFXPrefab;
    public Transform attackFXSpawnPoint;
    public GameObject attackFXPrefab_1;
    public Transform attackFXSpawnPoint_1;
    public GameObject attackFXPrefab_2;
    public Transform attackFXSpawnPoint_2;

    public static PlayerController Instance;
    public bool IsAttacking = false; // ใช้เช็คเพื่อไม่ให้กดรัวเกิน Animation

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        prince = GetComponent<Prince>();
        princeCharacter = GetComponent<Character>();

        if (Instance == null) Instance = this;
    }

    void Update()
    {
        if (isMovementLocked)
        {
            movement = 0;
            animator.SetFloat("Run", 0);
            return;
        }

        movement = Input.GetAxis("Horizontal");
        animator.SetFloat("Run", Mathf.Abs(movement));

        // --- Jump ---
        if (Input.GetKeyDown(KeyCode.Space) && IsGround)
        {
            jumpPressed = true;
            audioSource.PlayOneShot(JumpSound);
        }

        // --- Floating ---
        if (Input.GetKey(KeyCode.Space) && !IsGround && rb.linearVelocity.y < 0)
        {
            isFloatingInput = true;
        }
        else
        {
            isFloatingInput = false;
        }

        // --- Execution ---
        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("Execution");
        }

        // --- Dash ---
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        // --- MANUAL ATTACK (M1) ---
        // กดคลิกซ้าย และ ต้องไม่กำลังโจมตีอยู่
        if (Input.GetMouseButtonDown(0) && !IsAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    // Coroutine จัดการจังหวะการโจมตีแบบ Manual
    IEnumerator AttackRoutine()
    {
        IsAttacking = true; // ล็อคสถานะ

        // 1. คำนวณความเร็วจาก Stats
        float speedMultiplier = GetSpeedMultiplier();
        animator.SetFloat("AttackSpeed", speedMultiplier);
        animator.SetTrigger("Attack");

        // 2. เสก Slash Prefab (เหมือนเดิม)
        if (slashPrefab != null && slashSpawnPoint != null)
        {
            GameObject slashObj = Instantiate(slashPrefab, slashSpawnPoint.position, Quaternion.identity);
            SlashProjectile slashScript = slashObj.GetComponent<SlashProjectile>();

            if (slashScript != null)
            {
                Vector2 direction = facingRight ? Vector2.right : Vector2.left;
                slashScript.Setup(direction);
            }
        }

        // 3. คำนวณเวลารอ (ยิ่ง Speed เยอะ ยิ่งรอน้อย = กดรัวได้ไวขึ้น)
        float waitTime = attackAnimLength / speedMultiplier;
        yield return new WaitForSeconds(waitTime);

        IsAttacking = false; // ปลดล็อค ให้กดใหม่ได้
    }

    float GetSpeedMultiplier()
    {
        if (prince != null)
        {
            return 1f + (prince.BonusAttackSpeed / 100f);
        }
        return 1f;
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        if (isMovementLocked)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        bool isTouchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isTouchingGround)
        {
            IsGround = true;
        }
        else
        {
            IsGround = false;
        }

        float xVelocity = movement * moveSpeed;
        float yVelocity = rb.linearVelocity.y;

        // Floating Physics
        if (isFloatingInput)
        {
            yVelocity = Mathf.Max(yVelocity, -floatingFallSpeed);
        }

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);

        if (movement > 0 && !facingRight)
            Flip();
        else if (movement < 0 && facingRight)
            Flip();

        if (jumpPressed)
        {
            Jump();
            jumpPressed = false;
            IsGround = false;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        if (trailRenderer != null) trailRenderer.enabled = true;
        yield return new WaitForSeconds(dashingTime);
        if (trailRenderer != null) trailRenderer.enabled = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            IsGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground") IsGround = false;
    }

    public void SetMovementLock(bool isLocked)
    {
        isMovementLocked = isLocked;
    }

    public void PlayAttackFX()
    {
        if (attackFXPrefab != null && attackFXSpawnPoint != null)
            Instantiate(attackFXPrefab, attackFXSpawnPoint.position, attackFXSpawnPoint.rotation);
    }

    public void PlayAttackFX_1()
    {
        if (attackFXPrefab_1 != null && attackFXSpawnPoint_1 != null)
            Instantiate(attackFXPrefab_1, attackFXSpawnPoint_1.position, attackFXSpawnPoint_1.rotation);
    }

    public void PlayerExecuteFX()
    {
        if (attackFXPrefab_2 != null && attackFXSpawnPoint_2 != null)
            Instantiate(attackFXPrefab_2, attackFXSpawnPoint_2);
    }
}