using UnityEngine;
using UnityEngine.UI; 

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public void UpdateBar(int currentHealth, int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
    }
}