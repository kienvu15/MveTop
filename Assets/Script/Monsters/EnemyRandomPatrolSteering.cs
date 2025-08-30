using UnityEngine;

public class EnemyRandomPatrolSteering : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public Vector2 waitTimeRange = new Vector2(1f, 2f);
    public Vector2 moveTimeRange = new Vector2(1f, 3f);

    private EnemySteering steering;
    private float timer = 0f;
    private Vector2 currentPatrolDir;

    private enum State { Moving, Waiting }
    private State currentState;

    void Start()
    {
        steering = GetComponent<EnemySteering>();
        //steering.useFlip = false;
        DecideNextAction();
    }

    void Update()
    {
        // gọi điều kiện tuần tra
        PatrolCondition();
    }

    public void PatrolCondition()
    {
        timer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Moving:
                steering.MoveInDirection(currentPatrolDir, moveSpeed);
                //HandleFlip(currentPatrolDir);
                if (timer <= 0f)
                    DecideNextAction();
                break;

            case State.Waiting:
                steering.StopMoving();
                if (timer <= 0f)
                    DecideNextAction();
                break;
        }
    }

    void DecideNextAction()
    {
        bool shouldWait = Random.value < 0.5f;  // 50% cơ hội dừng lại

        if (shouldWait)
        {
            StartWaiting();
        }
        else
        {
            StartMoving();
        }
    }

    void StartMoving()
    {
        currentPatrolDir = Random.insideUnitCircle.normalized;
        timer = Random.Range(moveTimeRange.x, moveTimeRange.y);
        currentState = State.Moving;
    }

    void StartWaiting()
    {
        timer = Random.Range(waitTimeRange.x, waitTimeRange.y);
        currentState = State.Waiting;
    }

    public void StopPatrol(bool stopMovement = true)
    {
        // Ngừng hành vi tuần tra
        currentState = State.Waiting;  // Hoặc một state an toàn, không di chuyển
        timer = 0f;

        if (stopMovement)
            steering.StopMoving();

        enabled = false;  // Ngắt Update
    }

    // 🔹 Hàm flip theo hướng di chuyển
    //void HandleFlip(Vector2 dir)
    //{
    //    if (dir.x > 0.05f)
    //    {
    //        transform.localScale = new Vector3(1f, 1f, 1f);
    //    }
    //    else if (dir.x < -0.05f)
    //    {
    //        transform.localScale = new Vector3(-1f, 1f, 1f);
    //    }
    //}
}
