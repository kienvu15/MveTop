using UnityEngine;

public class Missile : MonoBehaviour
{
    public PlayerStats playerStats;
    public EnemyStats enemyStats;
    public HomingBullet homingBullet; // Assuming this is a script that handles homing behavior
    Animator animator;

    private CircleCollider2D circleCollider;
    public bool player = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        homingBullet = GetComponent<HomingBullet>();
        if(player == true)
        {
            circleCollider = GetComponent<CircleCollider2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void activeBox()
    {
        circleCollider.enabled = true;
    }

    public void unactiveBox()
    {
        circleCollider.enabled = false;
    }

    public void setDestroyOnanimation()
    {
        Destroy(gameObject);
    }

    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private LayerMask obstacleLayers;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isTargetTag = collision.CompareTag("Player");
        bool isTargetLayer = ((1 << collision.gameObject.layer) & targetLayers) != 0;
        bool isObstacleLayer = ((1 << collision.gameObject.layer) & obstacleLayers) != 0;

        EnemyStats enemy = collision.GetComponent<EnemyStats>();
        PlayerStats player = collision.GetComponent<PlayerStats>();

        if (isTargetTag && player != null && player == false)
        {
            Debug.Log("Bullet hit target by tag or layer!");
            homingBullet.stopped = true; // Assuming this stops the homing behavior
            homingBullet.rb.linearVelocity = Vector2.zero; // Stop the bullet's movement
            animator.SetBool("Exploded", true);
            player.TakeDamage(enemyStats.damage, transform.position);
        }

        if( isTargetLayer && enemy != null)
        {
            Debug.Log("Bullet hit target by layer!");
            homingBullet.stopped = true; // Assuming this stops the homing behavior
            homingBullet.rb.linearVelocity = Vector2.zero; // Stop the bullet's movement
            animator.SetBool("Exploded", true);
            enemy.TakeDamage(enemy.damage, transform.position);
        }

        if( isObstacleLayer )
        {
            Debug.Log("Bullet hit an obstacle, destroying the bullet.");
            homingBullet.stopped = true; // Assuming this stops the homing behavior
            homingBullet.rb.linearVelocity = Vector2.zero; // Stop the bullet's movement
            animator.SetBool("Exploded", true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //bool isTargetTag = collision.CompareTag("Player");
        //bool isTargetLayer = ((1 << collision.gameObject.layer) & targetLayers) != 0;

        //EnemyStats enemy = collision.GetComponent<EnemyStats>();
        //PlayerStats player = collision.GetComponent<PlayerStats>();

        //if(isTargetTag && player != null || isTargetLayer && enemy != null)
        //{
        //    Debug.Log("Player is still in the trigger area, applying damage.");
        //    if(player != null)
        //    {
        //        player.TakeDamage(enemyStats.damage, transform.position);
        //    }
        //    if(enemy != null)
        //    {
        //        enemy.TakeDamage(enemyStats.damage, transform.position);
        //    }
        //}

        bool isTargetTag = collision.CompareTag("Player");
        bool isTargetLayer = ((1 << collision.gameObject.layer) & targetLayers) != 0;
        bool isObstacleLayer = ((1 << collision.gameObject.layer) & obstacleLayers) != 0;

        EnemyStats enemy = collision.GetComponent<EnemyStats>();
        PlayerStats player = collision.GetComponent<PlayerStats>();

        if (isTargetTag && player != null && player == false)
        {
            Debug.Log("Bullet hit target by tag or layer!");
            homingBullet.stopped = true; // Assuming this stops the homing behavior
            homingBullet.rb.linearVelocity = Vector2.zero; // Stop the bullet's movement
            animator.SetBool("Exploded", true);
            player.TakeDamage(enemyStats.damage, transform.position);
        }

        if (isTargetLayer && enemy != null)
        {
            Debug.Log("Bullet hit target by layer!");
            homingBullet.stopped = true; // Assuming this stops the homing behavior
            homingBullet.rb.linearVelocity = Vector2.zero; // Stop the bullet's movement
            animator.SetBool("Exploded", true);
            enemy.TakeDamage(enemy.damage, transform.position);
        }

        if (isObstacleLayer)
        {
            Debug.Log("Bullet hit an obstacle, destroying the bullet.");
            homingBullet.stopped = true; // Assuming this stops the homing behavior
            homingBullet.rb.linearVelocity = Vector2.zero; // Stop the bullet's movement
            animator.SetBool("Exploded", true);
        }
    }
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        Debug.Log("Player is still in the trigger area, applying damage.");
    //        playerStats.TakeDamage(enemyStats.damage, transform.position);
    //    }
    //}
}
