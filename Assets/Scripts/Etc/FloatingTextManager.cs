using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance;

    [Header("Prefab")]
    [SerializeField] private GameObject textPrefab;

    void Awake()
    {
        
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowDamage(int damage, Vector3 position, bool isCritical = false)
    {
        if (textPrefab == null) return;

        
        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.5f, 1.0f), 0);

        GameObject go = Instantiate(textPrefab, position + randomOffset, Quaternion.identity);

        FloatingText ft = go.GetComponent<FloatingText>();
        if (ft != null)
        {
            ft.Setup(damage, isCritical);
        }
    }

    public void ShowHeal(int amount, Vector3 position)
    {
        if (textPrefab == null) return;

        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.5f, 1.0f), 0);

        GameObject go = Instantiate(textPrefab, position + randomOffset, Quaternion.identity);

        FloatingText ft = go.GetComponent<FloatingText>();
        if (ft != null)
        {
            ft.SetupHeal(amount);
        }
    }
}