using UnityEngine;

public class ArrowDamage : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayers;
    public PlayerStats playerStats;
    public int damage = 1; // Damage dealt by the arrow
    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isTargetTag = collision.CompareTag("Player");
        bool isTargetLayer = ((1 << collision.gameObject.layer) & targetLayers) != 0;

        if (isTargetTag)
        {
            Debug.Log("Player entered the trigger area, applying damage.");
            playerStats.TakeDamage(damage, transform.position);
            Destroy(gameObject);
        }
        
        if (isTargetLayer)
        {
            Debug.Log("Target layer wall obstacle hit");
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player is still in the trigger area, applying damage.");
            playerStats.TakeDamage(damage, transform.position);
        }
    }
}
