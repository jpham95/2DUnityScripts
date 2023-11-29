using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Algorithms.AStar;

public class Enemy : MonoBehaviour
{
    private float _health = 100f;
    private float _damage = 10f;
    private float _speed = 1f;
    private float _attackRange = 1f;
    private float _attackCooldown = 1f;
    private float _attackTimer = 0f;
    private Transform _playerTransform;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Vector3[] path;
    private int targetIndex;
    private Vector3 currentWaypoint;
    private Vector3 currentTarget;
    private float stuckTimer = 0f;
    private float stuckThreshold = 0.5f;
    private float wrongWayTimer = 0f;
    private float wrongWayThreshold = 0.3f;
    private Transform stuckObject;

    

    private void OnEnable()
    {
        _playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentTarget = _playerTransform.position;
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        currentWaypoint = path[0];
        while (Vector3.Distance(currentTarget, _playerTransform.position) < 0.5f)
        {
            if (Vector3.Distance(transform.position, currentWaypoint) <= Mathf.Min(_attackRange, 0.5f) && currentWaypoint != path[path.Length - 1])
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            // handle getting stuck on walls
            if (Vector3.Distance(_rigidbody.velocity, Vector3.zero) < 0.1f)
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer >= stuckThreshold)
                {   
                    // get object stuck on
                    stuckObject = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Walls")).transform;
                    // push away from stuckObject
                    _rigidbody.AddForce((transform.position - stuckObject.position).normalized * _speed*3, ForceMode2D.Impulse);
                    Debug.Log("Stuck");
                }
            }
            // handle going the wrong way
            if (Vector3.Distance(transform.position, currentWaypoint) > Vector3.Distance(transform.position, _playerTransform.position))
            {
                wrongWayTimer += Time.deltaTime;
                if (wrongWayTimer >= wrongWayThreshold)
                {
                    Debug.Log("Wrong Way");
                }
            }
            if (wrongWayTimer > wrongWayThreshold || stuckTimer > stuckThreshold)
            {
                wrongWayTimer = 0f;
                stuckTimer = 0f;
                Debug.Log("Repathing");
                PathRequestManager.ResetPath(transform.position, _playerTransform.position, OnPathFound);
            }
            _rigidbody.AddForce((currentWaypoint - transform.position).normalized * _speed, ForceMode2D.Force);
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(path[i], 0.5f);
                
                Gizmos.color = Color.white;
                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i-1], path[i]);
                }
            }
        }
        if (currentWaypoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(currentWaypoint, 0.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(currentTarget, _playerTransform.position) > 0.5f)
        {
            currentTarget = _playerTransform.position;
            PathRequestManager.RequestPath(transform.position, _playerTransform.position, OnPathFound);
        }

    }
}