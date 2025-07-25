using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemySteering;

[RequireComponent(typeof(EnemySteering))]
public class RangedEnemyController : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool debugDrawRays = false; // Biến để bật tắt vẽ raycast trong editor

    [Header("Dirts Setting")]
    public float driftStepTime = 0.2f;
    public float maxDriftDistance = 0.5f; // Khoảng cách drift tối đa mỗi lần
    public float minDriftDistance = 0.1f; // Khoảng cách drift tối thiểu mỗi lần
    [Space(15)]

    
    public Transform player;

    public float decisionCooldown = 2f;
    public float moveSpeed = 2f;
    public float orbitSpeed = 1f; // Tốc độ xoay quanh player (độ mỗi giây)
    public float driftDuration = 1.5f;


    public float preferredDistance = 3f; // Khoảng cách mong muốn khi di chuyển quanh player
    private Rigidbody2D rb;

    private Vector3 pinnedTargetPosition;
    private bool isOrbitingPinnedTarget = false;

    public float chaseSpeed = 2.5f;
    public float searchSpeed = 2f;

    private EnemySteering steering;
    private EnemyVision enemyVision;
    private EnemyAttackVision enemyAttackVision;

    public Vector2 ArrowDirection = Vector2.right;
    [Header("Deceptive Movement")]
    public float directionChangeInterval = 1.5f;
    public int directionChangesPerCycle = 3;

    public Vector2 playerPosition;
    //
    public event System.Action OnDritFinished;
    private Coroutine currentDriftCoroutine;

    public bool test = false; // Biến này để kiểm tra xem có đang trong trạng thái test không 

    public bool isRetreating = false;
    public bool isDirting = false;
    public bool isOrbitingPlayer = true;
    public bool moveFinished = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyVision = GetComponent<EnemyVision>();
        steering = GetComponent<EnemySteering>();
        enemyAttackVision = GetComponent<EnemyAttackVision>();
    }

    void Start()
    {
        steering.useFlip = false;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        //if (enemyVision.CanSeePlayer)
        //{
        //    ArcAroundPlayer();
        //}
    }


    public void MoveRandomDirection(float distance, float speed)
    {
        moveFinished = false;
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        StartCoroutine(MoveDistanceInDirection(randomDir, distance, speed));
    }

    public void MoveMultipleCondition()
    {
        StartCoroutine(MoveRandomDirectionMultipleTimes(2, 1.5f, 2f, 0.2f));
    }

    public IEnumerator MoveRandomDirectionMultipleTimes(int times, float distance, float speed, float delayBetween = 0.1f)
    {
        moveFinished = false;

        for (int i = 0; i < times; i++)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            yield return StartCoroutine(MoveDistanceInDirection(randomDir, distance, speed));
            yield return new WaitForSeconds(delayBetween);
        }

        moveFinished = true;
    }

    public IEnumerator MoveDistanceInDirection(Vector2 direction, float distance, float speed, float timeout = 2f)
    {
        moveFinished = false;

        direction.Normalize();
        Vector2 startPos = transform.position;
        Vector2 targetPos = startPos + direction * distance;

        float elapsedTime = 0f;

        while (Vector2.Distance(transform.position, targetPos) > 0.05f && elapsedTime < timeout)
        {
            Vector2 moveDir = (targetPos - (Vector2)transform.position).normalized;
            steering.MoveInDirection(moveDir, speed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        steering.StopMoving();
        moveFinished = true;
    }




    public void DritDec()
    {
        if (currentDriftCoroutine == null)
        {
            currentDriftCoroutine = StartCoroutine(DriftRaw(
            totalDuration: 20f,
            driftSpeed: 1f,
            directionChangeTime: 1f,
            maxOffset: 0.6f));
        }
    }

    public IEnumerator DriftRaw(
    float totalDuration,
    float driftSpeed = 1f,
    float directionChangeTime = 0.4f,
    float maxOffset = 1f)
    {
        isDirting = true;
        Vector2 center = transform.position;
        Vector2 driftDir = Random.insideUnitCircle.normalized;
        float timer = 0f;

        while (timer < totalDuration)
        {
            // Nếu đi quá xa, tự động quay về
            if (Vector2.Distance(rb.position, center) > maxOffset)
            {
                driftDir = (center - rb.position).normalized;
            }
            else
            {
                // Đổi hướng drift ngẫu nhiên mới nhưng tránh hướng về phía player
                int maxTries = 10;
                float minAngleFromPlayer = 60f; // độ lệch tối thiểu
                Vector2 toPlayer = ((Vector2)playerPosition - rb.position).normalized;

                for (int i = 0; i < maxTries; i++)
                {
                    Vector2 randomDir = Random.insideUnitCircle.normalized;
                    float angle = Vector2.Angle(randomDir, toPlayer);
                    if (angle > minAngleFromPlayer)
                    {
                        driftDir = randomDir;
                        break;
                    }
                }
            }

            float t = 0f;
            while (t < directionChangeTime && timer < totalDuration)
            {
                rb.linearVelocity = driftDir * driftSpeed;

                t += Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }
        }

        rb.linearVelocity = Vector2.zero;
        isDirting = false;
        OnDritFinished?.Invoke();
        Debug.Log("Dirt stage finished and event invoked.");
    }


    public void StopDritDec()
    {
        if (currentDriftCoroutine != null)
        {
            StopCoroutine(currentDriftCoroutine);
            currentDriftCoroutine = null;
        }
        rb.linearVelocity = Vector2.zero;
        isDirting = false;


    }



    public IEnumerator MoveZigZagGrouped(
    Transform player,
    float duration = 1.5f,
    float minAngle = 30f,
    float maxAngle = 60f,
    float switchInterval = 0.3f,
    float speed = 2f,
    float offsetDistance = 2f
    )
    {
        if (player == null) yield break;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) yield break;

        float timer = 0f;
        Vector2 baseDir = (player.position - transform.position).normalized;
        Vector2 offsetDir = (transform.position - player.position).normalized;

        while (timer < duration)
        {
            // 🔁 Random mỗi nhóm 2 lần
            bool goLeft = Random.value > 0.5f;
            float zigzagAngle = Random.Range(minAngle, maxAngle);

            // Thực hiện 2 lần liên tiếp cùng hướng & góc
            for (int i = 0; i < 2 && timer < duration; i++)
            {
                Vector2 zigDir = Quaternion.Euler(0, 0, goLeft ? -zigzagAngle : zigzagAngle) * baseDir;
                Vector2 finalDir = (zigDir + offsetDir * 0.8f).normalized;

                float t = 0f;
                while (t < switchInterval && timer < duration)
                {
                    rb.linearVelocity = finalDir * speed;
                    t += Time.deltaTime;
                    timer += Time.deltaTime;
                    yield return null;
                }
            }
        }

        rb.linearVelocity = Vector2.zero;
    }

    public IEnumerator MoveCurveSideOfPlayer(
     Transform player,
     float curveDistance = 1f,
     float speed = 1.3f,
     float arcWidth = 1.5f,
     int curveDir = 1 // 1 = phải, -1 = trái
 )
    {
        Vector2 start = transform.position;
        Vector2 toPlayer = ((Vector2)player.position - start).normalized;

        // ✅ Hướng đi cong: lệch khỏi hướng Player
        Vector2 curveDirVector = Quaternion.Euler(0, 0, 60f * curveDir) * toPlayer;

        // ✅ Điểm kết thúc cách enemy 1 đoạn lệch khỏi hướng player
        Vector2 end = start + curveDirVector.normalized * curveDistance;

        // ✅ Điểm điều khiển nằm giữa, cộng độ cong vuông góc
        Vector2 mid = (start + end) * 0.5f;
        Vector2 perp = Vector2.Perpendicular(curveDirVector.normalized);
        Vector2 controlPoint = mid + perp * arcWidth * curveDir;

        float t = 0f;
        float duration = curveDistance / speed;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            Vector2 a = Vector2.Lerp(start, controlPoint, t);
            Vector2 b = Vector2.Lerp(controlPoint, end, t);
            Vector2 point = Vector2.Lerp(a, b, t);

            Vector2 dir = (point - (Vector2)transform.position).normalized;
            rb.linearVelocity = dir * speed;

            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        StartCoroutine(DriftRaw(
            totalDuration: 13f,
            driftSpeed: 1.2f,
            directionChangeTime: 1f,
            maxOffset: 0.7f
        ));
    }

    public void ArcAroundPlayer()
    {
        if (isOrbitingPlayer == false) return;
        float orbitSpeed = 30f;
        Vector3 dir = transform.position - player.position;
        dir = Quaternion.Euler(0, 0, orbitSpeed * Time.deltaTime) * dir;
        Vector3 targetPos = player.position + dir.normalized * preferredDistance;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    public void PinTargetPosition()
    {

        if (player == null) return;

        pinnedTargetPosition = player.position;
        isOrbitingPinnedTarget = true;

    }

    public void UnpinTargetPosition()
    {
        isOrbitingPinnedTarget = false;
        StopAllCoroutines();
        Debug.Log("Unpinned target position.");
    }

    IEnumerator MoveTo(Vector2 targetPos, float stopThreshold = 0.1f, float timeout = 2f)
    {
        float elapsed = 0f;

        while (Vector2.Distance(transform.position, targetPos) > stopThreshold && elapsed < timeout)
        {
            Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
            steering.MoveInDirection(dir, moveSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        steering.StopMoving();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pinnedTargetPosition, 1f);
    }

    private IEnumerator RandomRun()
    {
        for (int i = 0; i < directionChangesPerCycle; i++)
        {
            float angle = Random.Range(0f, 360f);
            ArrowDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
            yield return new WaitForSeconds(directionChangeInterval);
        }
    }
}
