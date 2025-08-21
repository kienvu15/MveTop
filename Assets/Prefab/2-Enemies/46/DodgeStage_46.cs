using System;
using UnityEngine;
using static UnityEngine.CullingGroup;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DodgeStage_46 : EnemyState
{
    public DodgeStage_46(EnemyBrain brain) : base(brain) { }

    public EnemyLaserBeam enemyLaserBeam;
    public RangedEnemyController rangedEnemyController;
    public AvoidPlayer avoidPlayer;

    private float stateTimer = 0f;
    private float stateDuration; 
    public override void Enter()
    {
        base.Enter();
        Debug.Log("DodgeStage_20: Entering dodge stage");
        enemyLaserBeam = brain.GetComponent<EnemyLaserBeam>();
        avoidPlayer = brain.GetComponent<AvoidPlayer>();
        rangedEnemyController = brain.GetComponent<RangedEnemyController>();

        stateTimer = 0f;
        stateDuration = UnityEngine.Random.Range(0.5f, 1.5f); // Random duration for the dodge stage
    }

    public override void Update()
    {
        if (brain.EnemyStateController.canMove)
        {
            rangedEnemyController.DritDec();
        }

        stateTimer += Time.deltaTime;

        if (stateTimer >= stateDuration)
        {
            rangedEnemyController.StopDritDec();
            brain.ChangeState(new ArcAround_46(brain));
        }

        if (brain.EnemyVision.lastSeenPosition.HasValue && !brain.EnemyVision.CanSeePlayer)
        {
            brain.EnemySteering.StopMoving();
            rangedEnemyController.StopDritDec();
            brain.ChangeState(new LastSeenStage_46(brain));
        }
    }

    public override void Exit()
    {
        base.Exit();
        // Here you can add any cleanup code for the dodge stage
        Debug.Log("DodgeStage_20: Exiting dodge stage");
    }
}
