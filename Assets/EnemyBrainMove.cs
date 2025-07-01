using UnityEngine;
using Pathfinding;

public class EnemyBrainMove : MonoBehaviour
{
    public Transform target; // The target the enemy will move towards
    public float speed = 2f; // Speed of the enemy movement
    public float nextWaypointDistance = 0.5f; // Distance to the next waypoint to consider it reached

    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    private Seeker seeker;
    private Rigidbody2D rb;
    private EnemyVision enemyVision;
    public Transform EnemyVFX;

    private bool isChasingPlayer = false;
    private bool goingToLastSeen = false;
    private Vector3? currentLastSeenPosition = null;

    private FormationPoints formationPoints; // tham chiếu đến script chứa các điểm formation
    private Transform formationTarget;       // điểm formation hiện tại mà enemy chọn đuổi theo

    private EnemyAttackBehaviour enemyAttackBehaviour; // Tham chiếu đến EnemyAttackBehaviour nếu cần

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        enemyVision = GetComponent<EnemyVision>();
        formationPoints = FindFirstObjectByType<FormationPoints>();
        enemyAttackBehaviour = GetComponent<EnemyAttackBehaviour>();

        InvokeRepeating("UpdatePath", 0f, 0.1f);
    }

    void UpdatePath()
    {
        if (!seeker.IsDone() || enemyVision == null) return;

        if (enemyVision.CanSeePlayer)
        {
            isChasingPlayer = true;
            goingToLastSeen = false;

            // 👇 Chọn 1 formation point gần nhất làm mục tiêu
            if (formationPoints != null)
            {
                formationTarget = formationPoints.GetClosestAvailablePoint(transform.position);
                if (formationTarget != null)
                    seeker.StartPath(rb.position, formationTarget.position, OnPathComplete);
            }
        }
        else if (isChasingPlayer && enemyVision.lastSeenPosition.HasValue)
        {
            // Mất dấu player → đi đến last seen position
            isChasingPlayer = false;
            goingToLastSeen = true;
            currentLastSeenPosition = enemyVision.lastSeenPosition;

            seeker.StartPath(rb.position, currentLastSeenPosition.Value, OnPathComplete);
        }
    }


    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    {

        if (path == null || currentWaypoint >= path.vectorPath.Count)
        {
            rb.linearVelocity = Vector2.zero;
            reachedEndOfPath = true;
            return;
        }

        if(enemyAttackBehaviour.isAttacking == true)
        {
            rb.linearVelocity = Vector2.zero; // Dừng di chuyển khi đang tấn công
            return;
        }

        reachedEndOfPath = false;

        // Tính hướng và áp dụng velocity
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        rb.linearVelocity = direction * speed;

        // Kiểm tra nếu gần waypoint thì chuyển sang waypoint kế
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;

            if (goingToLastSeen && currentWaypoint >= path.vectorPath.Count)
            {
                goingToLastSeen = false;
                currentLastSeenPosition = null;
                rb.linearVelocity = Vector2.zero;
                // TODO: Có thể chuyển sang trạng thái "Search"
            }
        }

        // Xoay mặt
        if (currentLastSeenPosition.HasValue)
        {
            float dirToPlayer = currentLastSeenPosition.Value.x - transform.position.x;
            if (dirToPlayer > 0.01f)
                EnemyVFX.localScale = new Vector3(1, 1, 1);
            else if (dirToPlayer < -0.01f)
                EnemyVFX.localScale = new Vector3(-1, 1, 1);
        }
    }
}
