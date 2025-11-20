using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;

    private UpgradeData myData;
    private LevelUpManager manager;


    private float rolledValue;

    public void Setup(UpgradeData data, LevelUpManager mgr)
    {
        myData = data;
        manager = mgr;

      
        if (data.isInteger)
        {
        
            rolledValue = Random.Range(Mathf.RoundToInt(data.minValue), Mathf.RoundToInt(data.maxValue) + 1);
        }
        else
        {
            rolledValue = Random.Range(data.minValue, data.maxValue);
        }

        
        if (iconImage != null) iconImage.sprite = data.icon;
        if (nameText != null) nameText.text = data.upgradeName;

        if (descText != null)
        {
            
            string valueString = data.isInteger ? rolledValue.ToString("0") : rolledValue.ToString("0.##");


            if (data.type == UpgradeType.AttackSpeed ||
                 data.type == UpgradeType.CooldownReduction ||
                 data.type == UpgradeType.XPMultiplier)
            {

                if (data.type == UpgradeType.AttackSpeed)
                    valueString = (rolledValue * 100f).ToString("0.#") + "%";

                else
                    valueString = valueString + "%";
            }

          
            descText.text = string.Format(data.description, valueString);
        }
    }

    public void OnClick()
    {
        if (manager != null)
        {
            manager.SelectUpgrade(myData, rolledValue);
        }
    }
}