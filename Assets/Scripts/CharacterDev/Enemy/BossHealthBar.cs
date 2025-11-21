using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Enemy bossReference;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient colorGradient;


    [Header("Text Display")]
    [SerializeField] private TextMeshProUGUI healthText;

    void Start()
    {
        if (bossReference != null && healthSlider != null)
        {
            healthSlider.maxValue = bossReference.MaxHealth;
            healthSlider.value = bossReference.MaxHealth;

            if (fillImage != null)
                fillImage.color = colorGradient.Evaluate(1f);
        }
    }

    void Update()
    {
        if (bossReference == null)
        {
            Destroy(gameObject);
            return;
        }

     
        healthSlider.maxValue = bossReference.MaxHealth;
        healthSlider.value = bossReference.Health;

        if (healthText != null)
        {
            healthText.text = $"{bossReference.Health} / {bossReference.MaxHealth}";
        }

        
        if (fillImage != null)
        {
            float hpPercent = (float)bossReference.Health / bossReference.MaxHealth;
            fillImage.color = colorGradient.Evaluate(hpPercent);
        }

        
        transform.rotation = Quaternion.identity;
    }
}