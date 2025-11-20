using UnityEngine;

public class VampireBlade : Weapon
{
    [Header("Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifeTime = 3f;
    private int direction = 1;

    [Header("Bleed Effects")] 
    [SerializeField] private GameObject bleedVFX;
    [SerializeField] private AudioClip bleedSound;

    private Vector3 startLocalPos;

    void Start()
    {
        direction = GetShootDirection();


        if (direction < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * -1;
            transform.localScale = scale;
        }

        
        Destroy(gameObject, lifeTime);
    }


    private void FixedUpdate()
    {
        Move();
    }

    public override void Move()
    {
        
        transform.Translate(Vector3.right * speed * direction * Time.deltaTime);
    }

    public override void OnHitWith(Character character)
    {
        if (character is Enemy enemy)
        {
            BleedEffect existingBleed = enemy.GetComponent<BleedEffect>();

            if (existingBleed == null)
            {
                BleedEffect bleed = enemy.gameObject.AddComponent<BleedEffect>();
           
                bleed.Initialize(enemy, this.damage, bleedVFX, bleedSound);
            }
            else
            {
             
                existingBleed.Initialize(enemy, this.damage, bleedVFX, bleedSound);
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
}