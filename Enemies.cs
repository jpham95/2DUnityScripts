using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Algorithms.AStar;
using Player;
using DataStructures;

public abstract class Enemy : MonoBehaviour
{
    public bool displayPathGizmos, verbose;
    public GameObject expPrefab;
    protected string expPrefabPath = "Resources/Exp"; // TODO: make exp prefab
    protected int targetIndex;
    protected GameObject _player;
    protected PlayerStats _playerStats;
    protected Transform _playerTransform, stuckObject;
    protected Rigidbody2D _rigidbody, _playerRigidBody;
    protected SpriteRenderer _spriteRenderer;
    protected Vector3[] path;
    protected Vector3 currentWaypoint, currentTarget;
    protected float health, speed, attackRange, attackCooldown, damage, expValue;
    protected float attackTimer = 0f, damageTimer = 0f, damageCooldown = 0.5f;
    protected float _previousDistanceToTarget, _currentDistanceToTarget = 0f;
    protected bool isLeader = false;
    private static Enemy Leader;
    private static List<Enemy> Enemies = new List<Enemy>();
    private float leaderTimer, leaderThreshold = 3f;
    protected float DistanceToPlayer
    {
        get 
        {
            return Vector3.Distance(transform.position, _playerTransform.position);
        }
    }
    public int CompareTo(Enemy other)
    {
        int comparison = DistanceToPlayer.CompareTo(other.DistanceToPlayer);

        if (comparison == 0)
        {
            comparison = speed.CompareTo(other.speed);
        }
        return comparison;
    }
    private void OnDrawGizmos()
    {
        if (path != null && displayPathGizmos)
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
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(currentWaypoint, 0.5f);
        }
    }
    private void OnEnable()
    {
        _player = GameObject.FindWithTag("Player");
        _playerTransform = _player.GetComponent<Transform>();
        _playerStats = _player.GetComponent<PlayerStats>();
        _playerRigidBody = _player.GetComponent<Rigidbody2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        currentTarget = _playerTransform.position;
        SetStats();
    }
    protected abstract void SetStats();
    private void FixedUpdate()
    {
        if (health <= 0) {Die();}
        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
        }
        if (damageTimer <= 0)
        {
            damageTimer = 0;
        }
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        if (attackTimer <= 0)
        {
            attackTimer = 0;
        }
        SpecialUpdate();
        if (leaderTimer > 0)
        {
            leaderTimer -= Time.deltaTime;
        }
        if (leaderTimer <= 0)
        {
            leaderTimer = leaderThreshold;
            SetLeader();
        }
        if (isLeader)
        {
            _previousDistanceToTarget = _currentDistanceToTarget;
            _currentDistanceToTarget = Vector3.Distance(transform.position, currentTarget);

            if (_currentDistanceToTarget >= _previousDistanceToTarget)
            {
                PathRequestManager.ResetPath(transform.position, currentTarget, OnPathFound);
            }
        }
        else
        {   
            _rigidbody.AddForce((Leader.transform.position - transform.position).normalized * speed*3, ForceMode2D.Impulse);
        }
    }
    protected abstract void SpecialUpdate();
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
        if (path.Length == 0) {yield break;}
        currentWaypoint = path[0];
        while (Vector3.Distance(transform.position, currentTarget) > 0.5f)
        {
            if (Vector3.Distance(transform.position, currentWaypoint) <= Mathf.Min(attackRange, 0.5f) && currentWaypoint != path[path.Length - 1])
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            SpecialMovement();

            _rigidbody.AddForce((currentWaypoint - transform.position).normalized * speed, ForceMode2D.Force);
            yield return null;
        }
    }

    protected abstract void SpecialMovement();
    protected virtual void Die()
    {
        GameObject exp = Instantiate(Resources.Load<GameObject>(expPrefabPath), transform.position, Quaternion.identity);
        // exp.GetComponent<Exp>().SetExp(expValue); // TODO
        Destroy(gameObject);
    }
    public void TakeDamage(float damageAmount)
    {
        DamageAnimation();
        if (damageTimer <= 0)
        {
            health = Mathf.Max(health - damageAmount, 0);
            damageTimer = damageCooldown;
            if (verbose)
            {
                Debug.Log("Hit enemy for " + damageAmount + " damage");
            }
        }
    }
    protected virtual void DamageAnimation()
    {
        if (damageTimer <= 0)
        {
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, Color.white, 0.05f);
        }
        else
        {
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, Color.clear, 0.05f);
        }
    }

    private static void SetLeader()
    {
        Enemy potentialLeader = null;
        foreach (Enemy enemy in Enemies)
        {
            if (potentialLeader == null || enemy.DistanceToPlayer < potentialLeader.DistanceToPlayer)
            {
                potentialLeader = enemy;
            }
        }
        potentialLeader.isLeader = true;
        Leader = potentialLeader;
    }
}