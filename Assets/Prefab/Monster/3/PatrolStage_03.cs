using UnityEngine;

public class PatrolStage_03 : EnemyState
{
    public PatrolStage_03(EnemyBrain brain) : base(brain) { }

    private EnemyRandomPatrolSteering enemyRandomPatrolSteering;
    public float stateTimer = 0.0f;
    public float stateDuration = Random.Range(0.8f, 1.5f);
    public override void Enter()
    {
        base.Enter();
        enemyRandomPatrolSteering = brain.GetComponent<EnemyRandomPatrolSteering>();
        stateTimer = 0f;
        stateDuration = Random.Range(0.8f, 1f);
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
                Debug.Log("PatrolStage_03: Player detected, transitioning to DecisionStage_03");
                enemyRandomPatrolSteering.StopPatrol();
                brain.ChangeState(new DecisionStage_03(brain));
            }
        }

        //if (brain.EnemyVision.lastSeenPosition != null && brain.EnemyVision.CanSeePlayer == false)
        //{
        //    Vector2 lastSeen = brain.EnemyVision.lastSeenPosition.Value;
        //    brain.EnemySteering.MoveTo(lastSeen, 1f);

        //    float dist = Vector2.Distance(brain.transform.position, lastSeen);
        //    if (dist < 0.5f)
        //    {
        //        brain.EnemySteering.StopMoving();
        //        brain.ChangeState(new PatrolStage_03(brain));
        //    }
        //}
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("PatrolStage_03 Exit");
    }
}
