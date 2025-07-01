using UnityEngine;

public class SwordPatrolState : IEnemyState
{
    public EnemyStateMachine enemy;
    public void Enter(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
    }
    
    public void Update()
    {
        enemy.patrolState.PatrolRuntinr();
        if(enemy.vision.CanSeePlayer)
        {
            enemy.ChangeState(new SwordChaseState()); // Chuyển sang trạng thái Chase khi thấy player
        }
    }
    public void Exit()
    {
        enemy.patrolState.ResetPatrolTarget();
        // Không cần dừng gì đặc biệt khi thoát khỏi trạng thái này
    }
}
