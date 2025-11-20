using UnityEngine;
using TMPro;

public class FloodWarningUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI warningText;

    [Header("Animation Settings")]
    [SerializeField] private float pulseSpeed = 5f;
    [SerializeField] private float minScale = 1.0f; 
    [SerializeField] private float maxScale = 1.5f; 

    private void Start()
    {
        // ซ่อนตอนเริ่มเกม
        if (warningText != null) warningText.gameObject.SetActive(false);
    }

    public void ShowWarning(string message)
    {
        if (warningText != null)
        {
            warningText.text = message;
            warningText.color = Color.red;
            warningText.gameObject.SetActive(true);
        }
    }

    public void HideWarning()
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
      
        if (warningText != null && warningText.gameObject.activeSelf)
        {
           
            float scale = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2f);

         

            warningText.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }
}