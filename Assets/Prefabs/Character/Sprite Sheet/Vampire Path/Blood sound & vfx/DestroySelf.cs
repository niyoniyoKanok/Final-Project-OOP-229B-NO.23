using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.4f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}