using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingBullet : MonoBehaviour
{
    [Header("Targeting")]
    public Transform target;
    public float maxLifetime = 5f;
    [Range(0f, 1f)] public float homingDurationPercent = 0.7f;

    [Header("Movement")]
    public float startSpeed = 3f;
    public float maxSpeed = 8f;
    public float turnSpeed = 180f;
    public bool stopped = false;

    private float currentSpeed;
    private float lifeTimer;
    private Vector2 currentDirection;
    public Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Lấy hướng ban đầu: nếu có target → hướng về target, không thì dùng transform.right
        if (target != null)
            currentDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;
        else
            currentDirection = transform.right;

        currentSpeed = startSpeed;
    }

    void FixedUpdate()
    {
        if(stopped == false)
        {
            lifeTimer += Time.fixedDeltaTime;
            currentSpeed = Mathf.Lerp(startSpeed, maxSpeed, lifeTimer / maxLifetime);

            float homingPercent = Mathf.Clamp01((homingDurationPercent * maxLifetime - lifeTimer) / (homingDurationPercent * maxLifetime));

            if (target != null && homingPercent > 0f)
            {
                Vector2 desiredDir = ((Vector2)target.position - rb.position).normalized;

                float angleDiff = Vector2.SignedAngle(currentDirection, desiredDir);
                float maxRotate = turnSpeed * Time.fixedDeltaTime * homingPercent;
                float rotateAmount = Mathf.Clamp(angleDiff, -maxRotate, maxRotate);

                currentDirection = Quaternion.Euler(0, 0, rotateAmount) * currentDirection;
            }

            currentDirection.Normalize();

            // Update hướng nhìn (nếu cần)
            float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
            rb.rotation = angle;

            // Di chuyển
            rb.linearVelocity = currentDirection * currentSpeed;

            // Tự huỷ
            if (lifeTimer >= maxLifetime)
                Destroy(gameObject);
        }
    }
}
