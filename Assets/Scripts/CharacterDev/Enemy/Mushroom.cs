using UnityEngine;

public class Mushroom : Enemy
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        base.Initialized(startingHealth);
        DamageHit = 10;
        moveSpeed = 3f;
        canFly = false;
    }

    public override void Move()
    {
        PhysicsMove(canFly);
    }
}
