using UnityEngine;

public class MoveStage_1 : EnemyState
{
    public MoveStage_1(EnemyBrain brain) : base(brain) { }
    public EnemyLaserBeam enemyLaserBeam;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("MoveStage_1: Entering move stage");
        enemyLaserBeam = brain.GetComponent<EnemyLaserBeam>();
    }

    public override void Update()
    {
        enemyLaserBeam.HandleLockAndShoot();


        if (brain.EnemyVision.lastSeenPosition.HasValue && !brain.EnemyVision.CanSeePlayer)
        {
            Debug.Log("Move to last seen position");
            brain.EnemySteering.MoveTo(brain.EnemyVision.lastSeenPosition.Value, 2f);
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("MoveStage_1: Exiting move stage");
    }
}
