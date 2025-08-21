using UnityEngine;

public class DecisionStage_00 : EnemyState
{
    public DecisionStage_00(EnemyBrain brain) : base(brain) { }
    private EnemyRandomPatrolSteering enemyRandomPatrolSteering;

    public float stateTimer = 0f;
    public float stateDuration = 1f;
    public override void Enter()
    {

        base.Enter();
        Debug.Log("DecisionStage_00: Entering decision stage");
        // Here you can add any initialization code for the decision stage
        enemyRandomPatrolSteering = brain.GetComponent<EnemyRandomPatrolSteering>();

        stateTimer = 0f;

    }

    public override void Update()
    {

        if (brain.EnemyStateController.canMove)
        {
            enemyRandomPatrolSteering.PatrolCondition();
        }

        stateTimer += Time.deltaTime;
        if (brain.EnemyVision.CanSeePlayer && stateTimer > stateDuration)
        {

            Debug.Log("PatrolStage_03: Player detected, transitioning to DecisionStage_03");
            enemyRandomPatrolSteering.StopPatrol();
            brain.ChangeState(new ArcAroundStage_00(brain));
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
