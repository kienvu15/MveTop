using UnityEngine;

public class EnemyVision : MonoBehaviour
{

    public bool debugDrawRays = false; // Biến để bật tắt vẽ raycast trong editor

    public float visionRadius = 5f;
    public Transform targetDetected;

    public float attackrange = 1f;

    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    
    public bool isSpecialVision = false;
    public bool CanSeePlayer { get; private set; } = false;
    public bool hasSeenPlayer { get; private set; } = false; // 👈 Biến này để kiểm tra xem đã thấy player chưa, có thể dùng trong các state khác
    public Vector3? lastSeenPosition { get; private set; } = null; // 👈 Vị trí cuối cùng thấy player

    void Update()
    {
        if(isSpecialVision == true && hasSeenPlayer == true)
        {
            Vision2();
        }
        else
        {
            Vision();
        }
    }

    public void Vision()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, visionRadius, playerLayer);
        
        if (hit != null) 
        { 
            Vector2 directionToTarget = hit.transform.position - transform.position;
            RaycastHit2D losHit = Physics2D.Raycast(transform.position, directionToTarget.normalized, visionRadius, obstacleLayer | playerLayer);
            if (losHit.collider != null && losHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                // Thấy rõ Player
                hasSeenPlayer = true; // Đánh dấu đã thấy Player
                CanSeePlayer = true;
                Debug.Log("Can see Player");
                targetDetected = hit.transform;
                lastSeenPosition = hit.transform.position; // Ghi lại vị trí cuối cùng thấy Player
                Debug.DrawLine(transform.position, targetDetected.position, Color.green);
            }
            else
            {
                // Bị che khuất
                CanSeePlayer = false;
                targetDetected = null;
                
            }
        }
        else
        {
            CanSeePlayer = false;
            targetDetected = null;
        }
    }

    public void Vision2()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, visionRadius, playerLayer);

        if (hit != null)
        {
            Vector2 directionToTarget = hit.transform.position - transform.position;

            // Thấy rõ Player
            hasSeenPlayer = true; // Đánh dấu đã thấy Player
            CanSeePlayer = true;
            Debug.Log("Can see Player");
            targetDetected = hit.transform;
            lastSeenPosition = hit.transform.position; // Ghi lại vị trí cuối cùng thấy Player
            Debug.DrawLine(transform.position, targetDetected.position, Color.green);
        }
        else
        {
            CanSeePlayer = false;
            targetDetected = null;
        }
    }
    public void Vision3()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, visionRadius, playerLayer);
        if (hit != null)
        {
            Debug.Log("Player in Range");
        }
    }
    public void ClearLastSeenPosition()
    {
        lastSeenPosition = null;
    }

    public Vector3 PlayerPosition
    {
        get
        {
            return targetDetected != null ? targetDetected.position : transform.position;
        }
    }

    private void OnDrawGizmos()
    {
        if (debugDrawRays == false) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, PlayerPosition);

        if (lastSeenPosition.HasValue)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, lastSeenPosition.Value);
        }   
    }

}
