using UnityEngine;
using static UnityEngine.CullingGroup;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DodgeStage_20 : EnemyState
{
    public DodgeStage_20(EnemyBrain brain) : base(brain) { }

    public EnemyLaserBeam enemyLaserBeam;
    public AvoidPlayer avoidPlayer;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("DodgeStage_20: Entering dodge stage");
        enemyLaserBeam = brain.GetComponent<EnemyLaserBeam>();
        avoidPlayer = brain.GetComponent<AvoidPlayer>();
    }

    public override void Update()
    {
        if (avoidPlayer.isDodging || avoidPlayer.isRetreating)
        {
            if (avoidPlayer.retreatNode != null)
            {
                Vector2 dir = (avoidPlayer.retreatNode.worldPosition - (Vector2)avoidPlayer.transform.position).normalized;
                if(brain.EnemyStateController.canMove)
                {
                    // Move towards the retreat node
                    brain.EnemySteering.MoveInDirection(dir, 3f);
                }

                float dist = Vector2.Distance(avoidPlayer.transform.position, avoidPlayer.retreatNode.worldPosition);
                if (dist < avoidPlayer.nodeStopDistance)
                {
                    if (!avoidPlayer.waitingToShoot)
                    {
                        avoidPlayer.waitingToShoot = true;
                        avoidPlayer.waitAtRetreatUntil = Time.time + avoidPlayer.cooldownBeforeNextShot;
                    }

                    if (Time.time >= avoidPlayer.waitAtRetreatUntil)
                    {
                        if (brain.EnemyVision.CanSeePlayer &&
                            avoidPlayer.gridManager.HasLineOfSight(avoidPlayer.transform.position, avoidPlayer.player.position))
                        {
                            avoidPlayer.TryShoot();
                        }
                        else if (!brain.EnemyVision.CanSeePlayer && brain.EnemyVision.lastSeenPosition.HasValue)
                        {
                            brain.EnemySteering.StopMoving();
                            brain.ChangeState(new LastSeenStage_20(brain));
                        }
                    }
                }
            }
        }

        if (brain.EnemyVision.targetDetected.position != null)
        {
            float distToPlayer = Vector2.Distance(brain.transform.position, brain.EnemyVision.targetDetected.position);


            if (!avoidPlayer.isDodging && distToPlayer <= avoidPlayer.avoidRadius && Time.time >= avoidPlayer.nextShootTime)
            {
                avoidPlayer.isDodging = true;
                avoidPlayer.waitingToShoot = false;
                avoidPlayer.retreatNode = null;
                avoidPlayer.ChooseCurvedRetreatDirection();
                avoidPlayer.nextShootTime = Time.time + avoidPlayer.cooldownAfterDodge;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        // Here you can add any cleanup code for the dodge stage
        Debug.Log("DodgeStage_20: Exiting dodge stage");
    }
}
