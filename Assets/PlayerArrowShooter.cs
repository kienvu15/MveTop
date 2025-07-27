using UnityEngine;

public class PlayerArrowShooter : MonoBehaviour
{
    public GameObject arrowPrefab;             // Prefab mũi tên
    public Transform attackPoint;              // Vị trí bắn
    public float arrowSpeed = 10f;             // Tốc độ mũi tên
    public float cooldownTime = 3f;            // Hồi chiêu
    private float lastShootTime = -Mathf.Infinity;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && Time.time >= lastShootTime + cooldownTime)
        {
            ShootArrow();
            lastShootTime = Time.time;
        }
    }

    void ShootArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, attackPoint.position, attackPoint.rotation);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = attackPoint.right * arrowSpeed;
        }
    }
}
