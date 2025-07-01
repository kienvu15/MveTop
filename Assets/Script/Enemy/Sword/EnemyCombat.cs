using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyCombat : MonoBehaviour
{
    public float wanderInterval = 2f;
    public float minWanderDistance = 0.5f;
    public float maxWanderDistance = 2f;
    public float avoidPlayerRadius = 1f;

    private EnemyAttackVision vision;
    private EnemySteering steering;

    public Vector2 wanderTarget;
    public float wanderTimer;
    public bool hasTarget = false;

    private void Start()
    {
        vision = GetComponent<EnemyAttackVision>();
        steering = GetComponent<EnemySteering>();
    }

    private void Update()
    {
        
    }

    public void PickNewWanderTarget()
    {
        Vector2 center = transform.position;
        Vector2 playerPos = vision.playerDetected.position;

        int attempts = 10;
        for (int i = 0; i < attempts; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle.normalized * Random.Range(minWanderDistance, maxWanderDistance);
            Vector2 candidate = center + randomOffset;

            float distToPlayer = Vector2.Distance(candidate, playerPos);
            float distToCenter = Vector2.Distance(candidate, center);

            if (distToPlayer > avoidPlayerRadius && distToCenter <= vision.attackVision)
            {
                wanderTarget = candidate;
                hasTarget = true;
                wanderTimer = wanderInterval;
                return;
            }
        }

        // fallback nếu không chọn được điểm phù hợp
        wanderTarget = center;
        hasTarget = false;
    }
}
