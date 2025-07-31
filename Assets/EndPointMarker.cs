using System.Collections.Generic;
using UnityEngine;

public class EndPointMarker : MonoBehaviour
{
    public static List<EndPointMarker> AllMarkers = new List<EndPointMarker>();

    public float checkLength = 3f;
    public bool isValid = true; 
    public bool inUse = false;

    public LayerMask groundLayer;

    void Awake()
    {
        AllMarkers.Add(this);
        Physics2D.queriesHitTriggers = true; // Cho phép raycast chạm trigger
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
        // Thay đổi màu theo isValid
        Gizmos.color = isValid ? Color.green : Color.red;
        Vector3 worldDir = transform.TransformDirection(Vector2.right) * checkLength;
        Gizmos.DrawLine(transform.position, transform.position + worldDir);

        Gizmos.color = inUse ? Color.red : Color.green;
        Gizmos.DrawLine(GetStart(), GetEnd());
    }

    public Vector2 GetStart() => transform.position;
    public Vector2 GetEnd() => transform.position + transform.TransformDirection(Vector2.right) * checkLength;
}
