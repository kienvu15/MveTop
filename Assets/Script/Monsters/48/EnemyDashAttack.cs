using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDashAttack : MonoBehaviour
{
    private EnemyVision EnemyVision;
    private EnemyAttackVision EnemyAttackVision;

    [Header("Dash")]
    private bool canDash = true;
    public bool isDashing = false;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private TrailRenderer trailRenderer;

    private float visionTimer = 0f;
    public float lockTime = 1f;

    private Rigidbody2D rb;
    public event System.Action OnDashFinished;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        EnemyVision = GetComponent<EnemyVision>();
        EnemyAttackVision = GetComponent<EnemyAttackVision>();
    }

    void Update()
    {
        
    }

    public void ConditionDash()
    {
        if (EnemyAttackVision.isAttackLocked == true && canDash && !isDashing)
        {
            StartCoroutine(DashCoroutine());
            canDash = false;
        }
    }

    public void Lock()
    {
        if (EnemyVision.CanSeePlayer)
        {
            visionTimer += Time.deltaTime;

            if (visionTimer >= lockTime && !EnemyAttackVision.isAttackLocked)
            {
                // Bắt buộc cập nhật trước khi Lock
                EnemyAttackVision.PlayerInAttackRange();

                if (EnemyAttackVision.playerDetected != null)
                {
                    EnemyAttackVision.MoveAttackPointToPlayer();
                    EnemyAttackVision.isAttackLocked = true;
                }
            }
        }
        else
        {
            visionTimer = 0f;
        }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;

        if (trailRenderer != null)
            trailRenderer.emitting = true;

        // Xác định hướng dash
        Vector2 dashDirection = (EnemyAttackVision.attackPoint.position - transform.position).normalized;
        

        // Áp dụng lực dash
        // Normalize để đảm bảo tốc độ dash không đổi
        rb.linearVelocity = dashDirection.normalized * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        // Kết thúc dash
        rb.linearVelocity = Vector2.zero;
        isDashing = false;
        OnDashFinished?.Invoke(); Debug.Log("Dash stage finished and event invoked.");
        EnemyAttackVision.isAttackLocked = false;
        visionTimer = 0f;

        if (trailRenderer != null)
            trailRenderer.emitting = false;

        // Bắt đầu thời gian hồi chiêu
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
