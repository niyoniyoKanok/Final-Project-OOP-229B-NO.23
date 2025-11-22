using UnityEngine;

public class Goblin : Enemy
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        base.Initialized(startingHealth); 
        moveSpeed = 3f;
        DamageHit = 10;
        canFly = false;
    }

    public override void Move()
    {
        PhysicsMove(canFly);
    }
}
