using System;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DashAttackState : IEnemyState
{
    public EnemyStateMachine enemy;
  
    public void Enter(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
        
    }
    public void Update()
    {
        if (!enemy.attackVision.isPlayerInAttackRange || enemy.attackVision.playerDetected == null)
        {
            enemy.steering.StopMoving();
            enemy.enemyCombatState.hasTarget = false;
            return;
        }

        // Tính khoảng cách đến Player để tránh vượt quá hoặc tiến lại gần quá
        float distToPlayer = Vector2.Distance(enemy.transform.position, enemy.attackVision.playerDetected.position);
        if (distToPlayer < enemy.enemyCombatState.avoidPlayerRadius)
        {
            // Nếu quá gần Player, né ra xa
            Vector2 fleeDir = (enemy.transform.position - enemy.attackVision.playerDetected.position).normalized;
            enemy.steering.MoveInDirection(fleeDir);
            enemy.enemyCombatState.hasTarget = false;
            return;
        }

        enemy.enemyCombatState.wanderTimer -= Time.deltaTime;
        if (!enemy.enemyCombatState.hasTarget || enemy.enemyCombatState.wanderTimer <= 0f || Vector2.Distance(enemy.transform.position, enemy.enemyCombatState.wanderTarget) < 0.2f)
        {
            enemy.enemyCombatState.PickNewWanderTarget();
        }

        enemy.steering.MoveTo(enemy.enemyCombatState.wanderTarget, 2f);



    }

    public void Exit()
    { }
}

