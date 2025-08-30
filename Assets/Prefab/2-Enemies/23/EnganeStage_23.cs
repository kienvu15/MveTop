using UnityEngine;

public class EnganeStage_23 : EnemyState
{
    public EnganeStage_23(EnemyBrain brain) : base(brain) { }

    private EnemyAttackController enemyAttackController;

    private float random;
    private bool canAttack = true;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("EnganeStage_23: Entering Engane stage");
        enemyAttackController = brain.GetComponent<EnemyAttackController>();
        random = Random.value;
    }

    public override void Update()
    {
        if (canAttack == true)
        {
            enemyAttackController.DashAttack();
            if (enemyAttackController.isDashDone == true)
            {
                canAttack = false;
                Debug.Log("Done Dashing");
                brain.ChangeState(new DecisionStage_23(brain));
            }

        }
        
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("EnganeStage_23: Exiting Engane stage");

    }

}
