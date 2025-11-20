using UnityEngine;

public class Mushroom : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        base.Initialized(50);
        DamageHit = 10;
        moveSpeed = 3f;
    }

    protected override void Start()
    {
        base.Start();
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
