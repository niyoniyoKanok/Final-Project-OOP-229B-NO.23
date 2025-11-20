using UnityEngine;
using TMPro; 

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float disappearSpeed = 2f;
    [SerializeField] private float lifeTime = 1f;

    private Color textColor;
    private float disappearTimer;

    
    public void Setup(int damageAmount, bool isCritical)
    {
        textMesh.text = damageAmount.ToString();

        if (isCritical)
        {
            
            textMesh.fontSize += 3;
            textMesh.color = Color.red;
            textMesh.text += "!"; 
        }
        else
        {
           
            textMesh.color = Color.yellow;
        }

       
        textColor = textMesh.color;
        disappearTimer = lifeTime;
    }

    public void SetupHeal(int amount)
    {
        
        textMesh.text = "+" + amount.ToString();
        textMesh.color = Color.green;


        textMesh.fontSize = 8;

        textColor = textMesh.color;
        disappearTimer = lifeTime;
    }

    void Update()
    {
        
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);

      
        disappearTimer -= Time.deltaTime;

        if (disappearTimer < 0)
        {
           
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;

      
            if (textColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}