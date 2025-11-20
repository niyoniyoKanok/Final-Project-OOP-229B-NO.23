using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Top Left Icon")]
    public Image pathIconDisplay;

    [Header("Dependencies")]
    public Prince princeRef; 


    [Header("Tab Info Panel")]
    public GameObject tabInfoPanel;

    public TextMeshProUGUI tabPathName;

  
    [SerializeField] private GameObject inGameHUD;


    public Image s1Icon;
    public TextMeshProUGUI s1Desc;
    public GameObject s1LockOverlay;


    public Image s2Icon; public TextMeshProUGUI s2Desc; public GameObject s2LockOverlay;
    public Image s3Icon; public TextMeshProUGUI s3Desc; public GameObject s3LockOverlay;

    public PlayerLevel playerLevel;
    private PathData currentData;

    public void UpdatePathIcon(PathData data)
    {
        currentData = data;


        pathIconDisplay.sprite = data.pathIcon;
        pathIconDisplay.gameObject.SetActive(true);



        tabPathName.text = data.pathName;

        s1Icon.sprite = data.skill1Icon; s1Desc.text = data.skill1Desc;
        s2Icon.sprite = data.skill2Icon; s2Desc.text = data.skill2Desc;
        s3Icon.sprite = data.skill3Icon; s3Desc.text = data.skill3Desc;

        UpdateSkillDescriptions();
    }

    public void UpdateSkillDescriptions()
    {
        if (currentData == null || princeRef == null) return;

        // 💡 ดึงค่า Description เริ่มต้นจาก PathData
        string desc1 = currentData.skill1Desc;
        string desc2 = currentData.skill2Desc;
        string desc3 = currentData.skill3Desc;

        switch (currentData.pathType)
        {
            case PathType.Death:
                {
                    float cd1 = princeRef.deathCircleInterval;
                    float cd2 = princeRef.deathSliceInterval;
                    float cd3 = princeRef.curseShoutInterval;

                    // 🛠️ แก้ไข: ใช้ string.Format และ ToString("F1") เพื่อบังคับการแสดงผลทศนิยม 1 ตำแหน่ง
                    s1Desc.text = string.Format(desc1, cd1.ToString("F1"));
                    s2Desc.text = string.Format(desc2, cd2.ToString("F1"));
                    s3Desc.text = string.Format(desc3, cd3.ToString("F1"));
                    break;
                }

            case PathType.Star:
                {
                    float cd1 = princeRef.starImpactInterval;
                    float cd2 = princeRef.starFallingInterval;
                    float cd3 = princeRef.arcaneCometInterval;

                    s1Desc.text = string.Format(desc1, cd1.ToString("F1"));
                    s2Desc.text = string.Format(desc2, cd2.ToString("F1"));
                    s3Desc.text = string.Format(desc3, cd3.ToString("F1"));
                    break;
                }

            case PathType.Vampire:
                {
                    float cd1 = princeRef.batSpawnInterval;
                    float cd2 = princeRef.vampireBladeInterval;
                    float cd3 = princeRef.vampireScratchInterval;

                    s1Desc.text = string.Format(desc1, cd1.ToString("F1"));
                    s2Desc.text = string.Format(desc2, cd2.ToString("F1"));
                    s3Desc.text = string.Format(desc3, cd3.ToString("F1"));
                    break;
                }

            default:
                // หากไม่มี Path ถูกเลือก หรือเกิดข้อผิดพลาด
                s1Desc.text = desc1;
                s2Desc.text = desc2;
                s3Desc.text = desc3;
                break;
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleTabInfo();
        }


        if (tabInfoPanel.activeSelf && currentData != null)
        {
            int lvl = playerLevel.CurrentLevel;


            s1LockOverlay.SetActive(lvl < 3);

            s2LockOverlay.SetActive(lvl < 7);

            s3LockOverlay.SetActive(lvl < 11);
        }
    }

    public void ToggleTabInfo()
    {
        bool isCurrentlyActive = tabInfoPanel.activeSelf;
        bool newActiveState = !isCurrentlyActive; 

        
        tabInfoPanel.SetActive(newActiveState);

     
        if (inGameHUD != null)
        {
            inGameHUD.SetActive(!newActiveState);
        }

      
        if (newActiveState)
        {
         
            Time.timeScale = 0f;
        }
        else
        {
            
            Time.timeScale = 1f;
        }
    }


}