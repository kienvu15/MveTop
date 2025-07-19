using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public PlayerStateController stateController;
    public PlayerInputHandler inputHandler;

    public float lockDuration = 2f;        // Thời gian khóa player (ví dụ 2 giây)

    public bool hasTriggered = false;

    private void Awake()
    {
        stateController = FindFirstObjectByType<PlayerStateController>();
        inputHandler = FindFirstObjectByType<PlayerInputHandler>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;          // Đảm bảo chỉ trigger 1 lần

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(LockPlayerRoutine());
        }
    }

    private System.Collections.IEnumerator LockPlayerRoutine()
    {
        inputHandler.isLocked = true;

        stateController.canMove = false;
        stateController.canFlip = false;
        stateController.canDash = false;
        stateController.canAttack = false;

        // Dừng ngay lập tức
        Rigidbody2D rb = stateController.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(lockDuration);

        stateController.canMove = true;
        stateController.canFlip = true;
        stateController.canDash = true;
        stateController.canAttack = true;

        inputHandler.isLocked = false;
    }

}
