using UnityEngine;

public class SwordIdleState : IEnemyState
{
    public EnemyStateMachine enemy;
    public void Enter(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
    }

    public void Update()
    {
        enemy.ChangeState(new SwordPatrolState()); // Chuyển sang trạng thái Chase khi có thể nhìn thấy người chơi
    }

    public void Exit()
    {
        // Không cần dừng gì đặc biệt khi thoát khỏi trạng thái này
    }
}
