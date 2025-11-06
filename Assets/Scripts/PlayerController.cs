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

    private bool teleportPressed;
    private Vector2 teleportTarget;
    public float teleportCooldown = 3f;
    private bool canTeleport = true;

    public AudioSource audioSource;
    public AudioClip teleportSound;

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
        }

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("Execution");
            audioSource.PlayOneShot(teleportSound);
            SetMovementLock(true);
        }

        if (Input.GetMouseButtonDown(1) && canTeleport)
        {
            teleportPressed = true;
            canTeleport = false;

            animator.SetTrigger("Teleport");

            if (teleportSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(teleportSound);
            }

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            teleportTarget = new Vector2(worldPos.x, worldPos.y);

            StartCoroutine(TeleportCooldownRoutine());
        }
    }

    private void FixedUpdate()
    {
        if (isMovementLocked)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
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

        if (teleportPressed)
        {
            rb.MovePosition(teleportTarget);
            teleportPressed = false;
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

    private IEnumerator TeleportCooldownRoutine()
    {
        yield return new WaitForSeconds(teleportCooldown);
        canTeleport = true;
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
}