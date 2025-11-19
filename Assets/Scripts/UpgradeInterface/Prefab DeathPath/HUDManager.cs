using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Top Left Icon")]
    public Image pathIconDisplay;

    [Header("Tab Info Panel")]
    public GameObject tabInfoPanel; 
 
    public TextMeshProUGUI tabPathName;

   
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
        bool isActive = tabInfoPanel.activeSelf;

       
        tabInfoPanel.SetActive(!isActive);
    }
}