using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class Wander : IEnemyState
{
    private EnemyStateMachine enemy;
    
    public void Enter(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
        enemy.StartCoroutine(enemy.randomRunState.CreatSmartRandomDir());
    }

    public void Update()
    {
        enemy.steering.MoveInDirection(enemy.randomRunState.arrowDirection);
        if (enemy.vision.CanSeePlayer)
        {
            enemy.ChangeState(new MeleeChaseState()); 
        }
    }
    
    
    public void Exit()
    {
       enemy.steering.StopMoving();
    }

    
}

