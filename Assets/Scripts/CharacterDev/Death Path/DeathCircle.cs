using UnityEngine;

public class DeathCircle : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private GameObject deathVFX; 
    [SerializeField] private GameObject explosionVFX; 
    [SerializeField] private AudioClip explosionSound;

    private int damage;
    private int explosionDamage;

    public void Init(Enemy target, int dmg, int explodeDmg)
    {
        this.damage = dmg;
        this.explosionDamage = explodeDmg;

   
        target.TakeDamage(damage);

       
        Death status = target.GetComponent<Death>();
        if (status == null)
        {
            status = target.gameObject.AddComponent<Death>();
          
            status.Initialize(target, 3f, 0.5f, explosionDamage, explosionVFX, explosionSound);

          
            if (deathVFX != null)
                Instantiate(deathVFX, target.transform.position, Quaternion.identity);
        }
        else
        {

        }

     
        Destroy(gameObject, 1f);
    }
}