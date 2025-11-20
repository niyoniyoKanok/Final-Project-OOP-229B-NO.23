using UnityEngine;

public class Skeleton : Enemy
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        base.Initialized(70);
        DamageHit = 15;
        moveSpeed = 1.5f;
        canFly = false;
    }

    public override void Move()
    {
        PhysicsMove(canFly);
    }
}
