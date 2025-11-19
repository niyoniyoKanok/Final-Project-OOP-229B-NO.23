using UnityEngine;

public class VampireBlade : Weapon
{
    [Header("Settings")]
    [SerializeField] private float stabSpeed = 5f; 
    [SerializeField] private float maxDistance = 2f; 
    [SerializeField] private float lifeTime = 0.5f;

    [Header("Bleed Effects")] 
    [SerializeField] private GameObject bleedVFX;
    [SerializeField] private AudioClip bleedSound;

    private Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = transform.localPosition;
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        Move();
    }

    public override void Move()
    {
        float distance = Vector3.Distance(startLocalPos, transform.localPosition);
        if (distance < maxDistance)
        {
            transform.localPosition += Vector3.right * stabSpeed * Time.fixedDeltaTime;
        }
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