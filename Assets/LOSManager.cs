using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LOSManager : MonoBehaviour
{
    public static LOSManager Instance { get; private set; }

    public Vector2Int gridSize = new Vector2Int(50, 30);
    public float nodeSpacing = 1f;
    public Transform player;
    public float influenceRadius = 5f;
    public LayerMask obstacleMask;

    private List<LOSNode> nodes = new List<LOSNode>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GenerateNodes();
    }

    private void Update()
    {
        UpdateVisibility();
    }

    private void GenerateNodes()
    {
        nodes.Clear();
        Vector2 offset = new Vector2(
            -gridSize.x * nodeSpacing * 0.5f + nodeSpacing * 0.5f,
            -gridSize.y * nodeSpacing * 0.5f + nodeSpacing * 0.5f);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2 pos = new Vector2(x * nodeSpacing, y * nodeSpacing) + offset;
                nodes.Add(new LOSNode(pos));
            }
        }
    }

    private void UpdateVisibility()
    {
        foreach (var node in nodes)
        {
            float dist = Vector2.Distance(node.position, (Vector2)player.position); // Explicitly cast player.position to Vector2
            if (dist <= influenceRadius)
            {
                Vector2 dir = ((Vector2)player.position - node.position).normalized; // Explicitly cast player.position to Vector2
                RaycastHit2D hit = Physics2D.Raycast(node.position, dir, dist, obstacleMask);
                node.color = (hit.collider == null) ? Color.green : Color.gray;
            }
            else
            {
                node.color = Color.gray;
            }
        }
    }


    public bool HasVisibleNodeNearPosition(Vector2 pos)
    {
        return nodes.Any(n => n.color == Color.green && Vector2.Distance(n.position, pos) <= influenceRadius);
    }

    public List<Vector2> GetVisibleTrailFromNodes()
    {
        return nodes
            .Where(n => n.color == Color.green && Vector2.Distance(n.position, player.position) <= influenceRadius)
            .OrderBy(n => Vector2.Distance(n.position, player.position))
            .Select(n => n.position)
            .ToList();
    }

    public Vector2? GetNodeThatSeesPlayerAndIsReachable(Vector2 enemyPos)
    {
        return nodes
            .Where(n =>
                n.color == Color.green &&
                Vector2.Distance(n.position, player.position) <= influenceRadius &&
                Vector2.Distance(n.position, enemyPos) <= influenceRadius &&
                !Physics2D.Linecast(enemyPos, n.position, obstacleMask))
            .OrderBy(n => Vector2.Distance(n.position, player.position))
            .Select(n => (Vector2?)n.position)
            .FirstOrDefault();
    }

    private void OnDrawGizmos()
    {
        if (nodes == null) return;
        foreach (var node in nodes)
        {
            Gizmos.color = node.color;
            Gizmos.DrawSphere(node.position, 0.1f);
        }
    }
}