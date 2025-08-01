using System.Collections;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    [Header("Slash VFX")]
    public GameObject[] slashVFXs;
    public LayerMask playerLayers; // Lớp của các enemy để tấn công

    [Header("Attack Settings")]
    public float attackCooldown = 1.5f;   // Thời gian hồi giữa 2 lần tấn công
    private float attackCooldownTimer = 0f;

    [Header("Dash Settings")]
    public float dashSpeed = 5f;         // Tốc độ dash
    public float dashDuration = 0.5f;    // Thời gian giữ dash
    public float lockDelay = 3f; // Thời gian chờ trước khi khóa tấn công
    public GameObject dashEffect; // Hiệu ứng khi dash
    private bool isDashing = false;
    
    public bool isLocking = false;
    public bool hasDashed = false;
    public bool isDashDone = false;

    private Rigidbody2D rb;
    private EnemyAttackVision enemyAttackVision;
    private EnemySteering enemySteering;
    private EnemyVision enemyVision;
    private PlayerRecoil playerRecoil;

    private float time = 0f;
    // ⭐ Thêm biến trạng thái:
    public bool hasAttacked { get; private set; } = false;

    void Awake()
    {
        enemyAttackVision = GetComponent<EnemyAttackVision>();
        enemyVision = GetComponent<EnemyVision>();


        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;

        // Tuỳ ý: có thể reset trạng thái đã tấn công sau khi hồi cooldown
        if (attackCooldownTimer <= 0f)
            hasAttacked = false;
    }

    public void LockIN()
    {
        if (isLocking) return;
        if (enemyVision.CanSeePlayer)
        {
            time += Time.deltaTime;
            if (time >= lockDelay)
            {
                isLocking = true;
                enemyAttackVision.isAttackLocked = true;
                LockAndDash();
            }
        }
    }

    public void LockAndDash()
    {
        if (!isLocking || hasDashed) return;

        Vector2 dashDirection = enemyAttackVision.attackPoint.right;
        rb.linearVelocity = dashDirection * dashSpeed;   // Dash bằng tốc độ

        dashEffect.SetActive(true); // Kích hoạt hiệu ứng dash
        isDashing = true;
        hasDashed = true;
        isDashDone = false;

        Invoke(nameof(StopDash), dashDuration);    // Dừng dash sau thời gian dashDuration
    }

    private void StopDash()
    {
        dashEffect.SetActive(false); // Tắt hiệu ứng dash
        rb.linearVelocity = Vector2.zero;
        enemyAttackVision.isAttackLocked = false;
        isDashing = false;
        isLocking = false;
        hasDashed = false;
        isDashing = false;
        isDashDone = true;
        time = 0f;
    }

    public void TryPerformAttack()
    {
        if (attackCooldownTimer <= 0f)
        {
            PerformAttack();
            attackCooldownTimer = attackCooldown;

            hasAttacked = true;  // ✅ Đánh dấu đã tấn công
        }
    }

    void PerformAttack()
    {
        ActivateSlashVFX();

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            enemyAttackVision.attackPoint.position,
            enemyAttackVision.attackRadius,
            enemyAttackVision.playerLayer);

        foreach (Collider2D hit in hits)
        {
            // Gọi hàm player take damage
        }

        if (hits.Length > 0)
        {
            // Recoil hoặc xử lý nếu đánh trúng player
        }
    }

    void ActivateSlashVFX()
    {
        foreach (var vfx in slashVFXs)
            vfx.SetActive(false);

        int index = Random.Range(0, slashVFXs.Length);
        var chosenVFX = slashVFXs[index];

        // Flip nếu Player ở bên trái
        if (enemyVision != null && enemyVision.targetDetected != null)
        {
            float dirToPlayer = enemyVision.targetDetected.position.x - transform.position.x;
            Vector3 scale = chosenVFX.transform.localScale;
            scale.x = Mathf.Sign(dirToPlayer) * Mathf.Abs(scale.x);  // Flip theo X
            chosenVFX.transform.localScale = scale;
        }

        chosenVFX.SetActive(true);
    }


    public bool CanAttack()
    {
        return attackCooldownTimer <= 0f;
    }

    public void ResetAttackCooldown()
    {
        attackCooldownTimer = attackCooldown;
        hasAttacked = false;  // ✅ Reset nếu muốn ép enemy chưa tấn công lại
    }
}
