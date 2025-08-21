using System.Collections.Generic;
using UnityEngine;

public class EndPointValidator : MonoBehaviour
{
    // Trả về list các marker invalid để xử lý
    public List<EndPointMarker> CheckIntersectionAndReturnInactiveMarkers()
    {
        var markers = new List<EndPointMarker>(EndPointMarker.AllMarkers);
        var invalidMarkers = new List<EndPointMarker>();

        for (int i = 0; i < markers.Count; i++)
        {
            var m1 = markers[i];
            if (m1 == null || !m1.gameObject.activeInHierarchy) continue;

            for (int j = i + 1; j < markers.Count; j++)
            {
                var m2 = markers[j];
                if (m2 == null || !m2.gameObject.activeInHierarchy) continue;

                // Tối ưu: Skip nếu quá xa (giả sử grid size ~10 units)
                if (Vector2.Distance(m1.GetStart(), m2.GetStart()) > 20f) continue;

                if (LineSegmentsIntersect(m1.GetStart(), m1.GetEnd(), m2.GetStart(), m2.GetEnd()))
                {
                    Debug.DrawLine(m1.GetStart(), m1.GetEnd(), Color.green, 0.2f);
                    Debug.DrawLine(m2.GetStart(), m2.GetEnd(), Color.green, 0.2f);
                    Debug.Log($"Intersection between {m1.name} and {m2.name}");

                    // Invalid cái mới hơn (spawnTime lớn hơn)
                    EndPointMarker toInvalid = (m1.spawnTime > m2.spawnTime) ? m1 : m2;
                    toInvalid.isValid = false;
                    invalidMarkers.Add(toInvalid);

                    // Debug invalid
                    Debug.DrawLine(toInvalid.GetStart(), toInvalid.GetEnd(), Color.red, 1f);
                }
            }
        }

        return invalidMarkers;  // Trả về để Generator xử lý
    }

    // Thêm: Reset all markers valid
    public void ResetMarkers()
    {
        foreach (var marker in EndPointMarker.AllMarkers)
        {
            if (marker != null)
                marker.isValid = true;
        }
    }

    bool LineSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
    {
        float o1 = Orientation(p1, p2, q1);
        float o2 = Orientation(p1, p2, q2);
        float o3 = Orientation(q1, q2, p1);
        float o4 = Orientation(q1, q2, p2);

        if (o1 != o2 && o3 != o4)
            return true;

        // Thêm: Xử lý colinear overlap (nếu cần detect chồng lấp)
        if (o1 == 0 && OnSegment(p1, q1, p2)) return true;
        if (o2 == 0 && OnSegment(p1, q2, p2)) return true;
        if (o3 == 0 && OnSegment(q1, p1, q2)) return true;
        if (o4 == 0 && OnSegment(q1, p2, q2)) return true;

        return false;
    }

    bool OnSegment(Vector2 p, Vector2 q, Vector2 r)
    {
        if (q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) &&
            q.y <= Mathf.Max(p.y, r.y) && q.y >= Mathf.Min(p.y, r.y))
            return true;
        return false;
    }

    float Orientation(Vector2 a, Vector2 b, Vector2 c)
    {
        float val = (b.y - a.y) * (c.x - b.x) - (b.x - a.x) * (c.y - b.y);
        if (Mathf.Abs(val) < 0.001f) return 0;  // Tăng epsilon nhẹ
        return (val > 0) ? 1 : 2;
    }
}