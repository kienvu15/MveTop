using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemySteering : MonoBehaviour
{
    [Header("Vision")]
    public EnemyVision vision;
    public float stopDistanceToLastSeen = 0.2f;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float smoothing = 5f;
    public bool useFlip = true; // Biến để bật tắt việc lật sprite
    public CurveMode chosenCurveMode;
    public bool hasChosenCurve = false;

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

    public Coroutine currentWobbleRoutineCoroutine;

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

        if (useFlip)
        {
            FlipSprite(rb.linearVelocity.x);
        }
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

        if (useFlip)
        {
            FlipSprite(rb.linearVelocity.x);
        }

    }

    public void MoveInDirection(Vector2 direction, float customSpeed)
    {
        finalMoveDir = AdjustDirection(direction.normalized);
        desiredlinearVelocity = finalMoveDir * customSpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredlinearVelocity, Time.deltaTime * smoothing);

        if (useFlip)
        {
            FlipSprite(rb.linearVelocity.x);
        }
    }

    public float loopbackOffset = 0f;

    public void SetRandomLoopbackOffset()
    {
        loopbackOffset = UnityEngine.Random.Range(-1f, 1f);
    }

    public enum CurveMode { Left, Right, LoopBack }

    public void MoveToWithBendSmart(Vector2 targetPosition, CurveMode mode, float customSpeed)
    {
        Vector2 currentPos = transform.position;
        Vector2 dirToTarget = (targetPosition - currentPos).normalized;

        // 1. Tính hướng bẻ cong ban đầu
        Vector2 bendOffset = mode switch
        {
            CurveMode.Left => Vector2.Perpendicular(dirToTarget),
            CurveMode.Right => -Vector2.Perpendicular(dirToTarget),
            CurveMode.LoopBack => -dirToTarget + Vector2.Perpendicular(dirToTarget) * loopbackOffset,

            _ => Vector2.zero
        };

        Vector2 desiredDir = (dirToTarget + bendOffset * 0.8f).normalized;

        // 2. Kiểm tra va chạm: sau lưng & hướng cong
        Vector2 backDir = -dirToTarget;
        Vector2 sideDir = bendOffset.normalized;

        bool blockedBack = Physics2D.Raycast(currentPos, backDir, 1.5f, obstacleMask);
        bool blockedSide = Physics2D.Raycast(currentPos, sideDir, 1.5f, obstacleMask);

        if (blockedBack && blockedSide)
        {
            desiredDir = dirToTarget;
        }
        else if (blockedSide)
        {
            desiredDir = (dirToTarget + bendOffset * 0.3f).normalized;
        }
        else if (blockedBack)
        {
            desiredDir = (dirToTarget + bendOffset * 0.5f).normalized;
        }

        // 3. Kiểm tra node ở hướng dự kiến có hợp lệ không (GridManager)
        Vector2 projectedPos = currentPos + desiredDir * 1.5f;
        Node node;
        if (GridManager.Instance != null &&
            GridManager.Instance.grid.TryGetValue(Vector2Int.RoundToInt(projectedPos), out node))
        {
            if (!node.isWalkable)
            {
                // Nếu node phía trước không walkable → chỉnh lại hướng, giảm độ cong
                desiredDir = (dirToTarget + bendOffset * 0.3f).normalized;
            }
        }

        // 4. Steering + Movement
        finalMoveDir = AdjustDirection(desiredDir);
        desiredlinearVelocity = finalMoveDir * customSpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredlinearVelocity, Time.deltaTime * smoothing);

        if (useFlip)
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

    public Vector2 lastStoppedPosition { get; private set; }

    public void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
        lastStoppedPosition = transform.position;
    }

    private Coroutine wobbleRoutine;
    private bool isWobbling = false;
    public bool IsWobbling => isWobbling;

    public void StartWobbleInPlace(float duration = 1.5f, float radius = 0.5f, float speed = 1.5f)
    {
        if (wobbleRoutine != null) StopCoroutine(wobbleRoutine);
        wobbleRoutine = StartCoroutine(WobbleRoutine(duration, radius, speed));
    }

    public void StopWobble()
    {
        if (wobbleRoutine != null)
        {
            StopCoroutine(wobbleRoutine);
            wobbleRoutine = null;
        }
        StopMoving();
    }

    private IEnumerator WobbleRoutine(float duration, float radius, float speed)
    {
        isWobbling = true;
        float timer = 0f;

        while (timer < duration)
        {
            Vector2 offset = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(0.1f, radius);
            Vector2 target = (Vector2)transform.position + offset;
            float t = 0f;

            while (t < 0.25f && timer < duration)
            {
                MoveTo(target, speed);
                t += Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }
        }

        StopMoving();
        isWobbling = false;
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(lastStoppedPosition, 0.5f);

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