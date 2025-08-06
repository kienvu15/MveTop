using TMPro;
using UnityEngine;
using static EnemySteering;

public class DecisionStage_06 : EnemyState
{
    public DecisionStage_06(EnemyBrain brain) : base(brain) { }

    private bool canCurvedMove = true;

    private bool hasStartedWobble = false;
    private bool isWobblingNow = false;
    private bool isDritfting = false;
    private bool hasEnteredCloseRange = false;
    public bool Onoveride = false;

    private float stateTimer = 0f;
    private float wobbleTimer = 0f;
    private float driftTimer = 0f;

    private float randomDuration;
    private float randomDistance;

    private float randChoice;
    private float randChoice2;

    private BulletSwapm bulletSwapm;
    private RangedEnemyController rangedEnemyController;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("DecisionStage_06");
        bulletSwapm = brain.GetComponent<BulletSwapm>();
        rangedEnemyController = brain.GetComponent<RangedEnemyController>();
        bulletSwapm.OnShotBulletFinished += FinishStage;

        randomDuration = Random.Range(1.3f, 3f);


        randomDistance = Random.Range(4f, 6f);

        randChoice = Random.value;
        randChoice2 = Random.value;

        stateTimer = 0f;
        wobbleTimer = 0f;
        driftTimer = 0f;

        hasStartedWobble = false;

        canCurvedMove = true;

        isWobblingNow = false;
        isDritfting = false;
        hasEnteredCloseRange = false;

        bulletSwapm.shootCooldown = 0f;
    }

    // Update is called once per frame
    public override void Update()
    {
        if (brain.EnemyVision.lastSeenPosition.HasValue)
        {
            Onoveride = true;
            brain.EnemySteering.MoveTo(brain.EnemyVision.lastSeenPosition.Value, 1.2f);
        }

        if (brain.EnemyVision.CanSeePlayer == true)
        {
            Onoveride = false;
            //if (randChoice < 0.5f)
            //{

                Debug.Log("DecisionStage_06: 50%aa - Shot");

                // Move To Player
                if (!brain.EnemySteering.hasChosenCurve)
                {
                    brain.EnemySteering.chosenCurveMode = (CurveMode)UnityEngine.Random.Range(0, 3);
                    brain.EnemySteering.hasChosenCurve = true;

                    if (brain.EnemySteering.chosenCurveMode == CurveMode.LoopBack)
                        brain.EnemySteering.SetRandomLoopbackOffset();
                }

                if (canCurvedMove)
                {
                    Debug.Log("Move to player");
                    brain.EnemySteering.MoveToWithBendSmart(brain.EnemyVision.targetDetected.position, brain.EnemySteering.chosenCurveMode, 1.3f);
                }


                if (hasEnteredCloseRange == false && brain.EnemyVision.distance < randomDistance)
                {
                    
                    hasEnteredCloseRange = true;
                    canCurvedMove = false;

                    //
                    if (randChoice2 < 0.5f)
                    {
                        Debug.Log("DecisionStage_06: StartWobbleInPlace");
                    if (isDritfting == false)
                    {
                        isDritfting = true;
                        driftTimer = 0f;
                        rangedEnemyController.DritDec();
                        bulletSwapm.ShotCondition();
                    }
                }
                    //
                    else
                    {
                        Debug.Log("DecisionStage_06: DritDec");
                        if (isDritfting == false)
                        {
                            isDritfting = true;
                            driftTimer = 0f;
                            rangedEnemyController.DritDec();
                            bulletSwapm.ShotCondition();
                        }
                    }
                }

                if (hasEnteredCloseRange)
                {

                    if (randChoice2 < 0.5f)
                    {
                    if (isDritfting == true)
                    {
                        driftTimer += Time.deltaTime;
                        Debug.Log("Time Dir");
                        if (driftTimer >= randomDuration)
                        {
                            isDritfting = false;
                            hasEnteredCloseRange = false;
                            driftTimer = 0f;
                            rangedEnemyController.StopDritDec();
                            
                            brain.ChangeState(new Move_State(brain));
                        }
                    }
                }

                    else if (randChoice2 > 0.5f)
                    {
                        if (isDritfting == true)
                        {
                            driftTimer += Time.deltaTime;
                            Debug.Log("Time Dir");
                            if (driftTimer >= randomDuration)
                            {
                                isDritfting = false;
                                hasEnteredCloseRange = false;
                                driftTimer = 0f;
                                rangedEnemyController.StopDritDec();
                            
                            brain.ChangeState(new Move_State(brain));
                            }
                        }
                    }
                }



            }

            //if (randChoice > 0.5f)
            //{
            //    Debug.Log("DecisionStage_06: 50% - Shot");

            //    // Move To Player
            //    if (!brain.EnemySteering.hasChosenCurve)
            //    {
            //        brain.EnemySteering.chosenCurveMode = (CurveMode)UnityEngine.Random.Range(0, 3);
            //        brain.EnemySteering.hasChosenCurve = true;

            //        if (brain.EnemySteering.chosenCurveMode == CurveMode.LoopBack)
            //            brain.EnemySteering.SetRandomLoopbackOffset();
            //    }
            //    if (canCurvedMove)
            //    {
            //        Debug.Log("Move to player");
            //        brain.EnemySteering.MoveToWithBendSmart(brain.EnemyVision.targetDetected.position, brain.EnemySteering.chosenCurveMode, 1.3f);
            //    }


            //    if (brain.EnemyVision.distance < randomDistance)
            //    {
            //        Debug.Log("DecisionStage_06: 50%aa - Shot - Distance < randomDistance");
            //        canCurvedMove = false;

            //        if (randChoice2 < 0.5f)
            //        {

            //            if (!brain.EnemySteering.IsWobbling && !isWobblingNow)
            //            {
            //                isWobblingNow = true;
            //                wobbleTimer = 0f;
            //                brain.EnemySteering.StartWobbleInPlace(duration: randomDuration, radius: 2f, speed: 1.2f);
            //                bulletSwapm.ShotCondition();
            //            }
            //            else
            //            {
            //                wobbleTimer += Time.deltaTime;
            //                if (wobbleTimer >= randomDuration)
            //                {
            //                    isWobblingNow = false;
            //                    wobbleTimer = 0f;
            //                    brain.ChangeState(new Move_State(brain, nextStage));
            //                }
            //            }
            //        }
            //        else
            //        {

            //            if (isDritfting == false)
            //            {
            //                isDritfting = true;
            //                driftTimer = 0f;
            //                rangedEnemyController.DritDec();
            //            }
            //            else
            //            {
            //                driftTimer += Time.deltaTime;
            //                if (driftTimer <= randomDuration)
            //                {
            //                    bulletSwapm.ShotCondition();
            //                    isDritfting = false;
            //                    driftTimer = 0f;
            //                }
            //                else
            //                {
            //                    brain.ChangeState(new Move_State(brain, nextStage));
            //                }
            //            }
            //        }
            //    }

            //    else
            //    {

            //    }
            //}
        

    }
   
    public override void Exit()
    {
        bulletSwapm.OnShotBulletFinished -= FinishStage;
    }
    void FinishStage() => IsFinished = true;
}
