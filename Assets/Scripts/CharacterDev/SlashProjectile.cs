using UnityEngine;

public class SlashProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;
    public int damage = 10;
    public float lifeTime = 0.5f; 

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
        Destroy(gameObject, lifeTime);
    }

    public void Setup(Vector2 direction)
    {
       
        rb.linearVelocity = direction.normalized * speed;


        if (direction.x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
      
        Enemy enemy = hitInfo.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage); 

           
        }

      
        if (hitInfo.CompareTag("Ground") || hitInfo.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}