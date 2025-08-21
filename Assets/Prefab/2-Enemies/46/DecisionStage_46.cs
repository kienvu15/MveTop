using UnityEngine;

public class DecisionStage_46 : EnemyState
{
    public DecisionStage_46(EnemyBrain brain) : base(brain) { }

    private EnemyRandomPatrolSteering enemyRandomPatrolSteering;
    private EnemyLaserBeam eneemyLaserBeam;
    public float stateTimer = 0f;
    public float stateDuration = Random.Range(1f, 2f); // Duration for the decision stage
    public override void Enter()
    {
        base.Enter();
        Debug.Log("DecisionStage_46: Entering decision stage");
        stateTimer = 0f;
        stateDuration = Random.Range(0.5f, 1.2f);
        enemyRandomPatrolSteering = brain.GetComponent<EnemyRandomPatrolSteering>();
        eneemyLaserBeam = brain.GetComponent<EnemyLaserBeam>();
    }

    public override void Update()
    {
        enemyRandomPatrolSteering.PatrolCondition();

        if (brain.EnemyVision.CanSeePlayer)
        {
            stateTimer += Time.deltaTime;
            if (stateTimer >= stateDuration)
            {
                Debug.Log("DecisionStage_46: Time elapsed, transitioning to ArcAround_46");
                enemyRandomPatrolSteering.StopPatrol();
                brain.ChangeState(new ArcAround_46(brain));
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        
    }

}
