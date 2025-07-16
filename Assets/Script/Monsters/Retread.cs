using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemySteering;

public class Retread : MonoBehaviour
{
    private EnemySteering steering;
    public Transform player;
    public bool isRetreating = false;
    public bool isDone = false;
    void Awake()
    {
        steering = GetComponent<EnemySteering>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isDone = false;
    }

    public void RetreatIfCloseTo(Transform player, float retreatThreshold = 3f, float retreatDistance = 2.5f, float retreatSpeed = 2f)
    {
        if (player == null) return; 

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist < retreatThreshold)
        {
            Vector2 dirFromPlayer = (transform.position - player.position).normalized;
            Vector2 retreatTarget = (Vector2)transform.position + dirFromPlayer * retreatDistance;

            steering.MoveTo(retreatTarget, retreatSpeed);
        }
    }

    public void RetreatCondition()
    {
        StartCoroutine(RetreatFromPlayer());
    }

    public IEnumerator RetreatFromPlayer()
    {
        isRetreating = true;
        Vector2 currentPos = transform.position;

        // 1. Hướng rút lui từ Player
        Vector2 retreatDir = -((Vector2)(player.position) - currentPos).normalized;

        Vector2 chosenNode1 = currentPos;

        // Tìm node đầu tiên theo hướng retreat
        for (float dist = 1.5f; dist >= 1f; dist -= 0.25f)
        {
            Vector2 tryPos = currentPos + retreatDir * dist;
            Vector2Int gridPos = Vector2Int.RoundToInt(tryPos);

            if (GridManager.Instance.grid.TryGetValue(gridPos, out Node node) && node.isWalkable)
            {
                chosenNode1 = (Vector2)gridPos;
                break;
            }
        }

        // → Di chuyển 1–2f về hướng node1
        Vector2 dir1 = (chosenNode1 - currentPos).normalized;
        float moveLength1 = UnityEngine.Random.Range(2f, 4f);
        Vector2 midTarget1 = currentPos + dir1 * moveLength1;

        yield return MoveTowardDirection(midTarget1, dir1, moveLength1);

        // =========== CHẶN 2 ==============
        Vector2 currentPos2 = transform.position;
        Vector2Int currentGridPos = Vector2Int.RoundToInt(currentPos2);

        // Tìm các node lân cận node1
        List<Vector2Int> neighbors = new List<Vector2Int>()
    {
        currentGridPos + Vector2Int.up,
        currentGridPos + Vector2Int.down,
        currentGridPos + Vector2Int.left,
        currentGridPos + Vector2Int.right
    };

        List<Vector2> walkableNeighbors = new List<Vector2>();
        foreach (var pos in neighbors)
        {
            if (GridManager.Instance.grid.TryGetValue(pos, out Node node) && node.isWalkable)
            {
                walkableNeighbors.Add((Vector2)pos);
            }
        }

        if (walkableNeighbors.Count > 0)
        {
            Vector2 chosenNode2 = walkableNeighbors[UnityEngine.Random.Range(0, walkableNeighbors.Count)];
            Vector2 dir2 = (chosenNode2 - currentPos2).normalized;
            float moveLength2 = UnityEngine.Random.Range(1f, 2f);
            Vector2 midTarget2 = currentPos2 + dir2 * moveLength2;

            yield return MoveTowardDirection(midTarget2, dir2, moveLength2);
        }

        isDone = true;
        steering.StopMoving();
        isRetreating = false;
    }

    private IEnumerator MoveTowardDirection(Vector2 midTarget, Vector2 direction, float length)
    {
        float speed = UnityEngine.Random.Range(1.3f, 2.5f);
        float duration = length / speed;
        float elapsed = 0f;

        CurveMode mode = UnityEngine.Random.value > 0.5f ? CurveMode.Right : CurveMode.Left;

        while (elapsed < duration)
        {
            steering.MoveToWithBendSmart(midTarget, mode, speed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        steering.StopMoving();
        yield return new WaitForSeconds(0.1f);
    }

    public void StopReytread()
    {
        StopCoroutine(RetreatFromPlayer());
    }

}
