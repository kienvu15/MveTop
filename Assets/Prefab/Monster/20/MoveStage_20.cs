using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MoveStage_20 : EnemyState
{
    public MoveStage_20(EnemyBrain brain) : base(brain) { }
    public AvoidPlayer avoidPlayer;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("MoveStage_20: Entering move stage");
        avoidPlayer = brain.GetComponent<AvoidPlayer>();
    }

    public override void Update()
    {
        float distToPlayer = Vector2.Distance(brain.transform.position, brain.EnemyVision.targetDetected.position);
        if (brain.EnemyVision.CanSeePlayer && brain.EnemyVision.targetDetected != null)
        {
            if (distToPlayer <= avoidPlayer.shotRadius &&
                GridManager.Instance.HasLineOfSight(brain.transform.position, avoidPlayer.player.position))
            {
                avoidPlayer.TryShoot();
            }
            else
            {
                avoidPlayer.MoveTowardPlayer();
            }
        }

        if (avoidPlayer.hasShot)
        {
            brain.ChangeState(new DodgeStage_20(brain));
            Debug.Log("MoveStage_20: Transitioning to DodgeStage_20");
        }

        if (!avoidPlayer.isDodging && distToPlayer <= avoidPlayer.avoidRadius && Time.time >= avoidPlayer.nextShootTime)
        {
            avoidPlayer.isDodging = true;
            avoidPlayer.waitingToShoot = false;
            avoidPlayer.retreatNode = null;
            avoidPlayer.ChooseCurvedRetreatDirection();
            avoidPlayer.nextShootTime = Time.time + avoidPlayer.cooldownAfterDodge;
        }
    }

    public override void Exit()
    {
        base.Exit();
        // Here you can add any cleanup code for the move stage
        Debug.Log("MoveStage_20: Exiting move stage");
    }
}
