using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Settings")]
    [SerializeField] private float duration = 900f;

    private float remainingTime;
    public float MaxTime { get; private set; }

    
    private bool isEnded = false;


    public bool IsOvertime => false;
    public float OvertimeDuration => 0f;

    void Awake()
    {
        remainingTime = duration;
        MaxTime = duration;
    }

    void Update()
    {
        if (isEnded) return; 

        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            remainingTime = 0;
            UpdateTimerUI();

        
            FinishLevel();
        }
    }

    void FinishLevel()
    {
        isEnded = true;
        Debug.Log("STAGE CLEAR!");

        
        GameManager.Instance.RestartGame();

        
       Time.timeScale = 0f; 
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            timerText.color = Color.white;
        }
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }

    public float GetTotalElapsedTime()
    {
      
        return MaxTime - remainingTime;
    }
}