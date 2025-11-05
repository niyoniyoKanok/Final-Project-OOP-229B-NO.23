using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    private float jumpHeight = 15f;

    private float movement;
    private float moveSpeed = 5f;
    bool facingRight = true;

    public bool isGround = true;
    private bool jumpPressed; 

    void Start()
    {
       
    }

    void Update()
    {

        movement = Input.GetAxis("Horizontal");


        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            jumpPressed = true; 
        }

        if (Mathf.Abs(movement) > .1f)
        {
            animator.SetFloat("Run", 1f);
        }
        else if (movement < .1f)
        {
            animator.SetFloat("Run", 0f);
        }
    }

    private void FixedUpdate()
    {

        transform.position += new Vector3(movement, 0f, 0f) * Time.fixedDeltaTime * moveSpeed;


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
        }
    }

    }