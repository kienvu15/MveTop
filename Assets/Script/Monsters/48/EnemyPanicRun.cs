//using UnityEngine;
//using System;

//public class EnemyPanicRun : MonoBehaviour
//{
//    [Header("Panic Settings")]
//    public int runTimes = 3;
//    public float runDuration = 1.2f;
//    public float panicSpeed = 3f;
//    public float avoidRadius = 3f;
//    public float shotRadius = 7f;

//    public event Action OnPanicComplete;

//    private EnemySteering steering;
//    private EnemyBrain brain;

//    private int runLeft;
//    private float timer;
//    private Vector2 runDirection;
//    private bool isRunning;

//    private Transform player;
//    private Node targetNode;

//    void Awake()
//    {
//        steering = GetComponent<EnemySteering>();
//        brain = GetComponent<EnemyBrain>();
//    }

//    public void StartPanic()
//    {
//        player = brain.PlayerTransform; // ← lấy từ brain
//        if (player == null)
//        {
//            Debug.LogError("PlayerTransform is null in PanicRun");
//            return;
//        }

//        runLeft = runTimes;
//        isRunning = true;
//        PickTargetNode();
//        timer = 0f;
//    }

//    public void StopPanic()
//    {
//        isRunning = false;
//        steering?.StopMoving();
//    }

//    void Update()
//    {
//        if (!isRunning || player == null) return;

//        timer += Time.deltaTime;

//        if (targetNode != null)
//        {
//            runDirection = (targetNode.worldPosition - (Vector2)transform.position).normalized;
//            Vector2 steerDir = steering.AdjustDirection(runDirection);
//            steering.MoveInDirection(steerDir, panicSpeed);
//        }

//        if (timer >= runDuration)
//        {
//            runLeft--;
//            if (runLeft <= 0)
//            {
//                StopPanic();
//                OnPanicComplete?.Invoke();
//            }
//            else
//            {
//                PickTargetNode();
//                timer = 0f;
//            }
//        }
//    }

//    void PickTargetNode()
//    {
//        if (GridManager.Instance == null)
//        {
//            Debug.LogError("No GridManager found!");
//            return;
//        }

//        targetNode = GridManager.Instance.GetAvoidNode(transform.position, player.position, avoidRadius, shotRadius);

//        if (targetNode == null)
//        {
//            Debug.LogWarning("No valid panic node found, fallback to random");
//            runDirection = UnityEngine.Random.insideUnitCircle.normalized; // Specify UnityEngine.Random explicitly
//        }
//        else
//        {
//            runDirection = (targetNode.worldPosition - (Vector2)transform.position).normalized;
//        }
//    }

//}
