using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Character
{
    public int DamageHit { get; protected set; }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void Die()
    {
        Destroy(this.gameObject);
    }
}
