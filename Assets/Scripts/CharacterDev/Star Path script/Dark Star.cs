using UnityEngine;

public class DarkStar : Weapon
{
    [Header("Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float rotateSpeed = 300f;
    [SerializeField] private float searchRadius = 20f; 
    [SerializeField] private float lifeTime = 5f; 

    private Transform target;


    private float damageInterval = 0.5f;
    private System.Collections.Generic.List<GameObject> hitTargets = new System.Collections.Generic.List<GameObject>();

    void Start()
    {
        Destroy(gameObject, lifeTime); 
        StartCoroutine(ResetHitList());
    }

    private void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
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
        if (character is Enemy)
        {
           
            if (!hitTargets.Contains(character.gameObject))
            {
                character.TakeDamage(this.damage);
                hitTargets.Add(character.gameObject); 

                
            }

      
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Character character = other.GetComponent<Character>();
        if (character != null && character is Enemy)
        {
            OnHitWith(character);
        }
    }

   
    private System.Collections.IEnumerator ResetHitList()
    {
        while (true)
        {
            yield return new WaitForSeconds(damageInterval);
            hitTargets.Clear();
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