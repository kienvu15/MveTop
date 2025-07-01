using UnityEngine;

public class SteeringLastScenePosition : MonoBehaviour
{
    private EnemyVision EnemyVision;
    private EnemySteering EnemySteering;
    public event System.Action OnSteeringFinished;

    public float customSpeed = 2.3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnemyVision = GetComponent<EnemyVision>();
        EnemySteering = GetComponent<EnemySteering>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(EnemyVision.hasSeenPlayer && !EnemyVision.CanSeePlayer && EnemyVision.lastSeenPosition.HasValue)
        //{
        //    GoToLastSeenPosition();
        //}
    }

    public void GoToLastSeenPosition()
    {
        float distance = Vector2.Distance(transform.position, EnemyVision.lastSeenPosition.Value);
        if (distance > EnemySteering.stopDistanceToLastSeen)
        {
            EnemySteering.MoveTo(EnemyVision.lastSeenPosition.Value, customSpeed);
        }
        else
        {
            EnemySteering.StopMoving();
            OnSteeringFinished?.Invoke();
            // Optional: EnemyVision.ClearLastSeenPosition();
        }
        
    }
}
