using UnityEngine;
using UnityEngine.SceneManagement;
// using UnityEngine.Rendering; // ไม่ได้ใช้ใน logic นี้ ลบออกได้ถ้าไม่จำเป็น

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject gameOverPanel;

    // 1. เพิ่มตัวแปรสำหรับหน้าเมนู Pause (Menu ที่จะเด้งตอนกด Esc)
    [SerializeField] private GameObject pauseMenuPanel;

    [SerializeField] private GameObject inGameHUD;

    // 2. ตัวแปรเช็คสถานะว่าเกมหยุดอยู่ไหม
    private bool isPaused = false;

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
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // ซ่อนหน้า Pause ตอนเริ่มเกม
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

        if (inGameHUD != null) inGameHUD.SetActive(true);

        Time.timeScale = 1f;
        isPaused = false;
    }

    // 3. เพิ่ม Update เพื่อรอรับปุ่ม Esc
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ป้องกันการกด Pause ซ้ำซ้อนตอน Game Over ไปแล้ว
            if (gameOverPanel.activeSelf) return;

            TogglePause();
        }
    }

    // 4. ฟังก์ชันสลับสถานะ หยุด/เล่นต่อ
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // หยุดเกม
            Time.timeScale = 0f;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
            if (inGameHUD != null) inGameHUD.SetActive(false);
        }
        else
        {
            // เล่นต่อ (Resume)
            Time.timeScale = 1f;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            if (inGameHUD != null) inGameHUD.SetActive(true);
        }
    }

    public void ShowGameOverScreen()
    {
        isPaused = true; // กันไม่ให้กด Esc ซ้อน
        Time.timeScale = 0f;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false); // ปิด Pause ถ้าตาย
        if (inGameHUD != null) inGameHUD.SetActive(false);
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

    // เพิ่มฟังก์ชันสำหรับปุ่ม Resume (เผื่อทำปุ่มกดกลับเข้าเกม)
    public void ResumeGame()
    {
        if (isPaused)
        {
            TogglePause();
        }
    }
}