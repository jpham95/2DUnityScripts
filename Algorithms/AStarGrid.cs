using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Algorithms.AStar;

public class AStarGrid : MonoBehaviour
{
    public bool displayGridGizmos;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public LayerMask unwalkableMask;
    private float _nodeDiameter;
    private Node[,] _grid;
    private int _gridSizeX, _gridSizeY;
    private void Awake()
    {
        _nodeDiameter = nodeRadius * 2;
        _gridSizeX = Mathf.RoundToInt(gridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(gridWorldSize.y / _nodeDiameter);
        CreateGrid();
    }
    
    private void CreateGrid()
    {
        _grid = new Node[_gridSizeX, _gridSizeY];
        Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * _nodeDiameter + nodeRadius) + Vector2.up * (y * _nodeDiameter + nodeRadius);
                bool isWalkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask));
                _grid[x, y] = new Node(isWalkable, worldPoint, x, y);
            }
        }
    }

    public int MaxSize
    {
        get { return _gridSizeX * _gridSizeY; }
    }

    public Node NodeFromWorldPoint(Vector3 worldPoint)
    {
        float percentX = (worldPoint.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPoint.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((_gridSizeX-1) * percentX);
        int y = Mathf.RoundToInt((_gridSizeY-1) * percentY);

        return _grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (_grid != null && displayGridGizmos)
        {   
            foreach (Node node in _grid)
            {
                Gizmos.color = (node._isWalkable) ? Color.clear : Color.red;
                Gizmos.color *= new Color(1, 1, 1, .5f);

                Gizmos.DrawCube(node._worldPosition, Vector3.one * (_nodeDiameter - .1f));
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = node._gridX + x;
                int checkY = node._gridY + y;

                if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                {
                    neighbours.Add(_grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }
}