using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject gameOverPanel;

    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    public void ShowGameOverScreen()
    {
        Time.timeScale = 0f;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
  
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToLobby()
    {
        Time.timeScale = 1f; 
        
        SceneManager.LoadScene("Lobby");
    }
}
