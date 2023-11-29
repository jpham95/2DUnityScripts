using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataStructures;
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

/*
MaxHeap: The MaxHeap data structure is used as a priority queue in the A*
algorithm. If you have thousands of instances, each with its own MaxHeap, it
could consume a significant amount of memory. Consider optimizing the MaxHeap
implementation or exploring alternative data structures that have better memory
efficiency.

HashSet: The HashSet data structure is used as the closed set in the A*
algorithm. Similar to the MaxHeap, if you have thousands of instances, each with
its own HashSet, it could consume a significant amount of memory. Consider
optimizing the HashSet implementation or exploring alternative data structures
that have better memory efficiency.

Grid: The AStarGrid class is used to represent the grid for pathfinding. If the
grid size is large and you have thousands of instances, it could consume a
significant amount of memory. Consider optimizing the grid representation or
exploring alternative data structures that have better memory efficiency.

Path and Waypoints: The path and waypoints lists are used to store the path and
simplified path respectively. If you have thousands of instances and each
instance has a long path, it could consume a significant amount of memory.
Consider optimizing the path storage or exploring alternative data structures
that have better memory efficiency.
*/
namespace Algorithms.AStar
{
    public class AStarPathfinding : MonoBehaviour
    {
        private PathRequestManager requestManager;
        public Transform seeker, target;
        AStarGrid grid;

        private void Awake()
        {
            grid = GetComponent<AStarGrid>();
            requestManager = GetComponent<PathRequestManager>();
        }

        public void StartFindPath(Vector3 startPos, Vector3 targetPos)
        {
            StartCoroutine(FindPath(startPos, targetPos));
        }

        private IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Vector3[] waypoints = new Vector3[0];
            bool pathFound = false;

            Node startNode = grid.NodeFromWorldPoint(startPos);
            Node targetNode = grid.NodeFromWorldPoint(targetPos);
            if (targetNode._isWalkable == false)
            {
                foreach (Node neighbour in grid.GetNeighbours(targetNode))
                {
                    if (neighbour._isWalkable && GetDistance(startNode, neighbour) < GetDistance(startNode, targetNode))
                    {
                        targetNode = neighbour;
                        break;
                    }
                }
            }
            if (startNode._isWalkable == false)
            {
                foreach (Node neighbour in grid.GetNeighbours(startNode))
                {
                    if (neighbour._isWalkable && GetDistance(neighbour, targetNode) < GetDistance(startNode, targetNode))
                    {
                        startNode = neighbour;
                        break;
                    }
                }
            }
            if (startNode._isWalkable && targetNode._isWalkable)
            {    
                MaxHeap<Node> openSet = new MaxHeap<Node>(grid.MaxSize); // priority queue
                HashSet<Node> closedSet = new HashSet<Node>(); // empty set
                openSet.Add(startNode); // add the start node to OPEN

                while (openSet.Count > 0) 
                {
                    Node currentNode = openSet.Pop();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        pathFound = true;
                        break;
                    }

                    // foreach neighbour of the current node
                    foreach (Node neighbour in grid.GetNeighbours(currentNode))
                    {
                        // if neighbour is not traversable || neighbour is in CLOSED
                        if (!neighbour._isWalkable || closedSet.Contains(neighbour))
                        {
                            continue; // skip to the next neighbour
                        }
                        // if new path to neighbour is shorter || neighbour is not in OPEN
                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            // set costs of neighbour
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            // set parent of neighbour to current
                            neighbour.parent = currentNode;
                            // add neighbour to OPEN
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
            yield return null; // wait for one frame before returning
            if (pathFound)
            {
                waypoints = RetracePath(startNode, targetNode);
            }
            requestManager.FinishedProcessingPath(waypoints, pathFound);
        }

        private Vector3[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            Vector3[] waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);
            return waypoints;

            // grid.path = path;
        }

        private Vector3[] SimplifyPath(List<Node> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;
            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(path[i-1]._gridX - path[i]._gridX, path[i-1]._gridY - path[i]._gridY);
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i]._worldPosition);
                }
                directionOld = directionNew;
            }
            return waypoints.ToArray();
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
    public class Node : IComparable<Node>
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

        public int CompareTo(Node other)
        {
            int comparison = fCost.CompareTo(other.fCost);

            if (comparison == 0)
            {
                comparison = hCost.CompareTo(other.hCost);
            }
            return -comparison;
        }

    }
}
