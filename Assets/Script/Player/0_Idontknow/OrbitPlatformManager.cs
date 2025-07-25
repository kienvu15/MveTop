using UnityEngine;

public class OrbitPlatformManager : MonoBehaviour
{
    public Transform[] platforms;   // Danh sách platform
    public float radius = 2f;       // Bán kính tối đa
    public float speed = 50f;       // Tốc độ quay
    public float radiusChangeSpeed = 1f; // Tốc độ thu vào/giãn ra

    private float angleOffset;
    private float currentRadius;
    private bool isShrinking = false;
    private bool isExpanding = false;

    void Start()
    {
        angleOffset = 360f / platforms.Length;
        currentRadius = radius;

        InvokeRepeating(nameof(StartShrink), 2f, Random.Range(5f, 8f));  // Tự động random thu vào
    }

    void Update()
    {
        // Điều chỉnh bán kính (thu vào hoặc giãn ra)
        if (isShrinking)
        {
            currentRadius = Mathf.MoveTowards(currentRadius, 0f, radiusChangeSpeed * Time.deltaTime);

            if (currentRadius <= 0.05f)
            {
                isShrinking = false;
                Invoke(nameof(StartExpand), Random.Range(0.5f, 1.5f));   // Chờ rồi mới nở ra
            }
        }
        else if (isExpanding)
        {
            currentRadius = Mathf.MoveTowards(currentRadius, radius, radiusChangeSpeed * Time.deltaTime);

            if (Mathf.Abs(currentRadius - radius) < 0.05f)
            {
                isExpanding = false;
            }
        }

        // Quay vòng tròn
        for (int i = 0; i < platforms.Length; i++)
        {
            float angle = (Time.time * speed) + (angleOffset * i);
            float radian = angle * Mathf.Deg2Rad;

            Vector3 newPosition = new Vector3(
                transform.position.x + Mathf.Cos(radian) * currentRadius,
                transform.position.y + Mathf.Sin(radian) * currentRadius,
                platforms[i].position.z
            );

            platforms[i].position = newPosition;
            platforms[i].rotation = Quaternion.identity;
        }
    }

    void StartShrink()
    {
        if (!isShrinking && !isExpanding)
        {
            isShrinking = true;
        }
    }

    void StartExpand()
    {
        isExpanding = true;
    }

    
}
