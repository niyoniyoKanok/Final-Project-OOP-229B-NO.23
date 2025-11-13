using UnityEngine;

public class FlyingEye : Enemy 
{
    [SerializeField] private float idealDistance = 6f; 

    protected override void Start()
    {
        base.Start();
        base.Initialized(20);
        moveSpeed = 2.5f; 



      
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0;
        }
    }

    public override void Move()
    {
        

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > idealDistance)
        {

            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
        }
        else if (distanceToPlayer < idealDistance - 0.5f)
        {

            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, -moveSpeed * Time.deltaTime);
        }
    }
           

}