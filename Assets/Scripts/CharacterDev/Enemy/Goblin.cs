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

        Vector2 targetPosition = new Vector2(playerTransform.position.x, transform.position.y);

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

}
