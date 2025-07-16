using UnityEngine;

public class EnemyRanged02 : MonoBehaviour
{
    public Transform player;
    
    public float preferredDistance = 4f;
    public float shootCooldown = 1.5f;
    public float moveSpeed = 1.5f;

    public GameObject bulletPrefab;
    public Transform firePoint;

    private float shootTimer = 0f;
    

    private EnemyVision enemyVision;

    private void Awake()
    {
        enemyVision = GetComponent<EnemyVision>();
    }
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Di chuyển nếu quá gần hoặc quá xa
        if (enemyVision.CanSeePlayer == true)
        {
            Vector2 dirToPlayer = (player.position - transform.position).normalized;

            if (distanceToPlayer > preferredDistance + 1f)
            {
                // Di chuyển lại gần
                transform.Translate(dirToPlayer * moveSpeed * Time.deltaTime);
            }
            else if (distanceToPlayer < preferredDistance - 0.5f)
            {
                // Di chuyển tránh xa
                transform.Translate(-dirToPlayer * moveSpeed * Time.deltaTime);
            }
            else
            {
                MoveOrbitStepOnce();
            }
        }

        //// Bắn nếu trong khoảng thích hợp
        //if (enemyVision.CanSeePlayer == true && Mathf.Abs(distanceToPlayer - preferredDistance) < 1f)
        //{
        //    shootTimer += Time.deltaTime;

        //    if (shootTimer >= shootCooldown)
        //    {
        //        ShootAtPlayer();
        //        shootTimer = 0f;
        //    }
        //}
    }
    private float zigzagTimer = 0f;
    private int zigzagDir = 1;

    float orbitAngle = 45f; // góc xoay nhỏ (đổi hướng mỗi lần)
    float orbitStepDistance = 1f; // chỉ di chuyển khoảng này mỗi lần

    void MoveOrbitStepOnce()
    {
        Vector3 dir = transform.position - player.position;
        dir = Quaternion.Euler(0, 0, orbitAngle) * dir; // xoay một chút quanh Player
        Vector3 targetPos = player.position + dir.normalized * preferredDistance;

        // Di chuyển 1 đoạn theo hướng đó
        Vector3 moveDir = (targetPos - transform.position).normalized;
        transform.position += moveDir * orbitStepDistance;
    }


    void MoveAroundPlayer()
    {
        //float orbitSpeed = 30f;
        //Vector3 dir = transform.position - player.position;
        //dir = Quaternion.Euler(0, 0, orbitSpeed * Time.deltaTime) * dir;
        //Vector3 targetPos = player.position + dir.normalized * preferredDistance;
        //transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        zigzagTimer += Time.deltaTime;

        if (zigzagTimer > 1.5f)
        {
            zigzagDir *= -1;
            zigzagTimer = 0f;
        }

        Vector2 dirToPlayer = (player.position - transform.position).normalized;
        Vector2 perpendicular = new Vector2(-dirToPlayer.y, dirToPlayer.x); // trái/phải

        Vector2 moveDir = perpendicular * zigzagDir;
        transform.Translate(moveDir * moveSpeed * 0.5f * Time.deltaTime);
    }


    void ShootAtPlayer()
    {
        Vector2 dir = (player.position - firePoint.position).normalized;
        firePoint.right = dir;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = firePoint.right * 6f;
    }
}