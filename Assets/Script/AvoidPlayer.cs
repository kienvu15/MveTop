using System;
using System.Collections.Generic;
using UnityEngine;

public class AvoidPlayer : MonoBehaviour
{
    [Header("Combat")]
    public float avoidRadius = 3f;
    public float shotRadius = 6f;
    public float moveAwayDistance = 4f;

    [Header("Cooldowns")]
    public float cooldownBeforeNextShot = 2f; // Cooldown sau mỗi lần bắn (retreat bình thường)
    public float cooldownAfterDodge = 5f;     // Cooldown sau khi né gấp

    [Header("Shooting")]
    public GameObject arrowPrefab;
    public Transform shootPoint;
    public float arrowSpeed = 10f;


    [Header("References")]
    public Transform player;
    private EnemySteering enemySteering;
    private EnemyVision enemyVision;

    public float nextShootTime = 0f;
    public float waitAtRetreatUntil = 0f;

    public Node retreatNode = null;
    public bool isRetreating = false;
    public bool isDodging = false;
    public bool waitingToShoot = false;
    public bool stateChanged = false;
    public float nodeStopDistance = 0.3f;
    public bool hasShot = false;
    void Start()
    {
        enemySteering = GetComponent<EnemySteering>();
        enemyVision = GetComponent<EnemyVision>();
        player = FindFirstObjectByType<PlayerStats>().transform;
    }

    void Update()
    {
        if (hasShot && Time.time >= nextShootTime)
        {
            hasShot = false;
        }
    }

    public void DodgeStage()
    {
        if (isDodging || isRetreating)
        {
            if (retreatNode != null)
            {
                if (stateChanged == true) return;
                Vector2 dir = (retreatNode.worldPosition - (Vector2)transform.position).normalized;
                enemySteering.MoveInDirection(dir);

                float dist = Vector2.Distance(transform.position, retreatNode.worldPosition);
                if (dist < nodeStopDistance)
                {
                    if (!waitingToShoot)
                    {
                        waitingToShoot = true;
                        waitAtRetreatUntil = Time.time + cooldownBeforeNextShot;
                    }

                    if (Time.time >= waitAtRetreatUntil)
                    {
                        if (enemyVision.CanSeePlayer &&
                            GridManager.Instance.HasLineOfSight(transform.position, player.position))
                        {
                            TryShoot();
                        }
                        else if (!enemyVision.CanSeePlayer && enemyVision.lastSeenPosition.HasValue)
                        {
                            // Ngưng MoveInDirection nếu đang né
                            isDodging = false;
                            isRetreating = false;
                            Vector2 lastSeen = enemyVision.lastSeenPosition.Value;
                            enemySteering.MoveTo(lastSeen, 2f);
                            if (dist < nodeStopDistance)
                            {
                                // Tới nơi thì dừng hoặc chuyển trạng thái
                                isRetreating = false;
                            }
                            return;
                        }
                    }

                }
            }
            return;
        }
    }

    public void OnE()
    {
        if (enemyVision.CanSeePlayer && enemyVision.targetDetected != null)
        {
            float distToPlayer = Vector2.Distance(transform.position, enemyVision.targetDetected.position);
            if (distToPlayer <= shotRadius &&
                GridManager.Instance.HasLineOfSight(transform.position, player.position))
            {
                TryShoot();
            }
            else
            {
                MoveTowardPlayer();
            }
        }
    }



    public void TryShoot()
    {
        if (Time.time < nextShootTime) return;

        Debug.DrawLine(transform.position, player.position, Color.red);
        ShootArrow();

        nextShootTime = Time.time + cooldownBeforeNextShot;
        isDodging = false;
        isRetreating = true;
        waitingToShoot = false;

        retreatNode = GridManager.Instance.GetAvoidNode(transform.position, player.position, avoidRadius, shotRadius);
        if (retreatNode == null)
        {
            ChooseCurvedRetreatDirection();
        }
    }

    public void ShootArrow()
    {
        if (arrowPrefab == null || shootPoint == null)
        {
            Debug.LogWarning("arrowPrefab hoặc shootPoint chưa được gán!");
            return;
        }

        // Instantiate mũi tên
        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
        // Tính hướng bắn
        Vector2 direction = (player.position - shootPoint.position).normalized;
        // Quay đầu mũi tên về hướng player
        arrow.transform.right = direction;
        // Bắn đi
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * arrowSpeed;
        }
        hasShot = true;
        Debug.Log("Shoot!");
    }

    

    public void ChooseCurvedRetreatDirection()
    {
        Vector2 fromPlayer = (Vector2)transform.position - (Vector2)player.position;
        Vector2 baseDir = fromPlayer.normalized;

        Vector2 perp = Vector2.Perpendicular(baseDir);
        float side = UnityEngine.Random.value < 0.5f ? 1f : -1f;
        perp *= side;

        float angleOffset = UnityEngine.Random.Range(-30f, 30f);
        Vector2 offsetDir = Quaternion.Euler(0, 0, angleOffset) * baseDir;
        Vector2 finalDir = (offsetDir + perp * 0.7f).normalized;

        Vector2 candidatePos = (Vector2)transform.position + finalDir * moveAwayDistance;
        Node bestNode = FindBestRetreatNode(candidatePos);

        if (bestNode != null)
        {
            retreatNode = bestNode;
        }
    }

    Node FindBestRetreatNode(Vector2 targetPos)
    {
        float minDist = 1.5f;
        float maxDist = shotRadius;

        Node best = null;
        float bestScore = float.MinValue;

        foreach (var node in GridManager.Instance.grid.Values)
        {
            if (!node.isWalkable) continue;

            float distToPlayer = Vector2.Distance(player.position, node.worldPosition);
            if (distToPlayer < minDist || distToPlayer > maxDist) continue;
            if (!GridManager.Instance.HasLineOfSight(node.worldPosition, player.position)) continue;

            float distToTarget = Vector2.Distance(targetPos, node.worldPosition);
            float score = -distToTarget + UnityEngine.Random.Range(-0.5f, 0.5f);

            if (score > bestScore)
            {
                best = node;
                bestScore = score;
            }
        }

        return best;
    }

    public void MoveTowardPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        enemySteering.MoveInDirection(dir);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (retreatNode != null)
        {
            Gizmos.DrawLine(transform.position, retreatNode.worldPosition);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, avoidRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, shotRadius);
    }

}
