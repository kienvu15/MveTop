using UnityEngine;

public class PatrolStage_03 : EnemyState
{
    public PatrolStage_03(EnemyBrain brain) : base(brain) { }

    private EnemyRandomPatrolSteering enemyRandomPatrolSteering;
    public override void Enter()
    {
        base.Enter();
        enemyRandomPatrolSteering = brain.GetComponent<EnemyRandomPatrolSteering>();
    }

    public override void Update()
    {
        
        enemyRandomPatrolSteering.PatrolCondition();
        if (brain.EnemyVision.CanSeePlayer)
        {
            Debug.Log("PatrolStage_03: Player detected, transitioning to DecisionStage_03");
            enemyRandomPatrolSteering.StopPatrol();
            brain.ChangeState(new DecisionStage_03(brain));
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("PatrolStage_03 Exit");
    }
}
