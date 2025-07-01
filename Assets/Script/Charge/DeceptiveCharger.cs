using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class DeceptiveCharger : MonoBehaviour
{
    [Header("Vision")]
    public EnemyVision vision;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Steering Settings")]
    [Range(0f, 2f)]
    public float steeringStrength = 0.6f;

    [Header("Obstacle Avoidance")]
    public float avoidDistance = 2f;
    public LayerMask obstacleMask;

    [Header("Deceptive Movement")]
    public float directionChangeInterval = 1.5f;
    public int directionChangesPerCycle = 3;

    [Header("Charge Attack")]
    public float chargeTime = 1f;
    public float chargeSpeed = 6f;
    public float chargeCooldown = 3f;
    public float chargeDistance = 8f;

    private Rigidbody2D rb;

    private Vector2 arrowDirection = Vector2.right;
    private Vector2 finalMoveDir;
    private Vector2 chargeTarget;

    private bool isChargingUp = false;
    private bool isCharging = false;
    private bool canCharge = true;

    private bool isHitStunned = false;

    private enum ChargerState
    {
        Idle,
        RandomRun,
        ChargePrep,
        Charging,
        Cooldown
    }

    private ChargerState state = ChargerState.Idle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (vision == null)
            Debug.LogWarning("EnemyVision chưa được gán trong DeceptiveCharger!");
    }

    void Update()
    {
        // Nếu đang bị hit stun, không làm gì hết
        if (isHitStunned)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        switch (state)
        {
            case ChargerState.Idle:
                if (vision != null && vision.CanSeePlayer)
                {
                    StartCoroutine(StartRandomRun());
                }
                break;

            case ChargerState.ChargePrep:
                if (vision.CanSeePlayer && canCharge)
                {
                    StartCoroutine(StartCharge());
                }
                else
                {
                    StartCoroutine(StartRandomRun());
                }
                break;
        }

        Debug.DrawRay(transform.position, arrowDirection * 2f, Color.yellow);
    }

    void FixedUpdate()
    {
        // Nếu đang bị hit stun hoặc đang charge up thì không di chuyển bình thường
        if (isHitStunned || isChargingUp)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isCharging)
        {
            Vector2 dir = (chargeTarget - (Vector2)transform.position).normalized;
            rb.linearVelocity = dir * chargeSpeed;
            return;
        }

        if (state != ChargerState.RandomRun)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Steering và obstacle avoidance
        if (IsStuck(arrowDirection))
        {
            arrowDirection = -arrowDirection;
            finalMoveDir = arrowDirection;
        }
        else
        {
            finalMoveDir = ApplySteering(arrowDirection);
        }

        rb.linearVelocity = finalMoveDir * moveSpeed;

        // Flip sprite theo chiều di chuyển
        if (rb.linearVelocity.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(rb.linearVelocity.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    IEnumerator StartRandomRun()
    {
        state = ChargerState.RandomRun;

        for (int i = 0; i < directionChangesPerCycle; i++)
        {
            float angle = Random.Range(0f, 360f);
            arrowDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;

            yield return new WaitForSeconds(directionChangeInterval);
        }

        rb.linearVelocity = Vector2.zero;
        state = ChargerState.ChargePrep;
    }

    IEnumerator StartCharge()
    {
        state = ChargerState.Charging;
        isChargingUp = true;
        rb.linearVelocity = Vector2.zero;

        Vector2 chargeDir = ((Vector2)vision.PlayerPosition - (Vector2)transform.position).normalized;
        chargeTarget = (Vector2)transform.position + chargeDir * chargeDistance;

        Debug.Log("⚡ Chuẩn bị charge...");
        yield return new WaitForSeconds(chargeTime);

        isChargingUp = false;
        isCharging = true;
        Debug.Log("💥 Đang charge!");

        float chargeDuration = 0.5f;
        float elapsed = 0f;

        while (elapsed < chargeDuration)
        {
            Vector2 dir = (chargeTarget - (Vector2)transform.position).normalized;
            rb.linearVelocity = dir * chargeSpeed;

            elapsed += Time.deltaTime;
            yield return null;
        }

        isCharging = false;
        rb.linearVelocity = Vector2.zero;
        canCharge = false;
        Debug.Log("😤 Hết charge. Đợi cooldown...");

        StartCoroutine(CooldownTimer());
        StartCoroutine(StartRandomRun());
    }

    IEnumerator CooldownTimer()
    {
        yield return new WaitForSeconds(chargeCooldown);
        canCharge = true;
        if (!isHitStunned && vision != null && vision.CanSeePlayer && state == ChargerState.Idle)
        {
            StartCoroutine(StartRandomRun());
        }
    }

    Vector2 ApplySteering(Vector2 desiredDir)
    {
        Vector2 adjusted = desiredDir;
        float[] angles = new float[] { -60f, -30f, -15f, 15f, 30f, 60f };

        foreach (float angle in angles)
        {
            Vector2 offsetDir = Quaternion.Euler(0, 0, angle) * desiredDir;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, offsetDir, avoidDistance, obstacleMask);

            if (hit.collider != null)
            {
                float dist = hit.distance;
                float weight = Mathf.Clamp01(1f - (dist / avoidDistance));

                Vector2 pushDir = Vector2.Perpendicular(offsetDir).normalized;
                float side = Vector2.SignedAngle(desiredDir, offsetDir);
                if (side > 0) pushDir *= -1;

                adjusted += pushDir * weight * steeringStrength;
            }
        }

        return adjusted.normalized;
    }

    bool IsStuck(Vector2 desiredDir)
    {
        float[] angles = new float[] { -60f, -30f, -15f, 15f, 30f, 60f };
        int obstacleHits = 0;

        foreach (float angle in angles)
        {
            Vector2 offsetDir = Quaternion.Euler(0, 0, angle) * desiredDir;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, offsetDir, avoidDistance, obstacleMask);

            if (hit.collider != null)
                obstacleHits++;
        }

        return obstacleHits >= 3;
    }

    // Gọi khi enemy bị đánh, đứng yên 2s rồi tiếp tục
    

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)finalMoveDir);

        float[] angles = new float[] { -60f, -30f, -15f, 15f, 30f, 60f };

        foreach (float angle in angles)
        {
            Vector2 offsetDir = Quaternion.Euler(0, 0, angle) * finalMoveDir;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, offsetDir, avoidDistance, obstacleMask);

            Gizmos.color = hit.collider != null ? Color.red : Color.green;
            Gizmos.DrawRay(transform.position, offsetDir * avoidDistance);
        }
    }


    public void OnHitInterrupt()
    {
        StopAllCoroutines();
        isHitStunned = true;
        canCharge = false;
        isCharging = false;
        isChargingUp = false;

        rb.linearVelocity = Vector2.zero;

        StartCoroutine(HitStunRoutine());
    }

    IEnumerator HitStunRoutine()
    {
        state = ChargerState.Idle;

        yield return new WaitForSeconds(2f);

        isHitStunned = false;
        canCharge = true;

        if (vision != null && vision.CanSeePlayer)
        {
            StartCoroutine(StartRandomRun());
        }
    }
}
