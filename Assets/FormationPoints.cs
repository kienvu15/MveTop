using System.Collections.Generic;
using UnityEngine;

public class FormationPoints : MonoBehaviour
{
    [HideInInspector]
    public List<Transform> points = new List<Transform>();

    void Awake()
    {
        // Tự động lấy các điểm con làm formation points
        foreach (Transform child in transform)
        {
            points.Add(child);
        }
    }

    public Transform GetClosestAvailablePoint(Vector3 fromPosition)
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform point in points)
        {
            float dist = Vector3.Distance(fromPosition, point.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = point;
            }
        }

        return closest;
    }

    public Transform GetRandomPoint()
    {
        if (points.Count == 0) return null;
        return points[Random.Range(0, points.Count)];
    }
}
