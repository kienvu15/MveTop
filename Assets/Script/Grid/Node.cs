using UnityEngine;

public class Node
{
    public Vector2Int gridPosition;
    public Vector2 worldPosition;
    public bool isWalkable;

    public Node(Vector2Int gridPos, Vector2 worldPos, bool walkable)
    {
        gridPosition = gridPos;
        worldPosition = worldPos;
        isWalkable = walkable;
    }
}
