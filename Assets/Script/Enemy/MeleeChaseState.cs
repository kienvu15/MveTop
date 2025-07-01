using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class MeleeChaseState : IEnemyState
{
    private EnemyStateMachine enemy;

    public void Enter(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
    }

    public void Update()
    {
        if (enemy.vision.CanSeePlayer)
        {
            enemy.steering.MoveTo(enemy.vision.targetDetected.position, 2f);
            if (enemy.attackRange.isPlayerInRange)
            {
                enemy.ChangeState(new Attack_44());
            }
        }
        else if (enemy.vision.lastSeenPosition.HasValue)
        {
            float dist = Vector2.Distance(enemy.transform.position, enemy.vision.lastSeenPosition.Value);
            if (dist < enemy.steering.stopDistanceToLastSeen)
            {
                enemy.vision.ClearLastSeenPosition();
                enemy.steering.StopMoving(); 
                enemy.ChangeState(new Wander());
            }
            else
            {
                enemy.steering.MoveTo(enemy.vision.lastSeenPosition.Value, 2f);
            }
        }
        else
        {
            enemy.steering.StopMoving();
            enemy.ChangeState(new Wander());
        }
    }

    public void Exit()
    {
        enemy.steering.StopMoving();
    }
}
