using System.Collections.Generic;
using UnityEngine;

public class LOSNode
{
    public Vector2 position;
    public Color color;
    public List<LOSNode> neighbors = new List<LOSNode>();

    public LOSNode(Vector2 pos)
    {
        position = pos;
        color = Color.gray;
    }
}