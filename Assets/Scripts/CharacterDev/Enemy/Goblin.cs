using UnityEngine;

public class Goblin : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        base.Initialized(40);
        moveSpeed = 3f;
        DamageHit = 10;
    }

    public override void Move()
    {
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) return;

        
        float direction = Mathf.Sign(playerTransform.position.x - transform.position.x);

   
        if (Mathf.Abs(transform.position.x - playerTransform.position.x) > 0.5f)
        {
          
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        }
        else
        {
          
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

     
        }
    }

}
