using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float patrolRadius = 5f;
    public float switchDistance = 1f; // Khi đến gần node thì chọn hướng mới

    private Node currentTarget;
    public EnemySteering enemySteering; // Tham chiếu đến EnemySteering nếu cần
    private Vector2 currentDirection;

    void Start()
    {
        PickNewDirection();
        enemySteering = GetComponent<EnemySteering>();
    }

    void Update()
    {
        PatrolRuntinr();
    }

    public void PatrolRuntinr()
    {
        if (currentTarget == null)
        {
            PickNewDirection();
            return;
        }

        // Di chuyển theo hướng hiện tại
        transform.position += (Vector3)(currentDirection * moveSpeed * Time.deltaTime);

        // Khi gần node định hướng → đổi node mới
        float distanceToNode = Vector2.Distance(transform.position, currentTarget.worldPosition);
        if (distanceToNode <= switchDistance)
        {
            PickNewDirection();
        }
    }
    void PickNewDirection()
    {
        currentTarget = enemySteering.gridManager.GetRandomWalkableNodeNear(transform.position, patrolRadius);

        if (currentTarget != null)
        {
            Vector2 dir = (currentTarget.worldPosition - (Vector2)transform.position).normalized;
            currentDirection = dir;
        }
        else
        {
            currentDirection = Vector2.zero;
        }
    }
    public void ResetPatrolTarget()
    {
        currentTarget = null;
        currentDirection = Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;

            // Vẽ line theo hướng đang di chuyển, không nối đến node
            Vector3 from = transform.position;
            Vector3 to = transform.position + (Vector3)(currentDirection.normalized * 1.5f);
            Gizmos.DrawLine(from, to);

            // Optionally: vẽ node định hướng như chấm nhỏ (debug)
            if (currentTarget != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(currentTarget.worldPosition, 0.1f);
            }
        }

        // Vẽ hình cầu xung quanh vị trí của enemy
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }

}