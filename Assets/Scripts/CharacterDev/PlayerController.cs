using UnityEngine;

using System.Collections;



public class PlayerController : MonoBehaviour

{

    public Animator animator;

    public Rigidbody2D rb;

    private float jumpHeight = 15f;

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

    public bool IsAttacking = false;





    void Start()

    {

        if (animator == null)

            animator = GetComponent<Animator>();

        if (rb == null)

            rb = GetComponent<Rigidbody2D>();

        if (audioSource == null)

            audioSource = GetComponent<AudioSource>();

        prince = GetComponent<Prince>();
        princeCharacter = GetComponent<Character>();

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



        if (Input.GetKeyDown(KeyCode.Space) && IsGround)

        {

            jumpPressed = true;



            audioSource.PlayOneShot(JumpSound);

        }



        if (Input.GetKeyDown(KeyCode.F))

        {

            animator.SetTrigger("Execution");

        }



        if (Input.GetMouseButtonDown(0))

        {
            float speedMultiplier = 1f + (prince.BonusAttackSpeed / 100f);
            animator.SetFloat("AttackSpeed", speedMultiplier);

            animator.SetTrigger("Attack");

        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }



    }

    private void FixedUpdate()

    {

        if (isDashing)
        {
            return;
        }

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

        rb.linearVelocity = new Vector2(movement * moveSpeed, rb.linearVelocity.y);

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

        if (collision.gameObject.tag == "Ground")

        {

            IsGround = false;

        }

    }



    public void SetMovementLock(bool isLocked)

    {

        isMovementLocked = isLocked;

    }



    public void PlayAttackFX()

    {



        if (attackFXPrefab != null && attackFXSpawnPoint != null)

        {



            Instantiate(attackFXPrefab, attackFXSpawnPoint.position, attackFXSpawnPoint.rotation);

        }

    }



    public void PlayAttackFX_1()

    {



        if (attackFXPrefab_1 != null && attackFXSpawnPoint_1 != null)

        {



            Instantiate(attackFXPrefab_1, attackFXSpawnPoint_1.position, attackFXSpawnPoint_1.rotation);

        }

    }

    public void PlayerExecuteFX()

    {



        if (attackFXPrefab_2 != null && attackFXSpawnPoint_2 != null)

        {



            Instantiate(attackFXPrefab_2, attackFXSpawnPoint_2.position, attackFXSpawnPoint_2.rotation);

        }



    }



}