using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [SerializeField] private LayerMask targetLayers;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isTargetTag = collision.CompareTag("Player");
        bool isTargetLayer = ((1 << collision.gameObject.layer) & targetLayers) != 0;

        if (isTargetTag || isTargetLayer)
        {
            Debug.Log("Bullet hit target by tag or layer!");
            Destroy(gameObject);
        }
    }

}
