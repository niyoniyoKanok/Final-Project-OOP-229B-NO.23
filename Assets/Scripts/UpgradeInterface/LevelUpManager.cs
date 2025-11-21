using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private UpgradeButton[] optionButtons;

    [Header("Data")]
    [SerializeField] private List<UpgradeData> allUpgrades;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip selectSound;

    private Prince player;
    private int levelsToProcess = 0;
    void Awake()
    {
        if (Instance == null) Instance = this;
        levelUpPanel.SetActive(false);
    }

    void Start()
    {
        player = FindFirstObjectByType<Prince>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
               
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }


    public void ShowLevelUpOptions()
    {

        Time.timeScale = 0f;
        levelUpPanel.SetActive(true);


        List<UpgradeData> choices = new List<UpgradeData>();
        List<UpgradeData> pool = new List<UpgradeData>(allUpgrades);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (pool.Count > 0)
            {
                int randomIndex = Random.Range(0, pool.Count);
                choices.Add(pool[randomIndex]);
                pool.RemoveAt(randomIndex);
            }
        }


        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < choices.Count)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].Setup(choices[i], this);
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }

        levelsToProcess++;


        if (!levelUpPanel.activeSelf)
        {
            Time.timeScale = 0f;
            levelUpPanel.SetActive(true);
            GenerateCards();
        }
    }
    private void GenerateCards()
    {
        List<UpgradeData> choices = new List<UpgradeData>();
        List<UpgradeData> pool = new List<UpgradeData>(allUpgrades);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (pool.Count > 0)
            {
                int randomIndex = Random.Range(0, pool.Count);
                choices.Add(pool[randomIndex]);
                pool.RemoveAt(randomIndex);
            }
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < choices.Count)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].Setup(choices[i], this);
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }
    public void SelectUpgrade(UpgradeData data, float rolledValue)
    {
        // เล่นเสียง
        if (audioSource != null && selectSound != null)
        {
            audioSource.PlayOneShot(selectSound);
        }

        // อัปเกรดค่าให้ผู้เล่น
        if (player != null)
        {
            player.ApplyUpgrade(data, rolledValue);
        }

        // 🛠️ ลดจำนวนคิวลง 1
        levelsToProcess--;

        // 3. เช็คว่ายังมีเลเวลค้างอยู่ไหม?
        if (levelsToProcess > 0)
        {
            // ถ้ายังมีค้าง -> สุ่มการ์ดชุดใหม่ทันที (หน้าต่างไม่ปิด)
            GenerateCards();
        }
        else
        {
            // ถ้าหมดคิวแล้ว -> ปิดหน้าต่าง + เดินเกมต่อ
            levelUpPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

}