using System;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class SwordChaseState : IEnemyState
{
    public EnemyStateMachine enemy;
    
    public void Enter(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
        
        // Có thể thêm logic khởi tạo khi vào trạng thái Chase
    }

    public void Update()
    {
        if (enemy.vision.CanSeePlayer)
        {
            enemy.steering.MoveTo(enemy.vision.PlayerPosition, 2f); // 👈 đuổi theo player
        }
        else if (enemy.vision.hasSeenPlayer && enemy.vision.lastSeenPosition.HasValue)
        {
            // Nếu đến gần lastSeenPosition thì dừng lại
            float dist = Vector2.Distance(enemy.transform.position, enemy.vision.lastSeenPosition.Value);
            if (dist > enemy.steering.stopDistanceToLastSeen)
            {
                enemy.steering.MoveTo(enemy.vision.lastSeenPosition.Value,3f); // 👈 đi tới vị trí cuối cùng thấy player
            }
            else
            {
                enemy.steering.StopMoving();
                enemy.vision.ClearLastSeenPosition(); // 👈 xóa nếu đã tới nơi
            }
        }
        else
        {
            // Tuần tra hoặc đứng yên
            enemy.ChangeState(new SwordPatrolState());
        }

        if (enemy.attackVision.isPlayerInAttackRange || enemy.attackVision.playerDetected != null)
        {
            Debug.Log("Chuyển sang DashAttackState");
            enemy.ChangeState(new DashAttackState());
        }

    }

    public void Exit()
    {
        // Không cần dừng gì đặc biệt khi thoát khỏi trạng thái này
    }
}
