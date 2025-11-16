using UnityEngine;

public class FlyingEye_Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 4f; 

    void Start()
    {

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
       
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        Character target = other.GetComponentInParent<Character>();


        if (target != null && target is Prince)
        {
            target.TakeDamage(damage); 
            Destroy(gameObject); 
        }

       
        if (other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
