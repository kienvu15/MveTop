using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    [SerializeField] public IEnemySpawner spawner;
    public int scoreValue = 100; // mặc định cộng 100 điểm khi chết
    public GameObject father;

    public void SetupSpawner(IEnemySpawner controller)
    {
        spawner = controller;
    }

    public void Die()
    {
        if (spawner != null)
        {
            Debug.Log("[EnemyDeath] Gọi OnEnemyDied cho " + gameObject.name);
            spawner.OnEnemyDied(father.transform);
        }
        else
            Debug.LogWarning("[EnemyDeath] Spawner NULL!");

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(scoreValue);

        Destroy(father);
    }

}
