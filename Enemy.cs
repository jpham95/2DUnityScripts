using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Algorithms.AStar;
using Player;
/*
Path Storage: The path variable is an array of Vector3 objects that stores the
path for each enemy instance. If there are thousands of instances, each with its
own path, it could consume a significant amount of memory. Consider optimizing
the path storage by using a more memory-efficient data structure or sharing
paths among instances when possible.

GameObject.FindWithTag: In the OnEnable method, GameObject.FindWithTag is used
to find the player object. Calling this method frequently for thousands of
instances can be inefficient. Consider caching the player object reference or
finding it once and sharing it among instances.

Coroutine Memory: The FollowPath coroutine is used to move the enemy along the
path. If there are thousands of instances, each running its own coroutine, it
can consume memory. Consider optimizing the coroutine usage or using a different
approach for enemy movement.
*/
public class Seeker : Enemy
{
    private float stuckTimer = 0f, stuckThreshold = 0.5f;
    private bool isFleeing = false;
    protected override void SetStats()
    {
        health = 100f;
        speed = 5f;
        attackRange = 1f;
        attackCooldown = 1f;
        damage = 10f;
        expValue = 10f;
    }
    protected override void SpecialMovement()
    {
        // handle getting stuck on walls
        if (Vector3.Distance(_rigidbody.velocity, Vector3.zero) < 0.1f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckThreshold)
            {   
                // get object stuck on
                stuckObject = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Walls")).transform;
                // push away from stuckObject
                _rigidbody.AddForce((transform.position - stuckObject.position).normalized * speed*3, ForceMode2D.Impulse);
                if (verbose)
                {
                Debug.Log("Stuck");
                }
            }
        }
        if (stuckTimer > stuckThreshold)
        {
            stuckTimer = 0f;
            PathRequestManager.ResetPath(transform.position, currentTarget, OnPathFound);
            if (verbose)
            {
                Debug.Log("Repathing");
            }
        }
    }
    protected override void SpecialUpdate()
    {
        if (attackTimer > 0) 
        {
            isFleeing = true;
        }
        else
        {
            isFleeing = false;
        }

        if (isFleeing) // handle fleeing
        {
            _spriteRenderer.color = Color.blue;
            Flee();
        }
        else
        {
            Attack();
        }
        if (stuckTimer > 0)
        {
            stuckTimer -= Time.deltaTime;
        }
        // handle lerping back to default color
        if (_spriteRenderer.color != Color.red)
        {
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, Color.red, 0.05f);
        }
    }

    private void Attack()
    {
        currentTarget = _playerTransform.position;
        if (Vector2.Distance(transform.position, currentTarget) <= attackRange)
        {
            _playerStats.TakeDamage(damage);
            attackTimer = attackCooldown;
            _playerRigidBody.AddForce((currentTarget - transform.position).normalized * 50, ForceMode2D.Impulse);
        }
    }
    private void Flee()
    {
        currentTarget = _playerTransform.position - (_playerTransform.position - transform.position).normalized * 10;
        PathRequestManager.RequestPath(transform.position, currentTarget, OnPathFound);
    }
}