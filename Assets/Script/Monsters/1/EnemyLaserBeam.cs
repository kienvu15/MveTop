using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyLaserBeam : MonoBehaviour
{
    private EnemyAttackVision EnemyAttackVision;
    private EnemyVision EnemyVision;

    public float visionTimer = 0f;
    public bool isShooting = false;
    public bool isLocked = false;
    public bool isInCooldown = false;
    public bool isAiming = false; 

    public float lockTime = 1f;
    public float cooldownTimer = 0f;
    public float shotCoolDown = 5f;

    [Header("Warning")]
    [SerializeField] private LineRenderer warningLine;
    [SerializeField] private float warningDuration = 1f;
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("Laser")]
    [SerializeField] public int laserDamage = 1;
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private float laserDuration = 3f;
    [SerializeField] private float laserWidth = 0.1f;
    [SerializeField] private LayerMask hitLayers; 

    private EnemySteering EnemySteering;
    private PlayerStats playerStats;
    private EnemyStats enemyStats;

    private Vector2 chargeMoveDir;
    private float preferredDistance = -1f;
    private Vector2 chargeMoveTarget = Vector2.zero;

    private Rigidbody2D rb;
    private EnemyAttackVision enemyAttackVision;
    public event System.Action OnShotLaserFinished;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        EnemyVision = GetComponent<EnemyVision>();
        EnemyAttackVision = GetComponent<EnemyAttackVision>();
        EnemySteering = GetComponent<EnemySteering>();

        playerStats = FindFirstObjectByType<PlayerStats>();
        enemyStats = GetComponent<EnemyStats>();

        EnemyVision.isSpecialVision = true;
    }

    void Update()
    {
        
    }

   
    public void HandleLockAndShoot()
    {
        if (isInCooldown || isShooting) return;

        if (EnemyVision.CanSeePlayer)
        {
            visionTimer += Time.deltaTime;
            MoveWhileCharging();
            if (visionTimer >= lockTime && !isLocked)
            {
                if (EnemyVision.targetDetected != null)
                {
                    isLocked = true;
                    // Bắt đầu theo sát Player
                    StartCoroutine(LaserWarningRoutine(rb.transform, EnemyVision.targetDetected));
                    
                }
            }
        }
        else
        {
            visionTimer = 0f;
            isLocked = false;
        }
    }

    private IEnumerator LaserWarningRoutine(Transform start, Transform target)
    {
        float timer = 0f;
        bool isVisible = true;
        preferredDistance = -1f;

        warningLine.enabled = true;

        float angle = Random.Range(0f, 360f);
        chargeMoveDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        while (timer < warningDuration)
        {
            if (isVisible)
            {
                warningLine.SetPosition(0, start.position);
                warningLine.SetPosition(1, target.position);
                warningLine.startColor = Color.red;
                warningLine.endColor = Color.red;
            }
            else
            {
                warningLine.SetPosition(0, Vector3.zero);
                warningLine.SetPosition(1, Vector3.zero); // Tắt bằng cách kéo vào nhau
            }

            MoveWhileCharging();

            isVisible = !isVisible;
            timer += blinkInterval;
            yield return new WaitForSeconds(blinkInterval);
        }

        EnemySteering?.StopMoving();
        EnemyAttackVision.isSpecial = true;
        StartCoroutine(FireLaserWithSharedLine(warningLine));

    }

    private IEnumerator FireLaserWithSharedLine(LineRenderer line)
    {
        isShooting = true;
        rb.linearVelocity = Vector2.zero;

        line.enabled = true;
        line.positionCount = 2;
        line.startWidth = laserWidth;
        line.endWidth = laserWidth;
        line.startColor = Color.red;
        line.endColor = Color.red;

        float timer = 0f;

        while (timer < laserDuration)
        {
            Vector2 firePos = EnemyAttackVision.attackPoint.position;
            Vector2 dir = EnemyAttackVision.attackPoint.right;

            RaycastHit2D hit = Physics2D.Raycast(firePos, dir, 100f, hitLayers);
            Vector2 endPos = hit.collider ? hit.point : firePos + dir * 100f;

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                playerStats.TakeDamage(enemyStats.damage, transform.position);
            }


            line.SetPosition(0, firePos);
            line.SetPosition(1, endPos);

            timer += Time.deltaTime;
            yield return null;
        }

        line.enabled = false;
        
        // 🔁 Reset lại width cho cảnh báo lần sau
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;

        isShooting = false;
        isLocked = false;
        visionTimer = 0f;

        chargeMoveTarget = Vector2.zero;

        yield return new WaitForSeconds(shotCoolDown);

        OnShotLaserFinished?.Invoke();

    }


    public void MoveWhileChargingWithoutPlayer()
    {
        if (EnemySteering == null) return;

        // Nếu chưa có target thì tìm một node ngẫu nhiên gần enemy
        if (chargeMoveTarget == Vector2.zero)
        {
            Node node = EnemySteering.gridManager.GetRandomWalkableNodeNear(transform.position, 4f);
            if (node != null)
            {
                chargeMoveTarget = node.worldPosition;
            }
        }

        // Nếu có target rồi thì di chuyển
        if (chargeMoveTarget != Vector2.zero)
        {
            Vector2 toTarget = chargeMoveTarget - (Vector2)transform.position;

            if (toTarget.magnitude < 0.1f)
            {
                // ✅ Reset để chọn lại node mới ở lần gọi tiếp theo
                chargeMoveTarget = Vector2.zero;
            }
            else
            {
                EnemySteering.MoveInDirection(toTarget.normalized, 0.8f);
            }
        }
    }



    public void MoveWhileCharging()
    {
        if (EnemySteering == null || EnemyVision == null || !EnemyVision.CanSeePlayer) return;

        Transform player = EnemyVision.targetDetected;
        if (player == null) return;

        // Nếu chưa có target → chọn node giữ LOS
        if (chargeMoveTarget == Vector2.zero)
        {
            Node node = EnemySteering.gridManager.GetAvoid2Node(
                transform.position,
                player.position,
                avoidRadius: 2f,       // enemy sẽ không đứng quá gần
                visionRadius: EnemyVision.visionRadius  // vẫn giữ trong tầm nhìn
            );

            if (node != null)
            {
                chargeMoveTarget = node.worldPosition;
            }
        }

        // Nếu có target rồi thì move
        if (chargeMoveTarget != Vector2.zero)
        {
            Vector2 toTarget = chargeMoveTarget - (Vector2)transform.position;

            // Nếu gần đến nơi rồi thì ngừng
            if (toTarget.magnitude < 0.1f)
            {
                EnemySteering.StopMoving();
            }
            else
            {
                EnemySteering.MoveInDirection(toTarget.normalized, 0.8f);
            }
        }
    }

    private void MaintainRandomDistanceToPlayer()
    {
        if (!EnemyVision.CanSeePlayer || isLocked || isInCooldown || isShooting) return;

        Transform target = EnemyVision.targetDetected;
        if (target == null || EnemySteering == null) return;

        Vector2 toPlayer = target.position - transform.position;
        float dist = toPlayer.magnitude;

        // Khởi tạo khoảng cách nếu chưa có
        if (preferredDistance < 0f)
        {
            preferredDistance = Random.Range(5.6f, 6.8f);
        }

        float diff = dist - preferredDistance;

        if (Mathf.Abs(diff) > 0.2f)
        {
            Vector2 moveDir = diff > 0f ? -toPlayer.normalized : toPlayer.normalized;
            EnemySteering.MoveInDirection(moveDir, 1.2f);
        }
        else
        {
            EnemySteering.StopMoving();
        }
    }

}
