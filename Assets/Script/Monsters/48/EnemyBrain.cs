using System;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    EnemyState currentState;

    [HideInInspector] public EnemyVision EnemyVision;
    [HideInInspector] public EnemyAttackVision EnemyAttackVision;
    [HideInInspector] public EnemySteering EnemySteering;

    public Transform PlayerTransform => playerTransform;
    [SerializeField] private Transform playerTransform;

    public Func<EnemyState> GetDecisionStageFunc;
    void Start()
    {
        ChangeState(new IdleState(this));

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        EnemyVision = GetComponent<EnemyVision>();
        EnemySteering = GetComponent<EnemySteering>();
        EnemyAttackVision = GetComponent<EnemyAttackVision>();
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