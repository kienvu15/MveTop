using UnityEngine;

public class ArrowDamage : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayers;
    public int damage = 1; // Damage dealt by the arrow
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isPlayerTargetTag = collision.CompareTag("Player");
        bool isEnemyTargetTag = collision.CompareTag("Enemy");
        bool isTargetLayer = ((1 << collision.gameObject.layer) & targetLayers) != 0;

        if (isPlayerTargetTag)
        {
            Debug.Log("Player entered the trigger area, applying damage.");
            PlayerStats playerStats = collision.GetComponent<PlayerStats>();
            playerStats.TakeDamage(damage, transform.position);
            Destroy(gameObject);
        }

        if (isEnemyTargetTag)
        {
            Debug.Log("Enemy hit by arrow, applying damage.");
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamageNoRecoil(1f);
            }
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
            PlayerStats playerStats = collision.GetComponent<PlayerStats>();
        }
    }
}
