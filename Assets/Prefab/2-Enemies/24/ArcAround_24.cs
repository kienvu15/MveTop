using UnityEngine;

public class ArcAround_24 : EnemyState  
{
    public ArcAround_24(EnemyBrain brain) : base(brain) { }
    private EnemyDashAttack enemyDashAttack;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("ArcAround_24: Entering ArcAround state");
        enemyDashAttack = brain.GetComponent<EnemyDashAttack>();
    }

    public override void Update()
    {
        if (brain.EnemyVision.CanSeePlayer && brain.EnemyStateController.canMove)
        {
            brain.EnemySteering.MoveTo(brain.EnemyVision.targetDetected.position, 3.8f);
        }

        if (brain.EnemyAttackVision.isPlayerInAttackRange)
        {
            enemyDashAttack.Lock();
            enemyDashAttack.ConditionDash();
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("ArcAround_24: Exiting ArcAround state");

    }
}
