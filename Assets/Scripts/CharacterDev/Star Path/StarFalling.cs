using UnityEngine;

public class StarFalling : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private GameObject hitVFX; 
    private int damage;

    public void Init(int dmg)
    {
        this.damage = dmg;
        Destroy(gameObject, 2f);
    }

    void Update()
    {
       
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemy.IsDead())
        {
            enemy.TakeDamage(damage);

            if (hitVFX != null) Instantiate(hitVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}