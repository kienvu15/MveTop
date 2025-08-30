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
            spawner.OnEnemyDied(father.transform);

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(scoreValue);

        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.Clear();          // Xóa hết vết đang render
            trail.enabled = false;  // Tắt trail ngay lập tức
        }

        Destroy(father);
    }


}
