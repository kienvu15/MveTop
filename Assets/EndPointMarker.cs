using UnityEngine;
using System.Collections.Generic;


public class EndPointMarker : MonoBehaviour
{
    public static List<EndPointMarker> AllMarkers = new List<EndPointMarker>();
    public float checkLength = 3f;

    void OnEnable()
    {
        if (!AllMarkers.Contains(this))
            AllMarkers.Add(this);
    }

    void OnDisable()
    {
        AllMarkers.Remove(this);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * checkLength);
       
    }
}
