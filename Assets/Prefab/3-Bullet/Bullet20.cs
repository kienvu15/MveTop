using UnityEngine;

public class Bullet20 : MonoBehaviour
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
    [SerializeField] private LayerMask dearDamageLayer;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyStats enemy = collision.GetComponent<EnemyStats>();
        bool isEnemyLayer = ((1 << collision.gameObject.layer) & dearDamageLayer) != 0;
        bool isTargetLayer = ((1 << collision.gameObject.layer) & targetLayers) != 0;

        if (isEnemyLayer)
        {
            Debug.Log("Bullet hit an enemy, applying damage.");
            enemy.TakeDamage(playerStats.damage, transform.position);
            Destroy(gameObject);
        }

        if (isTargetLayer)
        {
            Debug.Log("Bullet hit target by tag or layer!");
            Destroy(gameObject);
        }
    }

}
