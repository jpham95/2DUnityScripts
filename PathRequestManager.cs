using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Algorithms.AStar;

public class PathRequestManager : MonoBehaviour
{
    private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    private PathRequest currentPathRequest;
    private AStarPathfinding aStarPathfinding;
    private bool isProcessingPath;

    private static PathRequestManager instance;

    private void Awake()
    {
        instance = this;
        aStarPathfinding = GetComponent<AStarPathfinding>();
    }
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {   
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    public static void ResetPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        instance.pathRequestQueue.Clear();
        instance.isProcessingPath = false;
        instance.currentPathRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(instance.currentPathRequest);
        instance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            aStarPathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }
    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> callback)
        {
            pathStart = _start;
            pathEnd = _end;
            this.callback = callback;
        }
    }
}
