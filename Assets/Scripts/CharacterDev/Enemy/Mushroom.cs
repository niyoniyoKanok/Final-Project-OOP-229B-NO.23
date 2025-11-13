using UnityEngine;

public class Mushroom : Enemy
{
    protected override void Start()
    {
        base.Start();
        base.Initialized(50);
        DamageHit = 20;
        moveSpeed = 3f;
    }

    public override void Move()
    {
       

        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
    }

    protected override void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Dead");
        }
    }
}
