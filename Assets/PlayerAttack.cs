using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackRange = 1f;
    public float attackRadius = 0.5f;
    public LayerMask enemyLayers;
    public int damage = 2;
    [SerializeField] private float recoilForce = 5f;

    [Header("ComboAttack")]
    public bool isDashingAttack = false;
    public int comboStep = 0;
    [SerializeField] public float dashOnLastHitSpeed = 2f;
    [SerializeField] public float FinaledashOnLastHitSpeed = 10f;
    [SerializeField] public float dashDuration = 0.2f;

    [Header("Slash VFX")]
    public GameObject[] slashVFXs;
    private int lastSlashIndex = -1;
    [Space(5)]

    //
    private PlayerInputHandler playerInputHandler;
    private PlayerStateController PlayerStateController;
    private PlayerFlip playerFlip;
    private EnemyStats enemyStats;
    private Rigidbody2D rb;

    void Awake()
    {
        playerInputHandler = FindFirstObjectByType<PlayerInputHandler>();
        PlayerStateController = FindFirstObjectByType<PlayerStateController>();
        playerFlip = FindFirstObjectByType<PlayerFlip>();
        enemyStats = FindFirstObjectByType<EnemyStats>();
        rb = GetComponent<Rigidbody2D>();
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

        if (playerInputHandler.AttackPressed == true)
        {
            TryAttack();
        }

    }

    public void SetAttackPointDirection(Vector2 direction)
    {
        if (attackPoint == null) return;

        direction.Normalize();
        attackPoint.localPosition = direction * attackRange;

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
        attackPoint.localPosition = direction * attackRange;

        // ✅ Xoay attackPoint về phía enemy
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        attackPoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void TryAttack()
    {
        if (isDashingAttack) return;

        comboStep++;
        if (comboStep > 3)
            comboStep = 0;

        Attack(comboStep);
    }

    void Attack(int step)
    {
        foreach (var vfx in slashVFXs)
            vfx.SetActive(false);

        int newIndex;
        do
        {
            newIndex = Random.Range(0, slashVFXs.Length);
        } while (newIndex == lastSlashIndex);

        lastSlashIndex = newIndex;
        slashVFXs[newIndex].SetActive(true);

        StartCoroutine(AttackDashForward(dashOnLastHitSpeed));

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayers);

        foreach (Collider2D hit in hits)
        {
            Debug.Log("Hit: " + hit.name);
            enemyStats.TakeDamage(damage);
        }

        if(hits.Length > 0)
        {
            ApplyRecoil();
        }

        if(step == 3)
        {
            StartCoroutine(AttackDashForward(FinaledashOnLastHitSpeed));
        }
    }

    private IEnumerator AttackDashForward(float dashOnLastHitSpeed)
    {
        isDashingAttack = true;
        PlayerStateController.isDashing = true;
        Vector2 dashDir = (attackPoint.position - transform.position).normalized;

        float timer = 0f;
        while (timer < dashDuration)
        {
            rb.linearVelocity = dashDir * dashOnLastHitSpeed;
            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        isDashingAttack = false;
        PlayerStateController.isDashing = false;
    }

    private void ApplyRecoil()
    {
        // Recoil theo hướng ngược với attackPoint
        Vector2 recoilDir = (transform.position - attackPoint.position).normalized;
        Debug.DrawRay(transform.position, recoilDir * 2f, Color.green, 1f);
        StartCoroutine(RecoilRoutine());
        rb.AddForce(recoilDir * recoilForce, ForceMode2D.Impulse);
    }
    
    private IEnumerator RecoilRoutine()
    {
        PlayerStateController.isRecoiling = true;
        yield return new WaitForSeconds(0.15f); // tùy thời gian bạn muốn
        PlayerStateController.isRecoiling = false;
    }

    void OnDrawGizmos()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }

}
