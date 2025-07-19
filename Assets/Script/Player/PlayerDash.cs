using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    

    [SerializeField] private TrailRenderer trailRenderer;
    private Vector2 lastMoveDirection = Vector2.down; // Default to right direction

    private PlayerInputHandler input;
    private Rigidbody2D rb;
    private PlayerStateController StateController;
    private PlayerStats PlayerStats;

    void Awake()
    {
        // Lấy các thành phần cần thiết
        input = GetComponent<PlayerInputHandler>();
        rb = GetComponent<Rigidbody2D>();
        StateController = GetComponent<PlayerStateController>();
        PlayerStats = GetComponent<PlayerStats>();

        // Đảm bảo trailRenderer được thiết lập nếu có
        if (trailRenderer != null)
            trailRenderer.emitting = false;
    }
 
    // Update is called once per frame
    void Update()
    {
        if (input.DashPressed && StateController.canDash && !StateController.isDashing)
        {
            StartCoroutine(DashCoroutine());
        }

        if (input.MoveInput != Vector2.zero)
        {
            lastMoveDirection = input.MoveInput;
        }
    }

    private IEnumerator DashCoroutine()
    {
        StateController.canDash = false;
        StateController.isDashing = true;

        if (trailRenderer != null)
            trailRenderer.emitting = true;

        // Xác định hướng dash
        // Nếu người chơi đang di chuyển, dash theo hướng đó
        Vector2 dashDirection = input.MoveInput;

        // Nếu đứng yên, dash theo hướng di chuyển cuối cùng
        if (dashDirection == Vector2.zero)
        {
            dashDirection = lastMoveDirection;
        }

        // Áp dụng lực dash
        // Normalize để đảm bảo tốc độ dash không đổi
        rb.linearVelocity = dashDirection.normalized * PlayerStats.dashSpeed;

        yield return new WaitForSeconds(PlayerStats.dashDuration);

        // Kết thúc dash
        rb.linearVelocity = Vector2.zero;
        StateController.isDashing = false;

        if (trailRenderer != null)
            trailRenderer.emitting = false;

        // Bắt đầu thời gian hồi chiêu
        yield return new WaitForSeconds(PlayerStats.dashCooldown);
        StateController.canDash = true;
    }
}
