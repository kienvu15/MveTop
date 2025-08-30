using UnityEngine;

public class EnganeStage_03 : EnemyState
{
    public EnganeStage_03(EnemyBrain brain) : base(brain) { }

    private EnemyAttackController enemyAttackController;

    private bool canAttack = false;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("EnganeStage_03: Entering Engane stage");
        enemyAttackController = brain.GetComponent<EnemyAttackController>();
    }

    public override void Update()
    {
        if (brain.EnemyStateController.canMove && brain.EnemyVision.targetDetected != null)
        {
            brain.EnemySteering.MoveTo(brain.EnemyVision.targetDetected.position, 3.5f);
        }

        if (brain.EnemyVision.distance < 2.1f)
        {
            canAttack = true;
        }

        if(enemyAttackController.hasAttacked == true)
        {
            brain.ChangeState(new RetreadStage_03(brain));
        }

        if(canAttack == true)
        {
            //brain.EnemySteering.StopMoving();
            enemyAttackController.TryPerformAttack();
        }

        if (brain.EnemyVision.lastSeenPosition != null && brain.EnemyVision.CanSeePlayer == false)
        {
            Vector2 lastSeen = brain.EnemyVision.lastSeenPosition.Value;
            brain.EnemySteering.MoveTo(lastSeen, 1f);

            float dist = Vector2.Distance(brain.transform.position, lastSeen);
            if (dist < 0.5f)
            {
                brain.EnemySteering.StopMoving();
                brain.ChangeState(new PatrolStage_03(brain));
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("EnganeStage_03: Exiting Engane stage");
        
    }

}
