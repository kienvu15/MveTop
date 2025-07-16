using UnityEngine;

public class EnemyAttackVision : MonoBehaviour
{
    [HideInInspector] public EnemyVision EnemyVision;

    [Header("Debug Settings")]
    public bool debugDrawRays = false;

    [Header("Attack Vision Settings")]
    public float attackVision = 1f;
    public Transform attackPoint;
    public float attackRange = 0.5f; // Khoảng cách tấn công
    public float attackRadius = 0.5f;

    public float rotationSpeed = 2f; // Tốc độ xoay
    public float moveSpeed = 2f;     // Tốc độ di chuyển của AttackPoint

    public LayerMask playerLayer;
    public Transform playerDetected;

    public bool isPlayerInAttackRange  = false;
    public bool isAttackLocked = false;
    public bool isSpecial = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnemyVision = GetComponent<EnemyVision>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInAttackRange();

        if (!isAttackLocked && isSpecial == false)
        {
            MoveAttackPointToPlayer(); // chỉ cập nhật hướng nếu chưa khóa
        }
        else if(!isAttackLocked && isSpecial == true)
        {
            MoveAttackPointToPlayerLerp(); //Lerp cho bắn
        }
    }

    public void PlayerInAttackRange()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackVision, playerLayer);
        if (hit != null)
        {
            playerDetected = hit.transform;
            isPlayerInAttackRange = true;
            Debug.Log("player in attack range");
        }
        else
        {
            isPlayerInAttackRange = false;
        }
    }
    public void PlayerInVision2AttackRange()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, EnemyVision.visionRadius, playerLayer);
        if (hit != null)
        {
            playerDetected = hit.transform;
            isPlayerInAttackRange = true;
            Debug.Log("player in attack range");
        }
        else
        {
            isPlayerInAttackRange = false;
        }
    }
    public void MoveAttackPointToPlayer()
    {
        if (EnemyVision.targetDetected == null || attackPoint == null)
            return;

        Vector3 direction = (EnemyVision.targetDetected.position - transform.position).normalized;

        // Tính góc và xoay AttackPoint quanh enemy theo attackPointRadius
        float angle = Mathf.Atan2(direction.y, direction.x);

        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * attackRange;
        attackPoint.position = transform.position + offset;

        // Xoay AttackPoint theo hướng về Player
        float angleDeg = angle * Mathf.Rad2Deg;
        attackPoint.rotation = Quaternion.Euler(0f, 0f, angleDeg);
    }

    public void MoveAttackPointToPlayerLerp()
    {
        if (EnemyVision.targetDetected == null || attackPoint == null)
            return;

        // Vector từ enemy đến target
        Vector3 direction = (EnemyVision.targetDetected.position - transform.position).normalized;

        // Góc hiện tại
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Quay từ từ về hướng player (mượt)
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        attackPoint.rotation = Quaternion.Lerp(attackPoint.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Di chuyển position theo hướng player nhưng mượt
        Vector3 desiredOffset = direction * attackRange;
        Vector3 desiredPosition = transform.position + desiredOffset;

        attackPoint.position = Vector3.Lerp(attackPoint.position, desiredPosition, moveSpeed * Time.deltaTime);
    }

    public void OnDrawGizmos()
    {
        if (debugDrawRays == false) return;   
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackVision);
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);

       
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
