using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
OPEN = priority queue containing START
CLOSED = empty set
add the start node to OPEN

loop
    current = node in OPEN with the lowest f_cost
    remove current from OPEN
    add current to CLOSED

    if current is the target node //path has been found
        return
    
    foreach neighbour of the current node
        if neighbour is not traversable || neighbour is in CLOSED
            skip to the next neighbour
    
        if new path to neighbour is shorter || neighbour is not in OPEN
            set f_cost of neighbour
            set parent of neighbour to current
            if neighbour is not in OPEN
                add neighbour to OPEN
*/
namespace Algorithms.AStar
{
    public class AStarPathfinding : MonoBehaviour
    {
        public Transform seeker, target;
        AStarGrid grid;

        private void Awake()
        {
            grid = GetComponent<AStarGrid>();
        }

        private void Update()
        {
            FindPath(seeker.position, target.position);
        }
        private void FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Node startNode = grid.NodeFromWorldPoint(startPos);
            Node targetNode = grid.NodeFromWorldPoint(targetPos);

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];

                // current = node in openSet with the lowest f_cost
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                // remove current from OPEN
                openSet.Remove(currentNode);
                // add current to CLOSED
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    RetracePath(startNode, targetNode);
                    return;
                }

                // foreach neighbour of the current node
                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour._isWalkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        openSet.Add(neighbour);
                    }
                    }
                }
            }

        private void RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();

            grid.path = path;
        }

        private int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA._gridX - nodeB._gridX);
            int dstY = Mathf.Abs(nodeA._gridY - nodeB._gridY);

            // diagonal movement is 14, horizontal/vertical movement is 10
            if (dstX > dstY)
            {
                return 14 * dstY + 10 * (dstX - dstY);
            }
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
    public class Node
    {
        public bool _isWalkable;
        public Vector2 _worldPosition;
        public int _gridX;
        public int _gridY;
        public int gCost;
        public int hCost;
        public int fCost { get { return gCost + hCost; } }
        public Node parent;
        // public Node parent;
        public Node(bool isWalkable, Vector2 worldPosition, int gridX, int gridY)
        {
            _isWalkable = isWalkable;
            _worldPosition = worldPosition;
            _gridX = gridX;
            _gridY = gridY;
        }
    }
}
