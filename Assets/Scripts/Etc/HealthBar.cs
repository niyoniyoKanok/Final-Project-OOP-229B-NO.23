using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [Header("HP Bar")]
    public Slider slider;
    public TextMeshProUGUI HealthBarValue; 

    [Header("Shield Bar (New)")]
    public Slider shieldSlider; 

  
    public void UpdateBar(int currentHealth, int maxHealth, int shield = 0)
    {
       
        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = currentHealth;
        }

        if (shieldSlider != null)
        {
           
            bool shouldShow = shield > 0;
            shieldSlider.gameObject.SetActive(shouldShow);

            if (shouldShow)
            {
                shieldSlider.maxValue = maxHealth;
                shieldSlider.value = shield;
            }
        }

        if (HealthBarValue != null)
        {
            if (shield > 0)
            {
                HealthBarValue.text = $"{currentHealth} / {maxHealth} <color=#FFD700>(+{shield})</color>";
            }
            else
            {
                HealthBarValue.text = $"{currentHealth} / {maxHealth}";
            }
        }
    }

}