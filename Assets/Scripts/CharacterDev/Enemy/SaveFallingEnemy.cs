using UnityEngine;

public class SaveFallingEnemy : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fallLimitY = -4.75f; 
    [SerializeField] private LayerMask groundLayer; 
    [SerializeField] private float resetHeightOffset = 0.5f; 

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        
        if (transform.position.y < fallLimitY)
        {
            WarpBackToGround();
        }
    }

    private void WarpBackToGround()
    {
        
        Vector2 rayOrigin = new Vector2(transform.position.x, 10f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 20f, groundLayer);

        if (hit.collider != null)
        {
           
            transform.position = new Vector2(transform.position.x, hit.point.y + resetHeightOffset);
            Debug.Log($"Rescued {name} from falling!");
        }
        else
        {
          
            transform.position = Vector2.zero;
        }

       
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
      
        }
    }
}