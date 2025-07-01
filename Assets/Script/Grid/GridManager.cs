using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

[ExecuteInEditMode]
public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public Tilemap groundTilemap; // Tilemap nền (bắt buộc có tile để coi là node hợp lệ)
    public List<Tilemap> wallTilemaps;   // Tilemap tường

    public Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    public float cellSize = 1f;

    public LayerMask wallLayers;

    private void Awake()
    {
        Instance = this;
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid.Clear();

        BoundsInt bounds = groundTilemap.cellBounds; // Quét đúng vùng có tile thật

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3 worldPos = groundTilemap.CellToWorld(tilePos) + new Vector3(cellSize / 2, cellSize / 2, 0);

                bool hasGround = groundTilemap.HasTile(tilePos); // kiểm tra có nền không
                bool isWall = false;
                foreach (var tilemap in wallTilemaps)
                {
                    if (tilemap.HasTile(tilePos))
                    {
                        isWall = true;
                        break;
                    }
                }
                // kiểm tra có tường không

                if (!hasGround) continue; // bỏ qua node không có nền

                Node node = new Node(gridPos, worldPos, !isWall); // chỉ là walkable nếu không phải tường
                grid[gridPos] = node;
            }
        }
    }

    public Node GetAvoidNode(Vector2 enemyPos, Vector2 playerPos, float avoidRadius, float shotRadius)
    {
        List<Node> candidates = new List<Node>();

        foreach (var node in grid.Values)
        {
            if (!node.isWalkable) continue;

            float distToPlayer = Vector2.Distance(node.worldPosition, playerPos);
            float distToEnemy = Vector2.Distance(node.worldPosition, enemyPos);

            // Phải nằm trong khoảng an toàn
            if (distToPlayer < avoidRadius + 0.5f || distToPlayer > shotRadius) continue;
            if (distToEnemy < 1.5f) continue; // không chọn node quá gần enemy

            // Phải có LOS từ node tới player
            if (!HasLineOfSight(node.worldPosition, playerPos)) continue;

            // Phải có LOS từ enemy tới node
            if (!HasLineOfSight(enemyPos, node.worldPosition)) continue;

            candidates.Add(node);
        }

        if (candidates.Count == 0) return null;

        // Ưu tiên các node xa player hơn
        candidates.Sort((a, b) =>
            Vector2.Distance(b.worldPosition, playerPos).CompareTo(
            Vector2.Distance(a.worldPosition, playerPos)));

        int randIndex = Random.Range(0, Mathf.Min(4, candidates.Count)); // Random trong top 4 tốt nhất
        return candidates[randIndex];
    }

    public Node GetAvoid2Node(Vector2 enemyPos, Vector2 playerPos, float avoidRadius, float visionRadius)
    {
        List<Node> candidates = new List<Node>();

        foreach (var node in grid.Values)
        {
            if (!node.isWalkable) continue;

            float distToPlayer = Vector2.Distance(node.worldPosition, playerPos);
            float distToEnemy = Vector2.Distance(node.worldPosition, enemyPos);

            if (distToPlayer < avoidRadius + 0.5f || distToEnemy < 1.5f) continue;

            // 🟢 NEW: Node phải giữ player trong tầm attackVision
            if (distToPlayer > visionRadius) continue;

            if (!HasLineOfSight(node.worldPosition, playerPos)) continue;
            if (!HasLineOfSight(enemyPos, node.worldPosition)) continue;

            candidates.Add(node);
        }

        if (candidates.Count == 0) return null;

        candidates.Sort((a, b) =>
            Vector2.Distance(b.worldPosition, playerPos).CompareTo(
            Vector2.Distance(a.worldPosition, playerPos)));

        int randIndex = Random.Range(0, Mathf.Min(4, candidates.Count));
        return candidates[randIndex];
    }


    private List<Node> Shuffle(List<Node> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Node temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (grid == null) return;

        // Chỉ hoạt động nếu có object được chọn và là enemy
        Transform selected = Selection.activeTransform;
        if (selected == null || !selected.CompareTag("Enemy")) return;

        Vector3 enemyPos = selected.position;

        foreach (var node in grid.Values)
        {
            if (!node.isWalkable)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                bool canSee = HasLineOfSight(enemyPos, node.worldPosition);
                Gizmos.color = canSee ? Color.green : Color.red;
            }

            Gizmos.DrawSphere(node.worldPosition, 0.1f);
        }
    }
#endif


    public bool HasLineOfSight(Vector2 from, Vector2 to)
    {
        Vector2 dir = to - from;
        float dist = dir.magnitude;

        // Raycast qua tất cả layer trong wallLayers
        RaycastHit2D hit = Physics2D.Raycast(from, dir.normalized, dist, wallLayers);

        return hit.collider == null;
    }


    public Node GetRandomWalkableNode()
    {
        List<Node> walkables = new List<Node>();
        foreach (var node in grid.Values)
        {
            if (node.isWalkable)
                walkables.Add(node);
        }

        if (walkables.Count == 0) return null;
        return walkables[Random.Range(0, walkables.Count)];
    }

    public Node GetRandomWalkableNodeNear(Vector2 position, float radius)
    {
        List<Node> nearby = new List<Node>();
        foreach (var node in grid.Values)
        {
            if (node.isWalkable &&
                Vector2.Distance(position, node.worldPosition) <= radius &&
                HasLineOfSight(position, node.worldPosition)) // Thêm check LOS
            {
                nearby.Add(node);
            }
        }

        if (nearby.Count == 0) return null;
        return nearby[Random.Range(0, nearby.Count)];
    }

    public bool IsInBounds(Vector2 position)
    {
        Vector2Int gridPos = Vector2Int.FloorToInt(position);
        return grid.ContainsKey(gridPos);
    }

}
