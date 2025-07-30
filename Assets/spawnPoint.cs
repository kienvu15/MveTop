using UnityEngine;

public class spawnPoint : MonoBehaviour
{
    public bool hasSpawned = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}
