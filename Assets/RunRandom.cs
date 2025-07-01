using UnityEngine;
using System.Collections;

public class RunRandom : MonoBehaviour
{
    [Header("Deceptive Movement")]
    public float directionChangeInterval = 1.5f;
    

    private Rigidbody2D rb;

    public Vector2 arrowDirection = Vector2.right;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        
    }

    public IEnumerator CreatSmartRandomDir()
    {
        while (true)
        {
            Vector2 bestDir = Vector2.zero;
            float maxClearDistance = 0f;

            // Thử nhiều hướng ngẫu nhiên, chọn hướng ít bị cản nhất
            for (int i = 0; i < 10; i++)
            {
                float angle = Random.Range(0f, 360f);
                Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;

                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 3f, LayerMask.GetMask("Obstacle")); // thay "Obstacle" bằng layer bạn dùng
                float clearDist = hit.collider != null ? hit.distance : 3f;

                if (clearDist > maxClearDistance)
                {
                    maxClearDistance = clearDist;
                    bestDir = dir;
                }
            }

            arrowDirection = bestDir;
            Debug.DrawRay(transform.position, arrowDirection * 2f, Color.magenta, directionChangeInterval);

            yield return new WaitForSeconds(directionChangeInterval);
        }
    }




}
