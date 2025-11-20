using UnityEngine;
public enum UpgradeType
{
    AttackDamage,
    MaxHealth,
    PotionHeal,
    CooldownReduction,
    SwordWaveDamage,
    AttackSpeed,
    XPMultiplier
}
[CreateAssetMenu(fileName = "UpgradeData", menuName = "Scriptable Objects/UpgradeData")]

public class UpgradeData : ScriptableObject
{
    public string upgradeName;

   
    [TextArea] public string description;

    public Sprite icon;
    public UpgradeType type;

    [Header("Random Range")]
    public float minValue;
    public float maxValue;


    public bool isInteger = false;
}
