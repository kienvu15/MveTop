using UnityEngine.LowLevel;
using UnityEngine;

public class EnemyStealthSimple : MonoBehaviour
{
    public Transform player;

    public PlayerFlip playerFlip;

    public float moveSpeed = 2f;
    public float behindDistance = 1.5f;

    private Vector2 targetPos;
    private float waitTimer = 0f;
    private float waitDuration = 1.5f;
    private bool isWaiting = false;
    private bool isFlanking = false;

    void Update()
    {
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitDuration)
            {
                // Kết thúc đợi, chuyển sang flank
                isWaiting = false;
                isFlanking = true;

                int sign = Random.value > 0.5f ? 1 : -1;
                Vector2 flankOffset = Vector2.up * sign * 1.5f;
                Vector2 backDir = playerFlip.lookDirection == LookDirection.Right ? Vector2.left : Vector2.right;
                targetPos = (Vector2)player.position + backDir * behindDistance + flankOffset;
            }
        }
        else if (isFlanking)
        {
            if (!IsInFrontOfPlayer())
            {
                // Nếu đã vòng ra sau được thì tiếp cận
                isFlanking = false;
            }
            // targetPos giữ nguyên (vị trí flank)
        }
        else
        {
            // Hành vi bình thường: tiếp cận sau lưng
            if (IsInFrontOfPlayer())
            {
                // Nếu bị nhìn thấy → bắt đầu đợi
                isWaiting = true;
                waitTimer = 0f;
                return;
            }

            // Nếu không bị nhìn thì tìm điểm sau lưng
            Vector2 backDir = playerFlip.lookDirection == LookDirection.Right ? Vector2.left : Vector2.right;
            targetPos = (Vector2)player.position + backDir * behindDistance;
        }

        // Di chuyển tới targetPos
        MoveToTarget();
    }

    bool IsInFrontOfPlayer()
    {
        Vector2 toEnemy = (transform.position - player.position).normalized;
        bool facingRight = playerFlip.lookDirection == LookDirection.Right;

        return (facingRight && toEnemy.x > 0f) || (!facingRight && toEnemy.x < 0f);
    }


    void MoveToTarget()
    {
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
    }
}
