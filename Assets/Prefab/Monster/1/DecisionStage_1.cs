using UnityEngine;

public class DecisionStage_1 : EnemyState
{
    public DecisionStage_1(EnemyBrain brain) : base(brain) { }

    private EnemyRandomPatrolSteering enemyRandomPatrolSteering;
    public float stateTimer = 0f;
    public float stateDuration = Random.Range(1f, 2f); // Duration for the decision stage
    public override void Enter()
    {
        base.Enter();
        Debug.Log("DecisionStage_1: Entering decision stage");
        // Here you can add any initialization code for the decision stage
        stateTimer = 0f;
        stateDuration = Random.Range(1f, 2f);
        enemyRandomPatrolSteering = brain.GetComponent<EnemyRandomPatrolSteering>();
    }

    public override void Update()
    {
        enemyRandomPatrolSteering.PatrolCondition();
        if (brain.EnemyVision.CanSeePlayer)
        {
            stateTimer += Time.deltaTime;
            if (stateTimer >= stateDuration)
            {
                Debug.Log("DecisionStage_1: Time elapsed, transitioning to ArcAround_1");
                enemyRandomPatrolSteering.StopPatrol();
                brain.ChangeState(new MoveStage_1(brain));
            }

        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("DecisionStage_1: Exiting decision stage");
        // Here you can add any cleanup code for the decision stage
    }
}
