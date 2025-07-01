using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Attack_44 : IEnemyState
{
    private EnemyStateMachine enemy;


    private bool hasCharged = false;

    public void Enter(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
        hasCharged = false;
        Debug.Log("Attack_44: Enter");
    }

    public void Update()
    {
        if (!hasCharged && enemy.attackRange.isPlayerInRange && enemy.chargeAttack.canCharge)
        {
            enemy.StartCoroutine(enemy.chargeAttack.StartCharge(enemy.vision.PlayerPosition));
            hasCharged = true;
            if (enemy.vision.CanSeePlayer == true)
            {
                enemy.ChangeState(new MeleeChaseState());
                
            }
        }
    }


    public void Exit()
    {
        Debug.Log("Attack_44: Exit");
        hasCharged = false;
    }

}
