using System;
using UnityEngine;

public class ArcAround_29 : EnemyState
{
    public ArcAround_29(EnemyBrain brain) : base(brain) { }
    private bool canArc = false;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("ArcAround_29: Entering ArcAround state");
        canArc = false;
    }

    public override void Update()
    {
        if (brain.EnemyVision.CanSeePlayer && brain.EnemyStateController.canMove)
        {
            brain.EnemySteering.MoveTo(brain.EnemyVision.targetDetected.position, 3.8f);
        }
        float distanceToPlayer = Vector2.Distance(brain.transform.position, brain.PlayerTransform.position);
        if(distanceToPlayer < 11f)
        {
            canArc = true;
        }

        if(canArc == true)
        {
            brain.ChangeState(new BulletStage_29(brain));
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("ArcAround_29: Exiting ArcAround state");

    }
}
