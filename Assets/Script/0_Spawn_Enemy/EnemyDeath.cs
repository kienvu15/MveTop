using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    private IEnemySpawner spawner;
    public int scoreValue = 100; // mặc định cộng 100 điểm khi chết

    public void SetupSpawner(IEnemySpawner controller)
    {
        spawner = controller;
    }

    public void Die()
    {
        if (spawner != null)
            spawner.OnEnemyDied(transform);
        else
            Debug.LogWarning("[EnemyDeath] Spawner NULL!");

        // Cộng điểm tại đây
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(scoreValue);

        Destroy(gameObject);
    }
}
