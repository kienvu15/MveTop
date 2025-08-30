using System.Collections;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    [Header("Slash VFX")]
    public GameObject[] slashVFXs;
    public GameObject dashVFX;
    public LayerMask playerLayers; // Lớp của các enemy để tấn công

    [Header("Attack Settings")]
    public float attackCooldown = 1.5f;   // Thời gian hồi giữa 2 lần tấn công
    private float attackCooldownTimer = 0f;

    [Header("Dash Attack Settings")]
    public float dashAttackCooldown = 3f;   
    private float dashAttackCooldownTimer = 0f;

    [Header("Dash Settings")]
    public float dashSpeed = 5f; 
    public float dashDuration = 0.5f; 
    public float lockDelay = 3f;
    public TrailRenderer trailRenderer; 
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

        //if(enemyAttackVision.isPlayerInAttackRange && !isDashing)
        //{
        //    DashAttack();
        //}

        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;

        if (attackCooldownTimer <= 0f)
            hasAttacked = false;

        // ✅ giảm timer dash attack
        if (dashAttackCooldownTimer > 0f)
            dashAttackCooldownTimer -= Time.deltaTime;

    }

    public void DashAttack()
    {
        if (isDashing) return; // nếu đang dash thì không dash nữa
        if (dashAttackCooldownTimer > 0f) return; // đang hồi chiêu

        enemyAttackVision.isAttackLocked = true;
        // 2. Xác định hướng dash (dựa vào attackPoint.right)
        Vector2 dashDirection = enemyAttackVision.attackPoint.right;
        // 3. Di chuyển enemy theo hướng đó
        rb.linearVelocity = dashDirection * dashSpeed;
        // 1. Bật slash VFX
        ActivateSlashVFX();

        // 4. Bật hiệu ứng dash (nếu có)
        if (trailRenderer != null)
            trailRenderer.emitting = true;

        isDashing = true;
        isDashDone = false;

        // 5. Đặt cooldown
        dashAttackCooldownTimer = dashAttackCooldown;

        // 6. Dừng dash sau dashDuration
        Invoke(nameof(StopDashAttack), dashDuration);
    }


    private void StopDashAttack()
    {
        rb.linearVelocity = Vector2.zero;
        enemyAttackVision.isAttackLocked = false;
        dashVFX.SetActive(false);
        if (trailRenderer != null)
            trailRenderer.emitting = false;

        isDashing = false;
        isDashDone = true;
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

        if (trailRenderer != null)
            trailRenderer.emitting = true;
        isDashing = true;
        hasDashed = true;
        isDashDone = false;

        Invoke(nameof(StopDash), dashDuration);    // Dừng dash sau thời gian dashDuration
    }

    private void StopDash()
    {
        if (trailRenderer != null)
            trailRenderer.emitting = false;
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
