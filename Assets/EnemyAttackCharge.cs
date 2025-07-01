using System.Collections;
using UnityEngine;

public class EnemyAttackCharge : MonoBehaviour
{
    public float chargeTime = 1.5f;            // Thời gian vận chiêu
    public float chargeSpeed = 10f;            // Tốc độ lao
    public float attackCooldown = 2f;          // Thời gian hồi sau tấn công

    private bool isPlayerInRange = false;
    private bool isCharging = false;
    private bool isCooldown = false;

    private Vector2 chargeTarget;
    private Rigidbody2D rb;
    private Transform player;
    public EnemyVision vision;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Gọi từ vision script
        if (vision.CanSeePlayer == true && !isCharging && !isCooldown)
        {
            StartCoroutine(ChargeAttack());
        }
    }

    IEnumerator ChargeAttack()
    {
        isCharging = true;

        // Dừng chuyển động trong lúc vận chiêu
        rb.linearVelocity = Vector2.zero;

        // Hiệu ứng vận chiêu tại đây (nếu có)
        yield return new WaitForSeconds(chargeTime);

        // Ghi lại vị trí Player sau khi vận chiêu xong
        chargeTarget = player.position;

        // Tính hướng lao
        Vector2 dir = (chargeTarget - (Vector2)transform.position).normalized;

        // 🌟 Vẽ ray để debug hướng phóng
        Debug.DrawRay(transform.position, dir * 5f, Color.red, 1f); // 5f là độ dài, 1f là thời gian hiển thị

        // Tấn công lao tới
        rb.linearVelocity = dir * chargeSpeed;

        // Di chuyển trong một khoảng ngắn (hoặc chờ đến khi chạm vật cản)
        yield return new WaitForSeconds(0.3f);

        rb.linearVelocity = Vector2.zero;

        isCharging = false;
        isCooldown = true;

        yield return new WaitForSeconds(attackCooldown);

        isCooldown = false;
    }


    // Hàm này gọi từ hệ thống Vision detection
    public void SetPlayerInRange(bool value)
    {
        isPlayerInRange = value;
    }
}
