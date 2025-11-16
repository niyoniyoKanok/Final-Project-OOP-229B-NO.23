using UnityEngine;
using TMPro;
public class StatPanelUI : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private GameObject statPanel;

    [SerializeField] private GameObject inGameHUD;

    [Header("Stat Scripts")]
    [SerializeField] private Prince prince; 
    [SerializeField] private PlayerLevel playerLevel; 
    [SerializeField] private Character playerCharacter; 

    [Header("Stat Texts")]
    [SerializeField] private TextMeshProUGUI attackPowerText;
    [SerializeField] private TextMeshProUGUI cooldownReductionText;
    [SerializeField] private TextMeshProUGUI potionHealText;
    [SerializeField] private TextMeshProUGUI xpBonusText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI swordWaveDamageText;
    [SerializeField] private TextMeshProUGUI maxHealthText;


    private bool isPanelOpen = false;

    void Start()
    {
        if (statPanel != null)
        {
            statPanel.SetActive(false);
        }

        if (inGameHUD != null)
        {
            inGameHUD.SetActive(true);
        }
    }

   
    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Time.timeScale == 0f && !isPanelOpen)
            {
                return;
            }

           
            isPanelOpen = !isPanelOpen;

            if (isPanelOpen)
            {
                
                Time.timeScale = 0f; 
                UpdateStatDisplay(); 
                statPanel.SetActive(true);

                if (inGameHUD != null) inGameHUD.SetActive(false);
            }
            else
            {
                
                Time.timeScale = 1f; 
                statPanel.SetActive(false);

                if (inGameHUD != null)
                {
                    inGameHUD.SetActive(true);
                    ForceEnableChildren(inGameHUD);
                }
            }

        }
    }

    void UpdateStatDisplay()
    {
        if (prince == null || playerLevel == null || playerCharacter == null) return;

        attackPowerText.text = "Attack Power: " + prince.BaseAttackDamage + " + " + prince.BonusAttackDamage;


        cooldownReductionText.text = "Skill Cooldown Reduction: " + (prince.BonusCooldownReduction * 100) + "%";


        potionHealText.text = "Potion Heal: " + prince.BasePotionHeal + " + " + prince.BonusPotionHeal;


        xpBonusText.text = "XP Bonus Increase: " + ((playerLevel.BonusXPMultiplier - 1.0f) * 100) + "%";

     
        attackSpeedText.text = "Attack Speed: " + (prince.BonusAttackSpeed * 100) + "%";

        
        swordWaveDamageText.text = "SwordWave Damage: " + (prince.SwordWaveDamage + prince.BonusSwordWaveDamage);

 
        maxHealthText.text = "Max Health: " + playerCharacter.MaxHealth;
    }

    void ForceEnableChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}

