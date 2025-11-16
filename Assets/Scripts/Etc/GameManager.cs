using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject gameOverPanel;

    [SerializeField] private GameObject inGameHUD;

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

        if (inGameHUD != null)
        {
            inGameHUD.SetActive(true);
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

        if (inGameHUD != null)
        {
            inGameHUD.SetActive(false);
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
