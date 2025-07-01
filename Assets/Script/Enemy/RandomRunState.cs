using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class RandomRunState : IEnemyState
{
    public EnemyStateMachine enemy;
    private Coroutine runCoroutine;

    public Vector2 ArrowDirection = Vector2.right;
    [Header("Deceptive Movement")]
    public float directionChangeInterval = 1.5f;
    public int directionChangesPerCycle = 3;
    public void Enter(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
        runCoroutine = enemy.StartCoroutine(RandomRun());
    }

    public void Update() { }

    public void FixedUpdate()
    {
        
    }

    public void Exit()
    {
        
    }

    private IEnumerator RandomRun()
    {
        for (int i = 0; i < directionChangesPerCycle; i++)
        {
            float angle = Random.Range(0f, 360f);
            ArrowDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
            yield return new WaitForSeconds(directionChangeInterval);
        }
    }

}