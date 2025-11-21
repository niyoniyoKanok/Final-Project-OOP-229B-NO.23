using UnityEngine;

public class GolemProjectile : Weapon 
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float lifeTime = 4f;

    private Vector2 moveDirection;
    private bool isInitialized = false;

   
    public void Init(Vector2 dir, int dmg)
    {
     
        this.damage = dmg;

        moveDirection = dir.normalized;
        moveDirection.y = 0; 


        if (moveDirection.x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * -1;
            transform.localScale = scale;
        }

        isInitialized = true;

     
        Destroy(gameObject, lifeTime);
    }

    public override void Move()
    {
        if (isInitialized)
        {
            transform.Translate(moveDirection * speed * Time.deltaTime);
        }
    }

   
    public override void OnHitWith(Character character)
    {
     

        if (character is Prince)
        {
            character.TakeDamage(this.damage);
            Destroy(gameObject);
        }

    
    }

    private void Update()
    {
        Move();
    }
}