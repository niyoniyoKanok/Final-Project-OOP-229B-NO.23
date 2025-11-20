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


    public float OvertimeDuration { get; private set; } = 0f;
    public bool IsOvertime => remainingTime <= 0;

    void Awake()
    {
        remainingTime = duration;
        MaxTime = duration;
    }

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerUI(false);
        }
        else
        {
        
            remainingTime = 0;
            OvertimeDuration += Time.deltaTime;
            UpdateTimerUI(true);
        }
    }

    void UpdateTimerUI(bool isOvertime)
    {
        if (timerText != null)
        {
            if (!isOvertime)
            {
                
                int minutes = Mathf.FloorToInt(remainingTime / 60);
                int seconds = Mathf.FloorToInt(remainingTime % 60);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                timerText.color = Color.white;
            }
            else
            {
               
                int minutes = Mathf.FloorToInt(OvertimeDuration / 60);
                int seconds = Mathf.FloorToInt(OvertimeDuration % 60);
                timerText.text = string.Format("+{0:00}:{1:00}", minutes, seconds);
                timerText.color = Color.red;
            }
        }
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }

    // ฟังก์ชันสำหรับหาเวลาที่ผ่านไปทั้งหมด (Normal + Overtime)
    public float GetTotalElapsedTime()
    {
        if (IsOvertime)
            return MaxTime + OvertimeDuration;
        else
            return MaxTime - remainingTime;
    }
}