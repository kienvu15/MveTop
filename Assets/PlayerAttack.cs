using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public LayerMask enemyLayers;

    [Header("Slash VFX")]
    public GameObject[] slashVFXs;

    private PlayerCombo playerCombo;
    private PlayerRecoil playerRecoil;
    private PlayerFlip playerFlip;
    private PlayerInputHandler playerInputHandler;
    public PlayerStats playerStats;

    void Awake()
    {
        playerInputHandler = FindFirstObjectByType<PlayerInputHandler>();
        playerFlip = FindFirstObjectByType<PlayerFlip>();
        playerCombo = GetComponent<PlayerCombo>();
        playerRecoil = GetComponent<PlayerRecoil>();
        playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (playerFlip.canSeeEnemy)
        {
            RotateAttackPointToNearestEnemy();
        }
        else
        {
            RotationAttackPoint();
        }

        if (playerCombo.CanAttackNow())
            PerformAttack();
    }

    public void SetAttackPointDirection(Vector2 direction)
    {
        if (attackPoint == null) return;

        direction.Normalize();
        attackPoint.localPosition = direction * playerStats.attackRange;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        attackPoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void RotationAttackPoint()
    {
        if (playerInputHandler.MoveInput.y > 0)
            SetAttackPointDirection(Vector2.up);

        if (playerInputHandler.MoveInput.y < 0)
            SetAttackPointDirection(Vector2.down);

        if (playerInputHandler.MoveInput.x > 0) // Chém phải
            SetAttackPointDirection(Vector2.right);

        if (playerInputHandler.MoveInput.x < 0)  // Chém trái
            SetAttackPointDirection(Vector2.left);

    }

    void RotateAttackPointToNearestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, playerFlip.PlayerVisionRadius, enemyLayers);
        if (enemies.Length == 0) return;

        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy.transform;
            }
        }

        if (nearest == null) return;

        // Tính hướng từ player đến enemy
        Vector2 direction = (nearest.position - transform.position).normalized;

        // ✅ Đặt vị trí local trên đường tròn attackRange
        attackPoint.localPosition = direction * playerStats.attackRange;

        // ✅ Xoay attackPoint về phía enemy
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        attackPoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void PerformAttack()
    {
        playerCombo.NextComboStep();

        ActivateSlashVFX();
        playerCombo.TryDashAttack();

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, playerStats.attackRadius, enemyLayers);

        foreach (Collider2D hit in hits)
        {
            hit.GetComponent<EnemyStats>()?.TakeDamage(playerStats.damage);
        }

        if (hits.Length > 0)
            playerRecoil.ApplyRecoil(attackPoint.position);
    }



    void ActivateSlashVFX()
    {
        foreach (var vfx in slashVFXs)
            vfx.SetActive(false);

        int index = Random.Range(0, slashVFXs.Length);
        slashVFXs[index].SetActive(true);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerStats.attackRange);

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, playerStats.attackRadius);
        }
    }
}
