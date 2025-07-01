using UnityEngine;

public class RangedChaseState : IEnemyState
{
    private EnemyStateMachine enemy;

    public void Enter(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
        Debug.Log("Ranged: Chase");
    }

    public void Update()
    {
        float dist = Vector2.Distance(enemy.transform.position, enemy.player.position);
        if (dist < 4f)
        {
            enemy.ChangeState(new RangedShootState());
            return;
        }

        enemy.transform.position = Vector2.MoveTowards(
            enemy.transform.position, enemy.player.position, 1.5f * Time.deltaTime);
    }

    public void Exit() { }
}
