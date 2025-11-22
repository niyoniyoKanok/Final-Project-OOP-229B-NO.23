using UnityEngine;

public class Bat : Weapon
{
    [Header("Bat Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotateSpeed = 200f;
    [SerializeField] private float searchRadius = 10f;

    private Transform target; 

    void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        if (target == null)
        {
            FindClosestEnemy();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

 
    public override void Move()
    {
        if (target != null)
        {
         
            Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
            direction.Normalize();

            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            GetComponent<Rigidbody2D>().angularVelocity = -rotateAmount * rotateSpeed;

            GetComponent<Rigidbody2D>().linearVelocity = transform.up * speed;
        }
        else
        {
            GetComponent<Rigidbody2D>().linearVelocity = transform.up * speed;
            GetComponent<Rigidbody2D>().angularVelocity = 0f;
        }
    }


    public override void OnHitWith(Character character)
    {
        if (character is Enemy enemy)
        {
            int finalDamage = this.damage;

            if (Shooter is Prince prince)
            {
                finalDamage = prince.CalculateVampireDamage(enemy, this.damage);

     
                prince.Ability3Heal(finalDamage);
            }
       

            enemy.TakeDamage(finalDamage);

            Destroy(this.gameObject);
        }
    }


    private void FindClosestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider2D enemyCollider in enemies)
        {

            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead())
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }
        }
        target = closestEnemy;
    }
}