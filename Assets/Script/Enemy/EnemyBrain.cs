using System;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    EnemyState currentState;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public EnemyVision EnemyVision;
    [HideInInspector] public EnemyAttackVision EnemyAttackVision;
    [HideInInspector] public EnemySteering EnemySteering;

    public Transform PlayerTransform => playerTransform;
    [SerializeField] private Transform playerTransform;

    public Func<EnemyState> GetDecisionStageFunc;

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        rb = GetComponent<Rigidbody2D>();
        EnemyVision = GetComponent<EnemyVision>();
        EnemySteering = GetComponent<EnemySteering>();
        EnemyAttackVision = GetComponent<EnemyAttackVision>();
    }

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (GetDecisionStageFunc != null)
            ChangeState(GetDecisionStageFunc.Invoke());
    }

    void Update()
    {
        currentState?.Update();
    }

    void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }

    public void ChangeState(EnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

}