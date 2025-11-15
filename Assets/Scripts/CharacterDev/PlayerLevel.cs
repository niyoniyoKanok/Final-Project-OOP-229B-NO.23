using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerLevel : MonoBehaviour
{
    [Header("XP Settings")]
    [SerializeField] private int xpPerKill = 10;
    [SerializeField] private int baseXPToLevelUp = 100;
    [SerializeField] private float xpMultiplier = 1.5f;



    [Header("UI References")]
    [SerializeField] private Slider xpSlider;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private TextMeshProUGUI xpText;

    [Header("Game System")]
    [SerializeField] private LevelUpManager levelUpManager;

    public int CurrentLevel { get; private set; }
    public int CurrentXP { get; private set; }
    public int XpToNextLevel { get; private set; }

    void Start()
    {
        CurrentLevel = 1;
        CurrentXP = 0;
        XpToNextLevel = baseXPToLevelUp;
        UpdateUI();
    }

    public void AddXP()
    {
        CurrentXP += xpPerKill;


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

       
        float newXpRequired = XpToNextLevel * xpMultiplier;
        XpToNextLevel = Mathf.RoundToInt(newXpRequired);

      


        Debug.Log("LEVEL UP! New Level: " + CurrentLevel + " | XP Needed: " + XpToNextLevel);
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
            levelText.text = "LV " + CurrentLevel;
        }

        if (xpText != null)
        {
            xpText.text = string.Format("{0} / {1}", CurrentXP, XpToNextLevel);
        }
    }
}

