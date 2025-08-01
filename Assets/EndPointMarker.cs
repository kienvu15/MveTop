using System.Collections.Generic;
using UnityEngine;

public class EndPointMarker : MonoBehaviour
{
    public static List<EndPointMarker> AllMarkers = new List<EndPointMarker>();

    public float checkLength = 3f;
    public bool isValid = true;
    public bool inUse = false;
    public bool isBlock = false; // ✅ Mới thêm
    public Collider2D blockerCollider; // ✅ Nếu có va chạm với blocker thì giữ lại tham chiếu

    public LayerMask groundLayer;

    void Awake()
    {
        AllMarkers.Add(this);
        Physics2D.queriesHitTriggers = true;
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

    // ✅ Kiểm tra va chạm với Blocker
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Blocker"))
        {
            isBlock = true;
            blockerCollider = other; // lưu lại collider để tắt sau
            Debug.Log($"🧱 EndPoint bị Blocker chặn: {other.name}");
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
