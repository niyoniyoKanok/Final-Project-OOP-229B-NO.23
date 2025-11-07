using UnityEngine;

public class TestEnemyObject : Enemy
{
    void Awake()
    {
        base.Initialized(100);
        DamageHit = 15;
    }
    protected override void Die()
    {
        Destroy(this.gameObject);
        Debug.Log(this.transform.name + " Died.");
    }


}
