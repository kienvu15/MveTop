using UnityEngine;

public class RangedIdleState : IEnemyState
{
    private EnemyStateMachine enemy;

    public void Enter(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
        Debug.Log("Ranged: Idle");
    }

    public void Update()
    {
        float dist = Vector2.Distance(enemy.transform.position, enemy.player.position);
        if (dist < 7f)
            enemy.ChangeState(new RangedChaseState());
    }

    public void Exit() { }
}

