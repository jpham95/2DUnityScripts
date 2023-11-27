using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float _health = 100f;
    private float _damage = 10f;
    private float _speed = 5f;
    private float _attackRange = 1f;
    private float _attackCooldown = 1f;
    private float _attackTimer = 0f;
    private Transform _playerTransform;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;

    private void OnEnable()
    {
        _playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}