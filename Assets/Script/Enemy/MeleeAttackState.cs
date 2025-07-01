using UnityEngine;

public class MeleeAttackState : IEnemyState
{
    private EnemyStateMachine enemy;
    private float timer = 0;

    public void Enter(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
        timer = 0f;
        Debug.Log("Melee: Attack!");
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1.2f)
            enemy.ChangeState(new MeleeIdleState());
    }

    public void Exit() { }
}

