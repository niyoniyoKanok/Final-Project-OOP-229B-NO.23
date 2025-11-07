using UnityEngine;

public class Character : MonoBehaviour
{
    private int health;
    public int Health
    {
        get { return health; }
        set { health = (value < 0) ? 0 : value; }
    }

    private int maxHealth;

    [SerializeField] HealthBar healthBar;

    public void Initialized(int starterHealth)
    {
        Health = starterHealth;
        maxHealth = starterHealth;

        healthBar = GetComponent<HealthBar>();
    }

    public bool IsDead()
    {
        if (Health <= 0)
        {
            Destroy(this.gameObject);
            return true;
        }

        else { return false; }
    }


}
