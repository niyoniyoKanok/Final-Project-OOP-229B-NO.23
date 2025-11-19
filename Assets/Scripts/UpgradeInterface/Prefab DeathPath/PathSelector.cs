using UnityEngine;
using System.Collections.Generic;

public class PathSelector : MonoBehaviour
{
    [Header("Data")]
    public List<PathData> allPaths; 

    [Header("UI")]
    public GameObject cardPrefab;
    public Transform cardContainer; 
    public GameObject selectionPanel;

    [Header("References")]
    public Prince prince;
    public HUDManager hudManager;

    void Start()
    {
        Time.timeScale = 0f; 

        // สร้างการ์ดตามข้อมูลที่มี
        foreach (var path in allPaths)
        {
            GameObject card = Instantiate(cardPrefab, cardContainer);
            card.GetComponent<PathSelectionCard>().Setup(path, this);
        }
    }

    public void SelectPath(PathData data)
    {
        // ส่งข้อมูลไปให้ Prince และ HUD
      prince.SetPathData(data);
        hudManager.UpdatePathIcon(data);

        // เริ่มเกม
        Time.timeScale = 1f;
        selectionPanel.SetActive(false);
    }
}