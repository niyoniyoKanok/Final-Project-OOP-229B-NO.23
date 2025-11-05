using Unity.VisualScripting;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    private float movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement = Input.GetAxis("Horizontal");
    }
}
