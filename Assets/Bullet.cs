using UnityEngine;

public class Bullet : MonoBehaviour
{
    public PlayerStats playerStats;
    public EnemyStats enemyStats;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
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

        if (isTargetTag)
        {
            Debug.Log("Player entered the trigger area, applying damage.");
            playerStats.TakeDamage(enemyStats.damage, transform.position);
            Destroy(gameObject);
        }

        if (isTargetLayer)
        {
            Debug.Log("Bullet hit target by tag or layer!");
            Destroy(gameObject);
        }
    }

}
