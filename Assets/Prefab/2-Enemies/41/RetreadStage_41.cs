using UnityEngine;

public class RetreadStage_41 : EnemyState
{
    public RetreadStage_41(EnemyBrain brain) : base(brain) { }

    private EnemyAttackController enemyAttackController;
    private Retread retread;

    private float stateTimer;
    private float waitTime;// Time to wait before starting the retread
    private float stateDuration; // Duration for the retread stage

    private float retreadDistance;
    private float ranChoice;
    private bool hasDashedAgain = false;

    public override void Enter()
    {
        base.Enter();
        Debug.Log("RetreadStage_41: Entering retread stage");

        enemyAttackController = brain.GetComponent<EnemyAttackController>();
        retread = brain.GetComponent<Retread>();

        stateTimer = 0f;
        stateDuration = Random.Range(1f, 1.5f); // Random duration for the retread stage
        retreadDistance = Random.Range(1.2f, 2.5f);
        ranChoice = Random.value;
        hasDashedAgain = false;

        waitTime = 0f;
    }

    public override void Update()
    {
        enemyAttackController.LockAndDash();

        if (enemyAttackController.isDashDone == true)
        {
            if(ranChoice > 0.5f)
            {
                retread.RetreatIfCloseTo(brain.EnemyVision.targetDetected, retreatThreshold: 3f, retreatDistance: retreadDistance, retreatSpeed: 2.8f);

                stateTimer += Time.deltaTime;
                if (stateTimer >= stateDuration)
                {
                    Debug.Log("EnganeStage_03: State duration reached, switching to Decision stage");
                    brain.ChangeState(new ArcAround_41(brain));
                }
            }

            else
            {
                if (!hasDashedAgain)
                {
                    waitTime += Time.deltaTime;

                    if (waitTime >= 1.7f) // Wait for a short time before dashing again
                    {
                        enemyAttackController.isLocking = true;
                        brain.EnemyAttackVision.isAttackLocked = true;
                        enemyAttackController.LockAndDash();
                    }
                    hasDashedAgain = true;
                }

                stateTimer += Time.deltaTime;
                if (stateTimer >= enemyAttackController.dashDuration)
                {
                    Debug.Log("EnganeStage_03: State duration reached, switching to Decision stage");
                    brain.ChangeState(new ArcAround_41(brain));
                }
            }


        }

    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("RetreadStage_41: Exiting retread stage");
        

    }
}
