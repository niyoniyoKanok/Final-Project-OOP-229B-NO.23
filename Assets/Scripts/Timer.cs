using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;

    [SerializeField] GameObject gameOverPanel; 
    private bool timerIsRunning = true; 

    void Start()
    {
     
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void Update()
    {

        if (!timerIsRunning)
        {
            return;
        }


        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime; 
        }
        else 
        {
            remainingTime = 0;
            timerIsRunning = false; 

            
            GameOver();
        }

     
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

 
    void GameOver()
    {
        timerText.color = Color.red; 

  
        Time.timeScale = 0f;

       
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

    }
}