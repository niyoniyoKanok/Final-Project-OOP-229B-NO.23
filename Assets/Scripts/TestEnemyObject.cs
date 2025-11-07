using UnityEngine;

public class TestEnemyObject : Enemy
{
    void Awake()
    {
        base.Initialized(30);
        DamageHit = 15;
    }
}
