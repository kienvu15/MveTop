using UnityEngine;

public class Move_RetreadStage_00 : EnemyState
{
    public Move_RetreadStage_00(EnemyBrain brain) : base(brain) { }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private RangedEnemyController rangedEnemyController;
    private bool hasMoved = false;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("Move_RetreadStage_00: Entering move retread stage");
        rangedEnemyController = brain.GetComponent<RangedEnemyController>();
    }
    public override void Update()
    {
        if (!hasMoved)
        {
            rangedEnemyController.MoveRandomDirection(1.5f, 2f);
            hasMoved = true;
        }

        if (rangedEnemyController.moveFinished == true)
        {
            brain.ChangeState(new ArcAroundStage_00(brain));
        }
        
    }
    public override void Exit()
    {
        base.Exit();
        Debug.Log("Move_RetreadStage_00: Exiting move retread stage");
        // Here you can add any cleanup code for the move retread stage
    }
}
