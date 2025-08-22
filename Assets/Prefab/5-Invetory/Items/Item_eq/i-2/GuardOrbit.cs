using UnityEngine;

public class GuardOrbit : MonoBehaviour
{
    public Transform target;
    public float radius = 2f;
    public float speed = 1.5f;

    private float angle;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("PivotPlayer")?.transform;
    }

    void Update()
    {
        if (target == null) return;

        angle += speed * Time.deltaTime;
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;

        transform.position = target.position + new Vector3(x, y, 0f);
    }
}
