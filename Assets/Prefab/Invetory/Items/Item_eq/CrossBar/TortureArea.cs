using System.Collections.Generic;
using UnityEngine;

public class TortureArea : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private float damageInterval = 1f;

    private List<EnemyStats> enemiesInGas = new List<EnemyStats>();

    void Start()
    {
        InvokeRepeating(nameof(DealDamage), 0f, damageInterval);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInTargetLayer(collision))
        {
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
            if (enemyStats != null && !enemiesInGas.Contains(enemyStats))
            {
                enemiesInGas.Add(enemyStats);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsInTargetLayer(collision))
        {
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
            if (enemyStats != null && enemiesInGas.Contains(enemyStats))
            {
                enemiesInGas.Remove(enemyStats);
            }
        }
    }

    private void DealDamage()
    {
        for (int i = enemiesInGas.Count - 1; i >= 0; i--)
        {
            EnemyStats enemy = enemiesInGas[i];
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                enemy.TakeDamageNoRecoil(0.5f);
            }
            else
            {
                enemiesInGas.RemoveAt(i); // loại bỏ enemy chết
            }
        }
    }

    private bool IsInTargetLayer(Collider2D collider)
    {
        return ((1 << collider.gameObject.layer) & targetLayers) != 0;
    }

    public void DestroyOnlastFrame()
    {
        Destroy(gameObject);
    }
}
