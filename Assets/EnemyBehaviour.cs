using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour
{
    public Transform player;
    
    public float moveStepDistance = 0.5f;
    public float minDistanceToPlayer = 4.6f;
    public float maxDistanceToPlayer = 5.2f;
    public float moveSpeed = 3f;
    public float waitAfterProbeSeconds = 5f;

    private bool isProbing = false;

    private EnemyVision enemyVision;
    private EnemyAttackVision enemyAttackVision;

    private void Awake()
    {
        enemyVision = GetComponent<EnemyVision>();
        enemyAttackVision = GetComponent<EnemyAttackVision>();
    }
    void Update()
    {
        if (!isProbing && enemyVision.CanSeePlayer == true)
        {
            StartCoroutine(ProbeMovement());
        }

        Debug.DrawLine(transform.position, player.position, Color.red);
    }

    IEnumerator ProbeMovement()
    {
        isProbing = true;

        int steps = Random.Range(2, 4); // 2–3 bước
        Debug.Log($"Enemy bắt đầu probe {steps} lần");

        for (int i = 0; i < steps; i++)
        {
            Vector2 chosenDirection = Vector2.zero;
            Vector2 targetPos = Vector2.zero;
            float futureDistance = 0f;
            bool foundValidDirection = false;

            // Thử nhiều lần để tìm hướng giữ khoảng cách phù hợp
            for (int attempt = 0; attempt < 10; attempt++)
            {
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                Vector2 tryPos = (Vector2)transform.position + randomDir * moveStepDistance;

                float tryDistance = Vector2.Distance(tryPos, player.position);
                if (tryDistance >= minDistanceToPlayer && tryDistance <= maxDistanceToPlayer)
                {
                    chosenDirection = randomDir;
                    targetPos = tryPos;
                    futureDistance = tryDistance;
                    foundValidDirection = true;
                    break;
                }
                else
                {
                    // Lưu lại để dùng nếu không tìm được cái nào phù hợp
                    chosenDirection = randomDir;
                    targetPos = tryPos;
                    futureDistance = tryDistance;
                }
            }

            Debug.Log(foundValidDirection
                ? $"[Step {i + 1}] Chọn hướng hợp lệ, cách Player: {futureDistance:F2}"
                : $"[Step {i + 1}] Không tìm được hướng hợp lệ, chấp nhận hướng cuối cùng (cách: {futureDistance:F2})");

            // Di chuyển tới targetPos
            float t = 0f;
            Vector2 startPos = transform.position;

            while (t < 1f)
            {
                t += Time.deltaTime * moveSpeed;
                transform.position = Vector2.Lerp(startPos, targetPos, t);
                yield return null;
            }

            Debug.DrawLine(startPos, targetPos, Color.green, 1.5f);
        }

        Debug.Log("Enemy hoàn thành probe. Đang đứng yên 5s...");
        yield return new WaitForSeconds(waitAfterProbeSeconds);

        isProbing = false;
    }
}
