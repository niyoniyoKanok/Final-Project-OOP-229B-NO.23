using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PathSelectionCard : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Button selectButton;

    private PathData myData;
    private PathSelector selectorRef;

    public void Setup(PathData data, PathSelector selector)
    {
        myData = data;
        selectorRef = selector;


        if (iconImage != null) iconImage.sprite = data.pathIcon;
        if (nameText != null) nameText.text = data.pathName;
        if (descriptionText != null) descriptionText.text = data.pathDescription;

       
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnCardClicked);
    }

    private void OnCardClicked()
    {
        if (selectorRef != null)
        {
            selectorRef.SelectPath(myData);
        }
    }
}