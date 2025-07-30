using UnityEngine;
using System.Collections.Generic;

public class EndPointValidator : MonoBehaviour
{
    public static EndPointValidator Instance { get; private set; }

    public float checkDistance = 3f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Đã tồn tại một EndPointValidator khác! Sẽ bị thay thế.");
        }
        Instance = this;
    }

    public void Validate()
    {
        List<EndPointMarker> markers = EndPointMarker.AllMarkers;

        for (int i = 0; i < markers.Count; i++)
        {
            for (int j = i + 1; j < markers.Count; j++)
            {
                var a = markers[i];
                var b = markers[j];

                if (DoLinesIntersect(a.transform.position, a.transform.forward, b.transform.position, b.transform.forward, a.checkLength, b.checkLength))
                {
                    Debug.LogWarning($"[EndPointValidator] Giao nhau giữa:\n- A: {a.gameObject.name}\n- B: {b.gameObject.name}");

                    if (b.gameObject.activeSelf)
                    {
                        Debug.Log($"--> Tắt {b.gameObject.name}");
                        b.gameObject.SetActive(false);
                    }
                    else
                    {
                        Debug.Log($"--> {b.gameObject.name} đã tắt sẵn");
                    }
                }
            }
        }
    }


    private bool DoLinesIntersect(Vector3 aStart, Vector3 aDir, Vector3 bStart, Vector3 bDir, float aLen, float bLen)
    {
        Vector3 aEnd = aStart + aDir.normalized * aLen;
        Vector3 bEnd = bStart + bDir.normalized * bLen;

        // Giản lược về 2D nếu dùng trong game top-down (x,z) hoặc platformer (x,y)
        Vector2 a1 = new Vector2(aStart.x, aStart.z);
        Vector2 a2 = new Vector2(aEnd.x, aEnd.z);
        Vector2 b1 = new Vector2(bStart.x, bStart.z);
        Vector2 b2 = new Vector2(bEnd.x, bEnd.z);

        return LinesIntersect(a1, a2, b1, b2);
    }

    private bool LinesIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
    {
        // Check if two line segments (p1-p2 and q1-q2) intersect
        float d = (p2.x - p1.x) * (q2.y - q1.y) - (p2.y - p1.y) * (q2.x - q1.x);
        if (Mathf.Approximately(d, 0)) return false; // Parallel

        float u = ((q1.x - p1.x) * (q2.y - q1.y) - (q1.y - p1.y) * (q2.x - q1.x)) / d;
        float v = ((q1.x - p1.x) * (p2.y - p1.y) - (q1.y - p1.y) * (p2.x - p1.x)) / d;

        return (u >= 0 && u <= 1) && (v >= 0 && v <= 1);
    }
}
