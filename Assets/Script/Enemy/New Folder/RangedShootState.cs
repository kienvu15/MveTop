using UnityEngine;

public class RangedShootState : IEnemyState
{
    private EnemyStateMachine enemy;
    private float timer;

    public void Enter(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
        timer = 0f;
        Debug.Log("Ranged: Shoot");
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1.5f)
        {
            Debug.Log("Ranged enemy shoots!");
            enemy.ChangeState(new RangedIdleState());
        }
    }

    public void Exit() { }
}

