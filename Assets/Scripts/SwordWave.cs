using UnityEngine;

public class SwordWave : Weapon
{
    [SerializeField] private float speed;

    public override void Move()
    {
        float newX = transform.position.x + speed * Time.fixedDeltaTime;
        float newY = transform.position.y;
        Vector2 newPosition = new Vector2(newX, newY);
        transform.position = newPosition;
    }


    public override void OnHitWith(Character character)
    {
        if (character is Enemy)
            character.TakeDamage(this.damage);
    }

    void Start()
    {

        int direction = GetShootDirection();
        speed = 6.0f * direction;
        damage = 30;

        if (direction < 0)
        {
           
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }
    private void FixedUpdate()
    {
        Move();
    }
}
