using UnityEngine;

public class ArcaneComet : MonoBehaviour
{
    [Header("Comet Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float aoeRadius = 3.5f;


    [Header("Star Falling Spawner")]
    [Tooltip("ลาก Prefab ของ StarFalling มาใส่ที่นี่")]
    [SerializeField] private GameObject starFallingPrefab;
    [Tooltip("จำนวนดาวที่จะเสกออกมาหลังระเบิด")]
    [SerializeField] private int numStarsToSpawn = 3;
    [Tooltip("ความสูงที่จะเสกดาวขึ้นไปเหนือจุดระเบิด")]
    [SerializeField] private float starSpawnHeightOffset = 5f;
    [Tooltip("ดาเมจของดาวลูกใหม่ คิดเป็นกี่เปอร์เซ็นต์ของ Comet (0.5 = 50%)")]
    [Range(0f, 1f)][SerializeField] private float starDamageMultiplier = 0.5f;


    private int damage;
    private bool hasExploded = false;

    public void Init(int dmg)
    {
        this.damage = dmg;
        Destroy(gameObject, 4f);
    }

    void Update()
    {
        if (!hasExploded)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasExploded) return;

        
        if (other.GetComponentInParent<Enemy>() != null || other.CompareTag("Ground"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        hasExploded = true;

        
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aoeRadius);

        foreach (var hit in hits)
        {
            Enemy hitEnemy = hit.GetComponentInParent<Enemy>();
            if (hitEnemy != null && !hitEnemy.IsDead())
            {
                hitEnemy.TakeDamage(damage);
            }
        }

        if (starFallingPrefab != null && numStarsToSpawn > 0)
        {
            SpawnFallingStars();
        }
        
        Destroy(gameObject);
    }

   
    private void SpawnFallingStars()
    {
        
        int subDamage = Mathf.RoundToInt(damage * starDamageMultiplier);
        if (subDamage < 1) subDamage = 1; 

        for (int i = 0; i < numStarsToSpawn; i++)
        {
           
            float randomXOffset = Random.Range(-aoeRadius, aoeRadius);
            Vector3 groundTargetPos = transform.position + new Vector3(randomXOffset, 0, 0);

            Vector3 spawnPos = groundTargetPos + new Vector3(0, starSpawnHeightOffset, 0);

          
            GameObject starObj = Instantiate(starFallingPrefab, spawnPos, Quaternion.identity);

           
            StarFalling starScript = starObj.GetComponent<StarFalling>();
            if (starScript != null)
            {
                starScript.Init(subDamage);
            }
            else
            {
                Debug.LogWarning("ArcaneComet: ไม่พบสคริปต์ StarFalling บน Prefab ที่กำหนด!");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}