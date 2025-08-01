using UnityEngine;

public class EnganeStage_03 : EnemyState
{
    public EnganeStage_03(EnemyBrain brain) : base(brain) { }

    private EnemyAttackController enemyAttackController;
    private Retread retread;

    private float stateTimer;
    private float stateDuration;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("EnganeStage_03: Entering Engane stage");
        enemyAttackController = brain.GetComponent<EnemyAttackController>();
        retread = brain.GetComponent<Retread>();

        stateTimer = 0f;
        stateDuration = Random.Range(1f, 1.2f); // Random duration for the Engane stage
    }

    public override void Update()
    {
        brain.EnemySteering.MoveTo(brain.EnemyVision.targetDetected.position, 1.5f);
        if(brain.EnemyVision.distance < 2f)
        {
            brain.EnemySteering.StopMoving();
            enemyAttackController.TryPerformAttack();
        }

        if(enemyAttackController.hasAttacked == true)
        {
            Debug.Log("EnganeStage_03: Attack performed, switching to Decision stage");
            retread.RetreatIfCloseTo(brain.EnemyVision.targetDetected, retreatThreshold: 3f, retreatDistance: 2.5f, retreatSpeed: 6f);

            stateTimer += Time.deltaTime;
            if (stateTimer >= stateDuration)
            {
                Debug.Log("EnganeStage_03: State duration reached, switching to Decision stage");
                brain.ChangeState(new DecisionStage_03(brain));
            }
        }

    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("EnganeStage_03: Exiting Engane stage");
        
    }

}
