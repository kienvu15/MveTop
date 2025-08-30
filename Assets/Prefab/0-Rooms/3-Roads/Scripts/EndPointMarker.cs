using System.Collections.Generic;
using UnityEngine;

public class EndPointMarker : MonoBehaviour
{
    public static List<EndPointMarker> AllMarkers = new List<EndPointMarker>();
    public SimpleDungeonGenerator simpleDungeonGenerator;

    public float checkLength = 3f;
    public bool isValid = true;
    public bool inUse = false;
    public bool isBlock = false; 
    public Collider2D blockerCollider; 

    public LayerMask groundLayer;
    public GameObject Road;
    public bool isWall = false;
    public StartPointMaker startPointMaker;

    public float spawnTime;
    void Awake()
    {
        AllMarkers.Add(this);
        spawnTime = Time.time;  // Thời gian spawn
        Physics2D.queriesHitTriggers = true;
    }

    private void Start()
    {
        simpleDungeonGenerator = FindFirstObjectByType<SimpleDungeonGenerator>();
    }
    void OnEnable()
    {
        if (!AllMarkers.Contains(this))
            AllMarkers.Add(this);
    }

    void OnDisable()
    {
        AllMarkers.Remove(this);
    }

    [SerializeField] private LayerMask targetLayers;

    // ✅ Kiểm tra va chạm với Blocker
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Blocker"))
        {
            isBlock = true;
            blockerCollider = other;
            Debug.Log($"🧱 EndPoint bị Blocker chặn: {other.name}");
        }
        else if (((1 << other.gameObject.layer) & targetLayers) != 0 ) //&& !other.CompareTag("Room"))
        {
            isWall = true;
            if (startPointMaker.isDone == true)
            {
                //Road.SetActive(false);
                Destroy(Road);
            }
            Debug.Log($"🚫 EndPointMarker {name} bị chặn bởi layer {other.gameObject.layer}");
        }

        
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        bool isTargetLayer = ((1 << collision.gameObject.layer) & targetLayers) != 0;
        if (isTargetLayer == true)
        {
            isWall = true;

            if (startPointMaker.isDone == true)
            {
                Destroy(Road);
            }

            Debug.Log($"🚫 EndPointMarker {name} bị chặn bởi layer {collision.gameObject.layer}");
        }

        if (collision.CompareTag("Blocker"))
        {
            if (simpleDungeonGenerator.isGeneratingDone == true)
            {
                GameObject blocker = collision.gameObject.CompareTag("Blocker") ? collision.gameObject : null;
                if (blocker != null)
                {
                    blocker.SetActive(false); // Tắt Blocker khi đã hoàn thành việc tạo đường đi
                    Debug.Log($"🧱 Blocker {blocker.name} đã bị tắt sau khi tạo đường đi.");
                }
            }
        }
    }

    public void CheckIfInUse()
    {
        Vector2 start = GetStart();
        Vector2 end = GetEnd();
        Vector2 dir = (end - start).normalized;
        float dist = Vector2.Distance(start, end);

        RaycastHit2D[] hits = Physics2D.RaycastAll(start, dir, dist, groundLayer);

        Debug.DrawLine(start, end, Color.cyan, 5f);
        Debug.Log($"🔎 CheckIfInUse from {start} to {end} - hits: {hits.Length}");

        inUse = false;

        foreach (var hit in hits)
        {
            Debug.Log($"👉 Hit: {hit.collider?.name}, Tag: {hit.collider?.tag}");
            if (hit.collider != null && hit.collider.CompareTag("Ground"))
            {
                inUse = true;
                Debug.Log($"🚫 Đường bị chặn bởi {hit.collider.name}");
                break;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isValid ? Color.green : Color.red;
        Vector3 worldDir = transform.TransformDirection(Vector2.right) * checkLength;
        Gizmos.DrawLine(transform.position, transform.position + worldDir);

        Gizmos.color = inUse ? Color.red : Color.green;
        Gizmos.DrawLine(GetStart(), GetEnd());
    }

    public Vector2 GetStart() => transform.position;
    public Vector2 GetEnd() => transform.position + transform.TransformDirection(Vector2.right) * checkLength;
}
