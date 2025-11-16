using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLevel : MonoBehaviour
{
    [Header("XP Settings")]
    [SerializeField] private int xpPerKill = 10;
    [SerializeField] private int baseXPToLevelUp = 100;
    [SerializeField] private float xpGrowthMultiplier = 1.5f;

    [Header("Permanent Stat Gains")]
    [SerializeField] private int attackDamagePerLevel = 2;

    [Header("UI References")]
    [SerializeField] private Slider xpSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI killCountText;

    private Prince princeRef;

    public int CurrentLevel { get; private set; }
    public int CurrentXP { get; private set; }
    public int XpToNextLevel { get; private set; }
    public int KillCount { get; private set; }

    [Header("Bonus (%)")]
    [Tooltip("XP Bonus in percent. Example: 7 = +7% xp gain")]
    public float BonusXPMultiplier = 0f; 

    void Start()
    {
        CurrentLevel = 1;
        CurrentXP = 0;
        XpToNextLevel = baseXPToLevelUp;
        KillCount = 0;

        princeRef = GetComponent<Prince>();

        UpdateUI();
    }

    public void AddXP()
    {
  
        float bonusRate = 1f + (BonusXPMultiplier / 100f);

        int gainedXP = Mathf.RoundToInt(xpPerKill * bonusRate);

        CurrentXP += gainedXP;
        KillCount++;

      
        while (CurrentXP >= XpToNextLevel)
        {
            LevelUp();
        }

        UpdateUI();
    }

    private void LevelUp()
    {
        CurrentLevel++;
        CurrentXP -= XpToNextLevel;


        if (princeRef != null)
        {
            princeRef.BonusAttackDamage += attackDamagePerLevel;
        }

       
        XpToNextLevel = Mathf.RoundToInt(XpToNextLevel * xpGrowthMultiplier);
    }

    private void UpdateUI()
    {
        if (xpSlider != null)
        {
            xpSlider.maxValue = XpToNextLevel;
            xpSlider.value = CurrentXP;
        }

        if (levelText != null)
        {
            levelText.text = $"LV {CurrentLevel}";
        }

        if (xpText != null)
        {
            xpText.text = $"{CurrentXP} / {XpToNextLevel}";
        }

        if (killCountText != null)
        {
            killCountText.text = $"Kills: {KillCount}";
        }
    }
}
