using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBounceRun : MonoBehaviour
{
    public GameObject damageObject;
    public float chargeTime = 1f;
    public float runTime = 4f;
    public float runSpeed = 3f;
    public float customSpeed = 2.3f;
    public LayerMask obstacleMask;

    private Rigidbody2D rb;
    private Transform player;
    private Vector2 runDirection;
    private float timer;
    public bool isCharging = false;
    public bool isRunning = false;

    private EnemyVision EnemyVision;
    private EnemyAttackVision EnemyAttackVision;
    private EnemySteering EnemySteering;

    
    private float visionTimer = 0f;
    public float lockTime = 1f;
    public bool isLocked = false;
    public bool isInCooldown = false;

    public event System.Action OnRunFinished;


    private EnemyShot EnemyShot;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        EnemySteering = GetComponent<EnemySteering>();
        EnemyAttackVision = GetComponent<EnemyAttackVision>();
        EnemyVision = GetComponent<EnemyVision>();
        EnemyShot = GetComponent<EnemyShot>();
    }

    public void Update()
    {

    }

    public void HandelConditionRun()
    {
        if (EnemyVision.CanSeePlayer)
        {
            visionTimer += Time.deltaTime;

            if (visionTimer >= lockTime && !EnemyAttackVision.isAttackLocked)
            {
                // Bắt buộc cập nhật trước khi Lock
                EnemyAttackVision.PlayerInVision2AttackRange();

                if (EnemyVision.targetDetected != null)
                {
                    EnemyAttackVision.MoveAttackPointToPlayer();
                    EnemyAttackVision.isAttackLocked = true;
                    if (EnemyAttackVision.isAttackLocked == true)
                    {
                        StartBounceRun(EnemyAttackVision.playerDetected);
                    }
                }
            }
        }
        else
        {
            visionTimer = 0f;
        }
    }

    public void StartBounceRun(Transform playerTransform)
    {
        if (isCharging || isRunning) return;
        
        player = playerTransform;
        if (player == null)
        {
            Debug.LogError("Player is null");
            return;
        }

        runDirection = (EnemyAttackVision.attackPoint.position - transform.position).normalized;
        StartCoroutine(ChargeAndRun());
    }

    private System.Collections.IEnumerator ChargeAndRun()
    {
        isCharging = true;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(chargeTime);
        isCharging = false;

        isRunning = true;
        timer = 0f;

        while (timer < runTime)
        {
            rb.linearVelocity = runDirection * runSpeed;
            timer += Time.fixedDeltaTime;
            damageObject.SetActive(true);
            // Check collision
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.3f, runDirection, 0.3f, obstacleMask);
            if (hit.collider != null)
            {
                Debug.Log($"🧱 Hit: {hit.collider.name}, Normal: {hit.normal}");
                Vector2 normal = hit.normal;
                runDirection = Vector2.Reflect(runDirection, normal).normalized;

                // Optional: vẽ phản xạ
                Debug.DrawRay(transform.position, runDirection * 1.5f, Color.red, 0.5f);
            }

            yield return new WaitForFixedUpdate(); // Chờ frame vật lý tiếp theo
        }

        StopBounceRun();
        OnRunFinished?.Invoke(); Debug.Log("Run stage finished and event invoked.");
        EnemyAttackVision.isAttackLocked = false;
    }


    private void FixedUpdate()
    {
    }

    private void StopBounceRun()
    {
        isRunning = false;
        rb.linearVelocity = Vector2.zero;
        damageObject.SetActive(false);
    }

    public bool IsRunning => isRunning || isCharging;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 origin = transform.position;
        Vector3 dir = runDirection.normalized * 0.3f;

        // Vẽ hướng chạy
        Gizmos.DrawLine(origin, origin + dir);

        // Vẽ hình tròn tại vị trí hiện tại (bán kính CircleCast)
        Gizmos.DrawWireSphere(origin, 0.3f);

        // Vẽ điểm đến (nơi kết thúc CircleCast)
        Gizmos.DrawWireSphere(origin + dir, 0.3f);

    }

}
