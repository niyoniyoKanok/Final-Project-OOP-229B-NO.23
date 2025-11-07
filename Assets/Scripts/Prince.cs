using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Prince : Character
{

    [Header("Game Dependencies")]
    public Camera playerCamera;

    [Header("Ability 1")]
    public Image AbilityImage1;
    public Text AbilityText1;
    public KeyCode ability1Key;
    public float Ability1Cooldown = 7f;
    private Vector2 teleportTarget;
    private bool teleportPressed;

    [Header("Ability 2")]
    public Image AbilityImage2;
    public Text AbilityText2;
    public KeyCode ability2Key;
    public float Ability2Cooldown = 5f;


    [Header("Ability 3")]
    public Image AbilityImage3;
    public Text AbilityText3;
    public KeyCode ability3Key;
    public float Ability3Cooldown = 15f;

    public AudioSource audioSource;
    public AudioClip HitSound;
    public AudioClip HealSound;
    public AudioClip TeleportSound;

    private bool isAbility1Cooldown = false;
    private bool isAbility2Cooldown = false;
    private bool isAbility3Cooldown = false;

    private float currentAbility1Cooldown;
    private float currentAbility2Cooldown;
    private float currentAbility3Cooldown;
    void Start()
    {
        base.Initialized(100);
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = GetComponent<Animator>();

        AbilityImage1.fillAmount = 0;
        AbilityImage2.fillAmount = 0;
        AbilityImage3.fillAmount = 0;

        AbilityText1.text = "";
        AbilityText2.text = "";
        AbilityText3.text = "";
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

            audioSource.PlayOneShot(HitSound);
        }
    }
    void Update()
    {
        Ability1Input();
        Ability2Input();
        Ability3Input();

        AbilityCooldown(ref currentAbility1Cooldown, Ability1Cooldown, ref isAbility1Cooldown, AbilityImage1, AbilityText1);
        AbilityCooldown(ref currentAbility2Cooldown, Ability2Cooldown, ref isAbility2Cooldown, AbilityImage2, AbilityText2);
        AbilityCooldown(ref currentAbility3Cooldown, Ability3Cooldown, ref isAbility3Cooldown, AbilityImage3, AbilityText3);
    }

    public void Ability1Input()
    { 
        if (Input.GetKeyDown(ability1Key) && !isAbility1Cooldown)
        {
            isAbility1Cooldown = true;
            currentAbility1Cooldown = Ability2Cooldown;

            Ability1Teleport();
        }
    }

    public void Ability1Teleport()
    {
        animator.SetTrigger("Teleport");

        if (TeleportSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(TeleportSound);
        }



        Vector3 mouseScreenPos = Input.mousePosition;


        mouseScreenPos.z = playerCamera.transform.position.z * -1;

       
        Vector3 worldPos = playerCamera.ScreenToWorldPoint(mouseScreenPos);
        teleportTarget = new Vector2(worldPos.x, worldPos.y);

        rb.position = teleportTarget; 
        rb.linearVelocity = Vector2.zero; 
        teleportPressed = true;


    }

    public void Ability2Input()
    {
        if (Input.GetKeyDown(ability2Key) && !isAbility2Cooldown)
        {
            isAbility2Cooldown = true;
            currentAbility2Cooldown = Ability2Cooldown;
        }
    }

    public void Ability3Input()
    {
        if (Input.GetKeyDown(ability3Key) && !isAbility3Cooldown)
        {

            Ability3Heal(10);
            isAbility3Cooldown = true;
            currentAbility3Cooldown = Ability3Cooldown;
            audioSource.PlayOneShot(HealSound);
        }
    }

    public void Ability3Heal(int healAmount)
    {
        if (healAmount < 0 || IsDead())
        {
            return;
        }
        Health += healAmount;
        base.ShowHealthBarThenHide();
    }

    private void AbilityCooldown(ref float currentCooldown, float maxCooldown, ref bool isCooldown, Image skillImage, Text skillText)
    {
        if (isCooldown)
        {
            currentCooldown -= Time.deltaTime;

           
            if (currentCooldown <= 0f)
            {
          
                isCooldown = false;
                currentCooldown = 0f;

                if (skillImage != null)
                {
                    skillImage.fillAmount = 0f;
                }
                if (skillText != null)
                {
                    skillText.text = "";
                }
            }
            
            else
            {
     
                if (skillImage != null)
                {
                    skillImage.fillAmount = currentCooldown / maxCooldown;
                }
                if (skillText != null)
                {
                  
                    skillText.text = Mathf.Ceil(currentCooldown).ToString();
                }
            }
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
