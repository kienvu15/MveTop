using System.Collections;
using UnityEngine;

public class EnemyAttackBehaviour : MonoBehaviour
{
    [Header("Attack")]
    public float attackRange = 1f;
    public float attackRadius = 0.5f;
    public float attackCooldown = 1f;
    public int damage = 1;
    [Space(5)]

    [Header("Attack Timing")]
    public float minPreAttackDelay = 0.3f;
    public float maxPreAttackDelay = 1.0f;
    public float minPostAttackWait = 0.1f;
    public float maxPostAttackWait = 0.3f;
    [Space(5)]

    [Header("Attack Point Circle")]
    public float attackPointRadius = 1.0f; // Vòng tròn mà attackPoint quay trên đó


    [Header("Condition")]
    public bool canAttack = true; // Biến để kiểm soát việc tấn công
    public bool canPerformAttack = false; // Biến để kiểm soát việc thực hiện tấn công
    public bool isAttacking = false;
    [Space(5)]

    [Header("References")]
    public LayerMask playerLayer;
    public Transform playerDetected;
    public Transform attackPoint;
    public GameObject slashVFX;
    public EnemyVision vision;

    public bool isPlayerInRange { get; private set; } = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        // Luôn xoay về phía Player nếu nhìn thấy
        if (vision != null && vision.CanSeePlayer && vision.targetDetected != null)
        {
            playerDetected = vision.targetDetected; // cập nhật playerDetected từ vision
            MoveAttackPointToPlayer();
        }

        // Kiểm tra tầm tấn công
        IsPlayerInAttackRange();
    }

    public void MoveAttackPointToPlayer()
    {
        if (playerDetected == null || attackPoint == null)
            return;

        Vector3 direction = (playerDetected.position - transform.position).normalized;

        // Tính góc và xoay AttackPoint quanh enemy theo attackPointRadius
        float angle = Mathf.Atan2(direction.y, direction.x);

        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * attackPointRadius;
        attackPoint.position = transform.position + offset;

        // Xoay AttackPoint theo hướng về Player
        float angleDeg = angle * Mathf.Rad2Deg;
        attackPoint.rotation = Quaternion.Euler(0f, 0f, angleDeg);
    }

    public void IsPlayerInAttackRange()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (hit != null)
        {
            playerDetected = hit.transform;
            isPlayerInRange = true;

            if (!isAttacking && canAttack) // ✅ Chỉ gọi nếu chưa đang tấn công
            {
                canPerformAttack = true;
                StartCoroutine(PerFormAttack());
                Debug.Log("Player detected in attack range! Starting attack.");
            }
        }
        else
        {
            playerDetected = null;
            isPlayerInRange = false;
            canPerformAttack = false;
            Debug.Log("No player detected in attack range.");
        }
    }

    public IEnumerator PerFormAttack()
    {
        if (!canAttack || !canPerformAttack) yield break;

        canAttack = false;
        canPerformAttack = false;

        // Giai đoạn chờ trước tấn công (vẫn cho di chuyển)
        float preAttackDelay = Random.Range(minPreAttackDelay, maxPreAttackDelay);
        yield return new WaitForSeconds(preAttackDelay);

        // Bắt đầu tấn công - chặn di chuyển tại đây
        isAttacking = true;

        Attack(); // Gọi tấn công

        // Thời gian "tấn công thực sự" → enemy đứng yên
        yield return new WaitForSeconds(0.1f); // Hoặc duration của animation slash

        isAttacking = false;

        // Sau khi tấn công → nghỉ
        float postAttackWait = Random.Range(minPostAttackWait, maxPostAttackWait);
        yield return new WaitForSeconds(postAttackWait);

        // Sau đó mới cho phép tấn công lại
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        canPerformAttack = true;
    }

    public void Attack()
    {
        slashVFX.SetActive(true);
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayer);
        foreach (Collider2D hit in hits)
        {
            PlayerHealth player = hit.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("Player attacked! Damage dealt: " + damage);
            }

            Knockback kb = hit.GetComponent<Knockback>();
            if (kb != null)
            {
                Debug.Log("Gọi knockback vào " + hit.name); // 👈 Kiểm tra
                kb.ApplyKnockback(transform.position);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);

        // Vẽ vòng tròn để attackPoint di chuyển trên đó
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackPointRadius);

    }

}
