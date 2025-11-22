using UnityEngine;

public class MiniBoss : Enemy
{



    protected override void Awake()
    {
        base.Awake();
        if (rb != null) rb.mass = 5;
    }

    protected override void Start()
    {
        base.Start();
       
        transform.localScale = Vector3.one * 1.2f;
    }

    public override void Move()
    {
        
        PhysicsMove(false);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        DamageHit = 30;
        moveSpeed = 2.0f; 
        base.Initialized(startingHealth);
        xpDrop = 500;
        transform.localScale = Vector3.one * 2f;
    }
}