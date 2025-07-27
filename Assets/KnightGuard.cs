using UnityEngine;

public class KnightGuard : Item
{

    public GameObject guardPrefab;
    private GameObject spawnedGuard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public override void ApplyEffect(GameObject player)
    {
        if (spawnedGuard == null)
        {
            spawnedGuard = Instantiate(guardPrefab, player.transform.position + Vector3.right * 2f, Quaternion.identity);
            GuardOrbit orbit = spawnedGuard.GetComponent<GuardOrbit>();
            if (orbit != null)
            {
                orbit.target = player.transform;
                orbit.radius = 2f;
                orbit.speed = 1.5f;
            }
        }
    }

    public override void RemoveEffect(GameObject player)
    {
        if (spawnedGuard != null)
        {
            Destroy(spawnedGuard);
        }
    }

}