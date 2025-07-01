using UnityEngine;

public class MeleeIdleState : IEnemyState
{
    private EnemyStateMachine enemy;

    public void Enter(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
        Debug.Log("Melee: Idle");
    }

    public void Update()
    {
        if(enemy.vision.CanSeePlayer)
        {
            enemy.ChangeState(new MeleeChaseState());
        }
    }

    public void Exit() { }
}
