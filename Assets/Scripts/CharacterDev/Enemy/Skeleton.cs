using UnityEngine;

public class Skeleton : Enemy
{
    protected override void Start()
    {
        base.Start();
        base.Initialized(70);
        DamageHit = 25;
        moveSpeed = 1.5f;
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
