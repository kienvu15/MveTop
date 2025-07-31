using System.Collections.Generic;
using UnityEngine;

public class EndPointValidator : MonoBehaviour
{
    void Update()
    {
        CheckIntersectionAndReturnInactiveMarkers();
    }

    public void CheckIntersectionAndReturnInactiveMarkers()
    {
        var markers = new List<EndPointMarker>(EndPointMarker.AllMarkers);

        for (int i = 0; i < markers.Count; i++)
        {
            var m1 = markers[i];
            if (m1 == null || !m1.gameObject.activeInHierarchy) continue;

            for (int j = i + 1; j < markers.Count; j++)
            {
                var m2 = markers[j];
                if (m2 == null || !m2.gameObject.activeInHierarchy) continue;

                if (LineSegmentsIntersect(m1.GetStart(), m1.GetEnd(), m2.GetStart(), m2.GetEnd()))
                {
                    Debug.DrawLine(m1.GetStart(), m1.GetEnd(), Color.green, 0.2f);
                    Debug.DrawLine(m2.GetStart(), m2.GetEnd(), Color.green, 0.2f);
                    Debug.Log($"Intersection between {m1.name} and {m2.name}");

                    if (m1.transform.GetSiblingIndex() > m2.transform.GetSiblingIndex())
                        m1.isValid = false;
                    else
                        m2.isValid = false;
                }
            }
        }
    }


    // Toán học kiểm tra giao nhau giữa 2 đoạn thẳng
    bool LineSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
    {
        float o1 = Orientation(p1, p2, q1);
        float o2 = Orientation(p1, p2, q2);
        float o3 = Orientation(q1, q2, p1);
        float o4 = Orientation(q1, q2, p2);

        if (o1 != o2 && o3 != o4)
            return true;

        return false;
    }

    float Orientation(Vector2 a, Vector2 b, Vector2 c)
    {
        float val = (b.y - a.y) * (c.x - b.x) - (b.x - a.x) * (c.y - b.y);
        if (Mathf.Abs(val) < 0.0001f) return 0;  // colinear
        return (val > 0) ? 1 : 2; // clockwise or counterclockwise
    }
}