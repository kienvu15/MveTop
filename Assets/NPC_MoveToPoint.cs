using UnityEngine;

public class NPC_MoveToPoint : MonoBehaviour
{
    public CutsceneTrigger cutsceneTrigger;
    public Transform targetPoint;
    public float moveSpeed = 3f;

    void Awake()
    {
        cutsceneTrigger = FindFirstObjectByType<CutsceneTrigger>();
        if (cutsceneTrigger == null)
        {
            Debug.LogError("CutsceneTrigger not found in the scene.");
        }
    }

    void Update()
    {
        if (targetPoint == null)
        {
            Destroy(gameObject);
            return;
        }

        if( cutsceneTrigger == null || cutsceneTrigger.hasTriggered)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
        }
        

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.05f)
        {
            Destroy(gameObject); // Hoặc gameObject.SetActive(false);
        }
    }
}
