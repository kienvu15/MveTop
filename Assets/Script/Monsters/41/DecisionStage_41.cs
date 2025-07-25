using UnityEngine;

public class DecisionStage_41 : EnemyState
{
    public DecisionStage_41(EnemyBrain brain) : base(brain) { }

    private EnemyRandomPatrolSteering enemyRandomPatrolSteering;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("DecisionStage_41: Entering decision stage");
        // Here you can add any initialization code for the decision stage

        enemyRandomPatrolSteering = brain.GetComponent<EnemyRandomPatrolSteering>();
    }

    public override void Update()
    {
        enemyRandomPatrolSteering.PatrolCondition();
        if (brain.EnemyVision.CanSeePlayer)
        {
            Debug.Log("PatrolStage_03: Player detected, transitioning to DecisionStage_03");
            enemyRandomPatrolSteering.StopPatrol();
            brain.ChangeState(new ArcAround_41(brain));
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("DecisionStage_41: Exiting decision stage");
        // Here you can add any cleanup code for the decision stage
    }
}
