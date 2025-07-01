using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemySteering : MonoBehaviour
{
    [Header("Vision")]
    public EnemyVision vision;
    public float stopDistanceToLastSeen = 0.2f;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float smoothing = 5f;

    [Header("Steering Settings")]
    [Range(0f, 2f)]
    public float steeringStrength = 0.6f;

    [Header("Obstacle Avoidance (Smart Steering)")]
    public float avoidRadius = 2f;
    public float agentColliderSize = 0.6f;
    public LayerMask obstacleMask;

    [Header("Debug")]
    public bool debugDrawRays = true;

    private Rigidbody2D rb;
    private Vector2 linearVelocitySmoothing;
    private Vector2 targetDir;
    private Vector2 desiredlinearVelocity;
    public Vector2 finalMoveDir;

    private bool isAvoidingHardObstacle = false;
    private float hardAvoidTimer = 0f;
    private Vector2 hardAvoidDir;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void MoveTo(Vector2 targetPosition, float customSpeed)
    {
        targetDir = (targetPosition - (Vector2)transform.position).normalized;
        finalMoveDir = AdjustDirection(targetDir);

        desiredlinearVelocity = finalMoveDir * customSpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredlinearVelocity, Time.deltaTime * smoothing);

        FlipSprite(rb.linearVelocity.x);
    }

    public void Move(Vector2 dir)
    {
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, dir.normalized * moveSpeed, Time.deltaTime * smoothing);
        FlipSprite(dir.x);
    }

    public void MoveInDirection(Vector2 direction)
    {
        finalMoveDir = AdjustDirection(direction.normalized);
        desiredlinearVelocity = finalMoveDir * moveSpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredlinearVelocity, Time.deltaTime * smoothing);

        FlipSprite(rb.linearVelocity.x);
    }

    public void MoveInDirection(Vector2 direction, float customSpeed)
    {
        finalMoveDir = AdjustDirection(direction.normalized);
        desiredlinearVelocity = finalMoveDir * customSpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredlinearVelocity, Time.deltaTime * smoothing);

        FlipSprite(rb.linearVelocity.x);
    }

    Vector2 AdjustDirection(Vector2 desiredDir)
    {
        Vector2 totalBlockedDir = Vector2.zero;
        int rayCount = 9;
        float maxAngle = 90f;
        float shortRayThreshold = 0.4f;

        int shortRayHits = 0;
        int totalHits = 0;

        for (int i = 0; i < rayCount; i++)
        {
            float t = i / (float)(rayCount - 1);
            float angle = Mathf.Lerp(-maxAngle, maxAngle, t);
            Vector2 rayDir = Quaternion.Euler(0, 0, angle) * desiredDir;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, avoidRadius, obstacleMask);

            if (hit.collider != null)
            {
                float dist = Mathf.Max(0.01f, hit.distance);
                if (dist < shortRayThreshold) shortRayHits++;
                totalHits++;

                float strength = Mathf.Pow(1f - (dist / avoidRadius), 2.5f);
                totalBlockedDir += rayDir * strength;

                if (debugDrawRays)
                    Debug.DrawRay(transform.position, rayDir * hit.distance, Color.red);
            }
            else
            {
                if (debugDrawRays)
                    Debug.DrawRay(transform.position, rayDir * avoidRadius, Color.green);
            }
        }

        // Nếu quá nhiều tia bị cản → Hard Avoid
        if (!isAvoidingHardObstacle && (totalHits >= rayCount * 0.8f || shortRayHits >= rayCount * 0.5f))
        {
            isAvoidingHardObstacle = true;
            hardAvoidTimer = 0.6f;

            // Tính 2 hướng vuông góc
            Vector2 perp1 = Vector2.Perpendicular(desiredDir).normalized;
            Vector2 perp2 = -perp1;

            // Chọn hướng vuông góc nào gần với target nhất (góc nhỏ hơn)
            float angle1 = Vector2.Angle(perp1, targetDir);
            float angle2 = Vector2.Angle(perp2, targetDir);
            hardAvoidDir = angle1 < angle2 ? perp1 : perp2;
        }

        // Nếu đang ở trong trạng thái né cứng
        if (isAvoidingHardObstacle)
        {
            hardAvoidTimer -= Time.deltaTime;
            if (hardAvoidTimer <= 0f)
                isAvoidingHardObstacle = false;

            return hardAvoidDir.normalized;
        }

        // Nếu có lực cản → né
        if (totalBlockedDir.magnitude > 0.01f)
        {
            Vector2 pushAwayDir = (desiredDir - totalBlockedDir.normalized).normalized;
            Vector2 bentDir = (pushAwayDir * 1.0f + desiredDir * 0.5f).normalized;
            return bentDir;
        }

        return desiredDir.normalized;
    }


    public void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
    }

    private void FlipSprite(float xlinearVelocity)
    {
        if (Mathf.Abs(xlinearVelocity) > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(xlinearVelocity) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    private void Update()
    {
        if (rb.linearVelocity.magnitude < 0.05f && desiredlinearVelocity.magnitude > 0.5f)
        {
            Debug.LogWarning("Enemy might be stuck!");
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !debugDrawRays) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, finalMoveDir * 1.5f);

        for (int i = 0; i < 8; i++)
        {
            Vector2 dir = Directions02.Eight[i];
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, avoidRadius, obstacleMask);
            float length = avoidRadius;
            Gizmos.color = hit.collider != null ? Color.red : Color.green;
            if (hit.collider != null) length = hit.distance;
            Gizmos.DrawRay(transform.position, dir * length);
        }
    }
}

