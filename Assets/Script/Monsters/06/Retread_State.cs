using UnityEngine;

public class Retread_State : EnemyState
{
    public Retread_State(EnemyBrain brain, EnemyState nextStage) : base(brain, nextStage)
    {
        this.nextStage = nextStage;
    }

    private RangedEnemyController rangedEnemyController;
    private float randomChoice;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("Retread_Stage");
        // Additional initialization code can go here

        randomChoice = Random.value;
        rangedEnemyController = brain.GetComponent<RangedEnemyController>();
    }

    public override void Update()
    {
        if (randomChoice < 0.5f)
        {

        }
        else 
        { 
        
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exiting Retread_Stage");
        // Cleanup code can go here
    }
}
