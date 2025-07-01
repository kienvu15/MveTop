using UnityEngine;

public class EnemyCombatMovement : MonoBehaviour
{
    public float minDistance = 1.5f;
    public float maxDistance = 3f;
    public float moveSpeed = 2f;
    public float retreatSpeedMultiplier = 0.6f;
    public float directionSwitchTime = 3f; // Thời gian đổi hướng orbit

    private Transform player;
    private Rigidbody2D rb;
    private EnemyVision vision;
    private EnemyAttackBehaviour attackBehaviour;
    private EnemyBrainMove brainMove;


    public float minTime = 2f;
    public float maxTime = 4f;

    private int orbitDirection = 1; // 1 = trái, -1 = phải
    private float directionTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        vision = GetComponent<EnemyVision>();
        attackBehaviour = GetComponent<EnemyAttackBehaviour>();
        brainMove = GetComponent<EnemyBrainMove>();
        directionTimer = directionSwitchTime;
    }

    void Update()
    {
        player = vision.targetDetected;

        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            // Tắt truy đuổi nếu gần
            if (distance < maxDistance && brainMove.enabled)
            {
                brainMove.enabled = false;
            }
            else if (distance > maxDistance + 1f && !brainMove.enabled)
            {
                brainMove.enabled = true;
            }

            // Đếm ngược thời gian để đổi hướng
            directionTimer -= Time.deltaTime;
            if (directionTimer <= 0f)
            {
                orbitDirection *= -1; // Đổi hướng
                directionTimer = Random.Range(minTime, maxTime); // Reset với thời gian ngẫu nhiên
            }
        }
        else
        {
            if (!brainMove.enabled)
                brainMove.enabled = true;

            directionTimer = directionSwitchTime;
        }
    }

    void FixedUpdate()
    {
        if (player == null || attackBehaviour.isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 toPlayer = (player.position - transform.position).normalized;

        if (distance < minDistance)
        {
            rb.linearVelocity = -toPlayer * moveSpeed * retreatSpeedMultiplier; // Lùi ra
        }
        else if (distance > maxDistance)
        {
            rb.linearVelocity = toPlayer * moveSpeed; // Tiến vào
        }
        else
        {
            // Vòng quanh player với hướng xoay
            Vector2 orbitDir = Vector2.Perpendicular(toPlayer) * orbitDirection;
            rb.linearVelocity = orbitDir.normalized * moveSpeed * 0.7f;
        }
    }
}
