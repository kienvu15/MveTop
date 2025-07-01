public interface IEnemyState
{
    void Enter(EnemyStateMachine enemy);
    void Update();
    void Exit();
}
