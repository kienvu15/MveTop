using UnityEngine;

public class PlayerArrowShooter : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public LayerMask enemyLayers;

    public GameObject arrowPrefab;             // Prefab mũi tên             // Vị trí bắn
    public float arrowSpeed = 10f;             // Tốc độ mũi tên
    public float cooldownTime = 3f;            // Hồi chiêu
    private float lastShootTime = -Mathf.Infinity;


    private PlayerRecoil playerRecoil;
    private PlayerFlip playerFlip;
    private PlayerInputHandler playerInputHandler;
    public PlayerStats playerStats;

    void Awake()
    {
        playerInputHandler = FindFirstObjectByType<PlayerInputHandler>();
        playerFlip = FindFirstObjectByType<PlayerFlip>();
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

        if (Input.GetKeyDown(KeyCode.V) && Time.time >= lastShootTime + cooldownTime)
        {
            ShootArrow();
            lastShootTime = Time.time;
        }
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

    void ShootArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, attackPoint.position, attackPoint.rotation);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = attackPoint.right * arrowSpeed;
        }
    }
}
