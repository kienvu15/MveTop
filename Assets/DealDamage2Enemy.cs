using UnityEngine;

public class DealDamage2Enemy : MonoBehaviour
{
    [SerializeField] public PlayerStats playerStats;
    [SerializeField] public Transform attackPoint;
    [SerializeField] public PlayerRecoil playerRecoil;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private bool hasHitEnemy = false;

    void Start()
    {
        playerStats = GetComponentInParent<PlayerStats>();
    }

    //private void OnEnable()
    //{
    //    hasHitEnemy = false; // Reset mỗi lần kích hoạt
    //}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayers) == 0)
            return; // Không phải enemy

        EnemyStats enemy = other.GetComponent<EnemyStats>();
        if (enemy != null)
        {
            enemy.TakeDamage(playerStats.damage, transform.position);
            Debug.Log($"DealDamage2Enemy: Dealt {playerStats.damage} damage to {enemy.name} at position {transform.position}");
            if (!hasHitEnemy)
            {
                hasHitEnemy = true;
                playerRecoil.ApplyRecoil(attackPoint.position);
            }
        }
    }
}
