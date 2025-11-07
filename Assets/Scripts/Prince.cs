using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
public class Prince : Character
{

    private bool healPotionPressed;
    private float healPotionCooldown = 12f;
    private bool canheal = true;

    void Start()
    {
        base.Initialized(100);
    }

    public void OnHitWith(Enemy enemy)
    {
        Debug.Log("Prince hit by Enemy. Enemy's DamageHit is: " + enemy.DamageHit);
        TakeDamage(enemy.DamageHit);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            OnHitWith(enemy);

        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)  && canheal)
        {
            healPotionPressed = true;
            canheal = false;

            Heal(10);

            StartCoroutine(HealPotionCooldownRoutine());
        }
    }

    private IEnumerator HealPotionCooldownRoutine()
    {
        yield return new WaitForSeconds(healPotionCooldown);
        canheal = true;
    }

    public void Heal(int healAmount)
    {
        if (healAmount < 0 || IsDead()) return;
        {
            Health += healAmount;

            base.ShowHealthBarThenHide();
        }
    }

    protected override void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Dead");
        }
        PlayerController prince = GetComponent<PlayerController>();
        if (prince != null)
        {
            prince.SetMovementLock(true);
        }

        Destroy(this.gameObject, 3f);
    }


}
