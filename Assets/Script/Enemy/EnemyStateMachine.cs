using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    public IEnemyState currentState;
    public Transform player;
    public Rigidbody2D rb;

    [HideInInspector] public EnemyVision vision;
    [HideInInspector] public EnemySteering steering;
    [HideInInspector] public AttackRange attackRange;
    [HideInInspector] public ChargeAttack chargeAttack;
    [HideInInspector] public RunRandom randomRunState;
    [HideInInspector] public GridManager gridManager;


    [HideInInspector] public EnemyPatrol patrolState;
    [HideInInspector] public EnemyAttackVision attackVision;
    [HideInInspector] public EnemyCombat enemyCombatState;

    void Awake()
    {
        vision = GetComponent<EnemyVision>();
        steering = GetComponent<EnemySteering>();
        randomRunState = GetComponent<RunRandom>();
        chargeAttack = GetComponent<ChargeAttack>();
        attackRange = GetComponent<AttackRange>();

        gridManager = FindFirstObjectByType<GridManager>();
        patrolState = GetComponent<EnemyPatrol>();
        enemyCombatState = GetComponent<EnemyCombat>();
        attackVision = GetComponent<EnemyAttackVision>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(IEnemyState startState)
    {
        vision.targetDetected = player;
        ChangeState(startState);
    }

    public void ChangeState(IEnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter(this);
    }

    void Update()
    {
        currentState?.Update();
    }
}
