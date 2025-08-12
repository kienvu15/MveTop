using UnityEngine;

public enum AttackType
{
    Melee,  // Cận chiến
    Range   // Tầm xa
}

public class PlayerAttackManager : MonoBehaviour
{
    [Header("References")]
    public PlayerAttack playerAttack;               // Cận chiến (có thể null)
    public PlayerArrowShooter playerArrowShooter;   // Tầm xa (có thể null)
    public PlayerFlip playerFlip;
    public PlayerStats playerStats;

    [Header("Ranges")]
    public float meleeRange = 2f;   // Tầm cận chiến

    [Header("Default Behavior")]
    public AttackType defaultAttackType = AttackType.Melee; // Nếu không thấy enemy thì chọn loại này

    private Transform currentTarget;

    void Update()
    {
        if (playerFlip.canSeeEnemy)
        {
            currentTarget = GetNearestEnemy();
            if (currentTarget != null)
            {
                float dist = Vector2.Distance(transform.position, currentTarget.position);

                // Nếu có attack cận chiến và trong tầm melee
                if (playerAttack != null && dist <= meleeRange)
                {
                    EnableMelee();
                }
                // Nếu có attack tầm xa và trong tầm bow
                else if (playerArrowShooter != null && dist > meleeRange)
                {
                    EnableRange();
                }
                else
                {
                    DisableAll();
                }
            }
        }
        else
        {
            // Không thấy enemy → bật theo loại mặc định
            if (defaultAttackType == AttackType.Melee && playerAttack != null)
            {
                EnableMelee();
            }
            else if (defaultAttackType == AttackType.Range && playerArrowShooter != null)
            {
                EnableRange();
            }
        }
    }

    void EnableMelee()
    {
        if (playerAttack != null) playerAttack.enabled = true;
        if (playerArrowShooter != null) playerArrowShooter.enabled = false;
    }

    void EnableRange()
    {
        if (playerAttack != null) playerAttack.enabled = false;
        if (playerArrowShooter != null) playerArrowShooter.enabled = true;
    }

    void DisableAll()
    {
        if (playerAttack != null) playerAttack.enabled = false;
        if (playerArrowShooter != null) playerArrowShooter.enabled = false;
    }

    Transform GetNearestEnemy()
    {
        LayerMask mask = (playerAttack != null) ? playerAttack.enemyLayers : playerArrowShooter.enemyLayers;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, playerFlip.PlayerVisionRadius, mask);

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
        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
