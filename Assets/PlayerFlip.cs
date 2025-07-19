using UnityEngine;

public enum LookDirection { Left, Right }

public class PlayerFlip : MonoBehaviour
{
    public bool DebugMode = false; // Chế độ debug để hiển thị bán kính tầm nhìn

    [Header("Player Settings")]
    public float PlayerVisionRadius = 10f; // Bán kính tầm nhìn của người chơi
    public bool canSeeEnemy = false;
    public LayerMask enemyLayers;
    public bool isFacingRight = true; // Biến để xác định hướng nhìn của người chơi

    public Transform playerSprite;
    private PlayerInputHandler playerInputHandler;
    private PlayerStateController playerStateController;
    public LookDirection lookDirection { get; private set; }

    private void Awake()
    {
        playerInputHandler = FindFirstObjectByType<PlayerInputHandler>();
        playerStateController = FindFirstObjectByType<PlayerStateController>();
    }
    void Update()
    {
        if (playerStateController.canFlip) 
            {
                Flip(); 
            } 
    }

    public void Flip()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, PlayerVisionRadius, enemyLayers);

        if (enemies.Length != 0)
        {
            canSeeEnemy = true;
            // Tìm enemy gần nhất
            Transform nearest = null;
            float minDist = Mathf.Infinity;
            foreach (var enemy in enemies)
            {
                float dist = Vector2.Distance(transform.position, enemy.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = enemy.transform;
                }
            }

            if (nearest == null) return;
           
            if (nearest.position.x < transform.position.x)
            {
                if (isFacingRight == true)
                {
                    isFacingRight = false;
                    playerSprite.localScale = new Vector3(-1, 1, 1);
                    lookDirection = LookDirection.Left;
                }
            }
            else
            {
                if (isFacingRight == false)
                {
                    isFacingRight = true;
                    playerSprite.localScale = new Vector3(1, 1, 1);
                    lookDirection = LookDirection.Right;
                }
            }
        }
        else if (enemies.Length == 0)
        {
            canSeeEnemy = false;
            Flip2();
        }

    }

    public void Flip2()
    {
        if(playerInputHandler.MoveInput.x > 0 && isFacingRight == false || playerInputHandler.MoveInput.x < 0 && isFacingRight == true)
        {
            isFacingRight = !isFacingRight;
            Vector3 Scale = playerSprite.localScale;
            Scale.x *= -1;
            playerSprite.localScale = Scale;
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (DebugMode == true)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, PlayerVisionRadius);
        }
    }
}