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
    [SerializeField] public float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private TrailRenderer trailRenderer;
    public GameObject dashDealDame;

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
        if(isDashing == false)
        {
            dashSpeed = Random.Range(15f, 22f);
        }

        
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

        Vector2 dashDirection = (EnemyAttackVision.attackPoint.position - transform.position).normalized;
        rb.linearVelocity = dashDirection.normalized * dashSpeed;
        dashDealDame.SetActive(true);

        yield return new WaitForSeconds(dashDuration);

        // ✅ Nếu object bị destroy trong lúc chờ
        if (this == null || gameObject == null) yield break;

        dashDealDame.SetActive(false);
        rb.linearVelocity = Vector2.zero;
        isDashing = false;
        OnDashFinished?.Invoke();
        EnemyAttackVision.isAttackLocked = false;
        visionTimer = 0f;

        if (trailRenderer != null)
            trailRenderer.emitting = false;

        yield return new WaitForSeconds(dashCooldown);

        // ✅ Check tiếp 1 lần nữa sau cooldown
        if (this == null || gameObject == null) yield break;

        canDash = true;
    }


    private void OnDestroy()
    {
        if (trailRenderer != null)
            trailRenderer.emitting = false;

        StopAllCoroutines();
    }
}
