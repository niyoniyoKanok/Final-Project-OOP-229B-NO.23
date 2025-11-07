using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    private float jumpHeight = 15f;

    private bool isMovementLocked = false;

    private float movement;
    private float moveSpeed = 5f;
    bool facingRight = true;

    public bool isGround = true;
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



    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
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

        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            jumpPressed = true;
            animator.SetBool("Jump", true);
            audioSource.PlayOneShot(JumpSound);
        }

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("Execution");
    

        }

    }

    private void FixedUpdate()
    {
        if (isMovementLocked)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        bool isTouchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);


        if (isTouchingGround)
        {
            isGround = true;
            animator.SetBool("Jump", false);
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
            isGround = false;
        }

     
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
            isGround = true;
            animator.SetBool("Jump", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGround = false;
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