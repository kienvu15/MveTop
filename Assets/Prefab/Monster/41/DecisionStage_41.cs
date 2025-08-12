using UnityEngine;

public class DecisionStage_41 : EnemyState
{
    public DecisionStage_41(EnemyBrain brain) : base(brain) { }

    private EnemyRandomPatrolSteering enemyRandomPatrolSteering;
    public float stateTimer = 0f;
    public float stateDuration = Random.Range(1f, 2f); // Duration for the decision stage
    public override void Enter()
    {
        base.Enter();
        Debug.Log("DecisionStage_41: Entering decision stage");
        // Here you can add any initialization code for the decision stage
        stateTimer = 0f;
        stateDuration = Random.Range(0.5f, 1f);
        enemyRandomPatrolSteering = brain.GetComponent<EnemyRandomPatrolSteering>();
    }

    public override void Update()
    {
        if (brain.EnemyStateController.canMove)
        {
            enemyRandomPatrolSteering.PatrolCondition();
        }

        if (brain.EnemyVision.CanSeePlayer)
        {
            stateTimer += Time.deltaTime;
            if(stateTimer >= stateDuration)
            {
                Debug.Log("DecisionStage_41: Time elapsed, transitioning to ArcAround_41");
                enemyRandomPatrolSteering.StopPatrol();
                brain.ChangeState(new ArcAround_41(brain));
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("DecisionStage_41: Exiting decision stage");
        // Here you can add any cleanup code for the decision stage
    }
}
