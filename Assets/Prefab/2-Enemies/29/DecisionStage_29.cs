using UnityEngine;

public class DecisionStage_29 : EnemyState
{

    public DecisionStage_29(EnemyBrain brain) : base(brain) { }

    private EnemyRandomPatrolSteering enemyRandomPatrolSteering;
    public float stateTimer;
    public float stateDuration;

    public override void Enter()
    {
        base.Enter();
        enemyRandomPatrolSteering = brain.GetComponent<EnemyRandomPatrolSteering>();
        stateDuration = Random.Range(0.3f, 1f);
        stateTimer = 0f;
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
            if (stateTimer >= stateDuration)
            {
                Debug.Log("DecisionStage_29: Time elapsed, transitioning to ArcAround_29");
                enemyRandomPatrolSteering.StopPatrol();
                brain.ChangeState(new ArcAround_29(brain));
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
