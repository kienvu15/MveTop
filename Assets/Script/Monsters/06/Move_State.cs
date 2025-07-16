using System;
using UnityEngine;
using UnityEngine.Rendering;
using static EnemySteering;

public class Move_State : EnemyState
{
    public Move_State(EnemyBrain brain) : base(brain)
    {
        
    }

    private float stateTimer = 0f;
    private float stateTimer2 = 0f;
    private bool isToFar = false;
    private float retreadTimer = 0f;

    private float randomDuration;
    private float randomArcDuration;

    private float randomDistance;
    private float randChoice;
    private float randChoice2;

    private bool hasEnteredCloseRange = false;
    private bool hasMovebackThisState = false;
    private bool hasStartedWobble = false;

    private bool canArcAround = true;
    private bool isWobblingNow = false;
    private bool isDritfting = false;
    private float driftTimer = 0f;

    private float wobbleTimer = 0f;
    private float wobbleTimer2 = 0f;
    private float arcAroundTimer = 0f;

    private RangedEnemyController rangedEnemyController;
    private BulletSwapm bulletSwapm;
    private Retread retread;

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Enter Move State");

        rangedEnemyController = brain.GetComponent<RangedEnemyController>();
        bulletSwapm = brain.GetComponent<BulletSwapm>();
        retread = brain.GetComponent<Retread>();

        randChoice = UnityEngine.Random.value;

        hasMovebackThisState = false;
        hasStartedWobble = false;
        canArcAround = true;
        hasEnteredCloseRange = false;
        isDritfting = false;
        isToFar = false;

        randomDuration = UnityEngine.Random.Range(3f, 3.5f);
        randomArcDuration = UnityEngine.Random.Range(3.5f, 4.3f);

        randomDistance = UnityEngine.Random.Range(3f, 5f);

        stateTimer = 0f;
        wobbleTimer = 0f;
        wobbleTimer2 = 0f;
    arcAroundTimer = 0f;
        driftTimer = 0f;
        retreadTimer = 0f;
        stateTimer2 = 0f;

    randChoice = UnityEngine.Random.value;
        randChoice2 = UnityEngine.Random.value;
    }

    public override void Update()
    {

        if(randChoice < 0.5f)
        {
            if (isDritfting == false)
            {
                isDritfting = true;
                driftTimer = 0f;
                rangedEnemyController.DritDec();
                bulletSwapm.ShotCondition();
            }
            else
            {
                driftTimer += Time.deltaTime;
                if (driftTimer > 6f)
                {
                    isWobblingNow = false;
                    rangedEnemyController.StopDritDec();
                    brain.ChangeState(new DecisionStage_06(brain));
                }
            }
        }
        else
        {

            if (isDritfting == false)
            {
                isDritfting = true;
                driftTimer = 0f;
                rangedEnemyController.DritDec();
                bulletSwapm.ShotCondition();
            }
            else
            {
                driftTimer += Time.deltaTime;
                if (driftTimer > 6f)
                {
                    isWobblingNow = false;
                    rangedEnemyController.StopDritDec();
                    brain.ChangeState(new DecisionStage_06(brain));
                }
            }


            //if (!brain.EnemySteering.IsWobbling && !isWobblingNow)
            //{
            //    isWobblingNow = true;
            //    wobbleTimer = 0f;
            //    Debug.Log("Enter Move State 02 -- StartWobbleInPlace");
            //    brain.EnemySteering.StartWobbleInPlace(duration: 30f, radius: 2f, speed: 1.3f);
            //}
            //else
            //{
            //    wobbleTimer += Time.deltaTime;
            //    if (wobbleTimer > randomDuration)
            //    {
            //        isWobblingNow = false;
            //        brain.EnemySteering.StopWobble();
            //        brain.ChangeState(new DecisionStage_06(brain));
            //    }
            //}

            //else
            //{
            //    wobbleTimer += Time.deltaTime;
            //    if (wobbleTimer > 7f)
            //    {
            //        isWobblingNow = false;
            //        brain.EnemySteering.StopWobble();

            //        brain.ChangeState(new DecisionStage_06(brain));
            //    }
            //}

        }
        
        //if(hasMovebackThisState == true)
        //{
        //    stateTimer += Time.deltaTime;
        //    if (stateTimer > randomDuration)
        //    {
        //        Debug.Log("Retreat done");
        //        if (!brain.EnemySteering.IsWobbling && !isWobblingNow)
        //        {
        //            isWobblingNow = true;
        //            wobbleTimer = 0f;
        //            Debug.Log("Enter Move State -- StartWobbleInPlace");
        //            brain.EnemySteering.StartWobbleInPlace(duration: 30f, radius: 2f, speed: 1f);
        //            bulletSwapm.ShotCondition();
        //        }
        //        else
        //        {
        //            wobbleTimer += Time.deltaTime;
        //            if (wobbleTimer > 6f)
        //            {
        //                Debug.Log("Enter Move State -- done");
        //                brain.EnemySteering.StopWobble();
        //                isWobblingNow = false;
        //                brain.ChangeState(new DecisionStage_06(brain));
        //            }
        //        }
        //    }
        }

        //if(brain.EnemyVision.distance < 8f)
        //{

        //    if(randChoice < 0.5f)
        //    {
        //        Debug.Log("ru lui 1");
        //        if (hasMovebackThisState == false)
        //        {
        //            hasMovebackThisState = true;
        //            retread.RetreatIfCloseTo(brain.EnemyVision.targetDetected);
        //        }
        //        else
        //        {
        //            stateTimer += Time.deltaTime;
        //            if (stateTimer > randomDuration)
        //            {

        //                Debug.Log("retread done");
        //                if (!brain.EnemySteering.IsWobbling && !isWobblingNow)
        //                {
        //                    isWobblingNow = true;
        //                    wobbleTimer = 0f;
        //                    Debug.Log("Enter Move State -- StartWobbleInPlace");
        //                    brain.EnemySteering.StartWobbleInPlace(duration: 30f, radius: 2f, speed: 1f);
        //                }
        //                else
        //                {
        //                    wobbleTimer += Time.deltaTime;
        //                    if(wobbleTimer > 10f)
        //                    {
        //                        Debug.Log("Enter Move State -- done");
        //                        brain.EnemySteering.StopWobble();
        //                        isWobblingNow = false;
        //                        brain.ChangeState(new DecisionStage_06(brain));
        //                    }
        //                }

        //            }
        //        }
        //    }

        //    else
        //    {
        //        Debug.Log("ru lui 2");
        //        if (hasMovebackThisState == false)
        //        {
        //            hasMovebackThisState = true;
        //            retread.RetreatCondition();

        //        }
        //        else
        //        {
        //            stateTimer += Time.deltaTime;
        //            if (stateTimer > randomDuration)
        //            {
        //                Debug.Log("retread done");
        //                if (!brain.EnemySteering.IsWobbling && !isWobblingNow)
        //                {
        //                    isWobblingNow = true;
        //                    wobbleTimer = 0f;
        //                    Debug.Log("Enter Move State2 -- StartWobbleInPlace");
        //                    brain.EnemySteering.StartWobbleInPlace(duration: 30f, radius: 2f, speed: 1f);
        //                }
        //                else
        //                {
        //                    wobbleTimer += Time.deltaTime;
        //                    if (wobbleTimer > 10f)
        //                    {
        //                        Debug.Log("Enter Move State -- S222tartWobbleInPlace");
        //                        brain.EnemySteering.StopWobble();
        //                        isWobblingNow = false;
        //                        brain.ChangeState(new DecisionStage_06(brain));
        //                    }
        //                }

        //            }
        //        }
        //    }

        //}

        //else
        //{
        //    if (isDritfting == false)
        //    {
        //        isDritfting = true;
        //        driftTimer = 0f;
        //        rangedEnemyController.DritDec();
        //        bulletSwapm.ShotCondition();
        //    }
        //    else
        //    {
        //        driftTimer += Time.deltaTime;
        //        Debug.Log("Time Dir");
        //        if (driftTimer >= randomDuration)
        //        {
        //            isDritfting = false;
        //            hasEnteredCloseRange = false;
        //            driftTimer = 0f;
        //            rangedEnemyController.StopDritDec();
        //            Debug.Log("Het cuu");
        //            if (brain.EnemyVision.distance > UnityEngine.Random.Range(4.8f, 5f))
        //            {
        //                isToFar = true;
        //            }
        //        }
        //    }
        //}

        ///// After isWobblingNow
        //if (isWobblingNow == true)
        //{
        //    Debug.Log("stop moving");
        //    wobbleTimer += Time.deltaTime;
        //    if (wobbleTimer > randomDuration)
        //    {
        //        brain.EnemySteering.StopWobble();

        //        if (canArcAround)
        //        {
        //            Debug.Log("Arc aroung");
        //            rangedEnemyController.ArcAroundPlayer();

        //            arcAroundTimer += Time.deltaTime;
        //            if (arcAroundTimer > randomDuration)
        //            {
        //                Debug.Log("Nngung Arc, bawn");
        //                Debug.Log("Shoot");
        //                bulletSwapm.ShotCondition();
        //                canArcAround = false;
        //                isWobblingNow = false;
        //                hasMovebackThisState = false;
        //            }
        //        }
        //    }
        //}

        //if (canArcAround == false)
        //{
        //    if (hasMovebackThisState == false)
        //    {
        //        Debug.Log("Arc done - Retreat");
        //        hasMovebackThisState = true;
        //        retread.RetreatCondition();

        //    }
        //    else
        //    {
        //        stateTimer2 += Time.deltaTime;
        //        if (stateTimer2 > randomDuration)
        //        {
        //            if (randChoice2 < 0.5f)
        //            {
        //                Debug.Log("50-1");
        //                if (!brain.EnemySteering.IsWobbling && !isWobblingNow)
        //                {
        //                    Debug.Log("Lopp taij cjo");
        //                    isWobblingNow = true;
        //                    wobbleTimer = 0f;
        //                    brain.EnemySteering.StartWobbleInPlace(duration: randomDuration, radius: 2f, speed: 1.2f);
        //                }
        //                else
        //                {
        //                    wobbleTimer2 += Time.deltaTime;
        //                    if (wobbleTimer2 >= randomDuration)
        //                    {
        //                        isWobblingNow = false;
        //                        hasEnteredCloseRange = false;
        //                        Debug.Log("vef decision");
        //                        brain.ChangeState(new DecisionStage_06(brain));
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            Debug.Log("vef decision");
        //            brain.ChangeState(new DecisionStage_06(brain));
        //        }


        //    }
        //}

        //else
        //{
        //    retreadTimer += Time.deltaTime;
        //    if (retreadTimer > randomDuration)
        //    {
        //        if (isDritfting == false)
        //        {
        //            isDritfting = true;
        //            driftTimer = 0f;
        //            rangedEnemyController.DritDec();
        //        }
        //        else
        //        {
        //            driftTimer += Time.deltaTime;
        //            Debug.Log("Time Dir");
        //            if (driftTimer >= randomDuration)
        //            {
        //                brain.EnemySteering.MoveTo(brain.EnemyVision.targetDetected.position, 1.5f);
        //            }
        //        }
        //    }
        //}





        //if(isToFar == true)
        //{
        //    brain.EnemySteering.MoveTo(brain.EnemyVision.targetDetected.position, 1.5f);
        //    if(brain.EnemyVision.distance < 5f)
        //    {
        //        brain.EnemySteering.StartWobbleInPlace(duration: 30f, radius: 2f, speed: 1f);
        //    }
        //}






        //if (!brain.EnemySteering.IsWobbling && !isWobblingNow)
        //{
        //    isWobblingNow = true;
        //    wobbleTimer = 0f;
        //    brain.EnemySteering.StartWobbleInPlace(duration: randomDuration, radius: 2f, speed: 1.2f);
        //    bulletSwapm.ShotCondition();
        //}

        //if (brain.EnemyVision.distance > 4.1f)
        //{
        //    isWobblingNow = false;
        //    wobbleTimer = 0f;
        //    if (canArcAround) 
        //    {
        //        rangedEnemyController.ArcAroundPlayer();
        //    }

        //    stateTimer += Time.deltaTime;
        //    if (stateTimer <= randomDuration)
        //    {
        //        bulletSwapm.ShotCondition();
        //    }
        //    else 
        //    { 
        //        canArcAround = false;
        //        if (randChoice2 > 0.5f)
        //        {
        //            Debug.Log("Change to Shot Stage");
        //            brain.ChangeState(new DecisionStage_06(brain));
        //        }
        //        else
        //        {
        //            Debug.Log("Change to DirtCon Stage");
        //            brain.ChangeState(new DecisionStage_06(brain));
        //        }
        //    }
        //}

        //else
        //{
        //    hasMovebackThisState = true;
        //    retread.RetreatIfCloseTo(brain.EnemyVision.targetDetected);

        //    Debug.Log("Retread");
        //    //brain.ChangeState(new DecisionStage_06(brain));
        //}


        //if(hasMovebackThisState == false && brain.EnemyVision.distance < 3.2f)
        //{
        //    Debug.Log("Retreat -----");
        //    hasMovebackThisState = true;
        //    //brain.EnemySteering.RetreatIfCloseTo(brain.EnemyVision.targetDetected);
        //}

        //if (brain.EnemyVision.CanSeePlayer)
        //{
        //    if (!brain.EnemySteering.hasChosenCurve)
        //    {
        //        brain.EnemySteering.chosenCurveMode = (CurveMode)UnityEngine.Random.Range(0, 3);
        //        brain.EnemySteering.hasChosenCurve = true;

        //        if (brain.EnemySteering.chosenCurveMode == CurveMode.LoopBack)
        //            brain.EnemySteering.SetRandomLoopbackOffset();
        //    }
        //    Debug.Log("Move to player");
        //    brain.EnemySteering.MoveToWithBendSmart(brain.EnemyVision.targetDetected.position, brain.EnemySteering.chosenCurveMode, 1.3f);

        //    if(brain.EnemyVision.distance < randomDistance)
        //    {
        //        Debug.Log("Vị chí thích hợp bắt đầu dirt");
        //        if (hasStartedWobble == false)
        //        {
        //            hasStartedWobble = true;
        //            wobbleTimer = 0f;
        //            rangedEnemyController.DritDec();
        //        }
        //        else
        //        {
        //            wobbleTimer += Time.deltaTime;
        //            if (wobbleTimer >= randomDuration)
        //            {
        //                Debug.Log("Dirt xong");
        //                rangedEnemyController.StopDritDec();
        //                if (randChoice2 > 0.5f)
        //                {
        //                    brain.ChangeState(new ShotStagetest(brain, new Move_State(brain, null)));
        //                }
        //                else
        //                {
        //                    brain.ChangeState(new DirtCon(brain, new Move_State(brain, null)));
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        brain.ChangeState(new DecisionStage_06(brain, DecisionStage_06.LastActionType.None));
        //    }
        //}

        //else if (brain.EnemyVision.lastSeenPosition.HasValue)
        //{
        //    brain.EnemySteering.MoveTo(brain.EnemyVision.lastSeenPosition.Value, 1.5f);
        //}


    

    public override void Exit()
    {
        brain.EnemySteering.hasChosenCurve = false;
    }

    public void FinishStage() => IsFinished = true;
}
