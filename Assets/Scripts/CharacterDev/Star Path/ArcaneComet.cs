using UnityEngine;

public class ArcaneComet : MonoBehaviour
{
    [SerializeField] private float speed = 8f; // ความเร็วตก
    [SerializeField] private float aoeRadius = 3.5f; // รัศมีระเบิด (AOE)

    private int damage;
    private bool hasExploded = false;

    public void Init(int dmg)
    {
        this.damage = dmg;
        // กันค้าง: ถ้าตกไม่ถึงพื้นภายใน 4 วิ ให้ทำลายตัวเอง
        Destroy(gameObject, 4f);
    }

    void Update()
    {
        if (!hasExploded)
        {
            // พุ่งลงข้างล่างเรื่อยๆ
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasExploded) return;

        // 1. เช็คว่าชนศัตรู (ต้องหาตัวแม่) หรือไม่
        Enemy enemy = other.GetComponentInParent<Enemy>();

        // 2. เช็คว่าชนพื้นหรือไม่
        bool isGround = other.gameObject.CompareTag("Ground");

        // ถ้าชนอย่างใดอย่างหนึ่ง -> ระเบิดตูม! (ทำดาเมจ)
        if (enemy != null || isGround)
        {
            Explode();
        }
    }

    private void Explode()
    {
        hasExploded = true;

        // --- ทำดาเมจวงกว้าง (AOE) ---
        // หาศัตรูทั้งหมดในรัศมี
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aoeRadius);

        foreach (var hit in hits)
        {
            // ดึงสคริปต์ Enemy (ใช้ GetComponentInParent เพื่อความชัวร์)
            Enemy hitEnemy = hit.GetComponentInParent<Enemy>();

            if (hitEnemy != null && !hitEnemy.IsDead())
            {
                hitEnemy.TakeDamage(damage);
            }
        }

        // (ตัดส่วนเล่น VFX ออกแล้ว)

        // ทำลายลูกอุกกาบาตทิ้งทันที
        Destroy(gameObject);
    }

    // วาดวงกลมให้เห็นใน Scene (เพื่อให้กะระยะง่าย)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}