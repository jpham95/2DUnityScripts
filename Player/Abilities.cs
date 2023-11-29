using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System;


namespace Abilities
{
    public class AbilityManager
    {
        public static List<Ability> Abilities { get; private set; } = new List<Ability>();
        public static List<Ability> AvailableAbilities { get; private set; } = new List<Ability>();
        private void Awake()
        {
            // Get all the types that derive from Ability
            Abilities = Assembly.GetAssembly(typeof(Ability))
                .GetTypes()
                .Where(type => type.IsSubclassOf(typeof(Ability)))
                .Select(type => (Ability)Activator.CreateInstance(type))
                .ToList();
            AvailableAbilities.AddRange(Abilities);
        }
        
        public static Ability SelectRandomAbility()
        {
            return AvailableAbilities[UnityEngine.Random.Range(0, AvailableAbilities.Count)];
        }
    }

    public abstract class Ability : MonoBehaviour
    {
        public abstract string Name { get; }
        public abstract void Activate();
        public abstract void Deactivate();
    }

    public class SpinningBall : Ability
    {
        private float _spinDuration = 1f;
        private float _spinTimer = 0.0f;
        private float _activeTimer = 0.0f;
        private float _activeDuration = 5.0f;
        private float _cooldownTimer = 0.0f;
        private float _cooldownDuration = 5.0f;
        private bool _isActive = false;
        private bool _isFading = false;
        private Vector2 _ballTransform;
        private Transform _projectileTransform;
        private SpriteRenderer _projectileSprite;
        public Rigidbody2D _playerRigidbody;
        public override string Name { get; } = "Spinning Ball";
        public override void Activate()
        {
            _isActive = true;
        }
        public override void Deactivate()
        {
            _isActive = false;
        }
        private void OnEnable()
        {
            _projectileTransform = GameObject.FindWithTag("SpinningBallProjectile").GetComponent<Transform>();
            _projectileSprite = GameObject.FindWithTag("SpinningBallProjectile").GetComponent<SpriteRenderer>();
            _projectileSprite.enabled = false;
        }
        private void Update()
        {
            if (_isActive)
            {
                _projectileSprite.enabled = true;
                _spinTimer += Time.deltaTime;
                _activeTimer += Time.deltaTime;
                // Calculate the angle based on the current spin timer
                float angle = (_spinTimer / _spinDuration) * 360f;
                // Calculate the position of the projectile in the circular path
                float x = Mathf.Cos(angle * Mathf.Deg2Rad) * 2f;
                float y = Mathf.Sin(angle * Mathf.Deg2Rad) * 2f;
                Vector2 position = _playerRigidbody.position + new Vector2(x, y);
                // Move the projectile to the calculated position
                _projectileTransform.position = position;
                if (!_isFading)
                {
                    _projectileSprite.color = Color.white;
                    // Perform a CircleCast from the center of the projectile
                    RaycastHit2D[] hits = Physics2D.CircleCastAll(position, 0.5f, Vector2.zero);
                    // Handle the hits (e.g., apply damage, trigger events, etc.)
                    foreach (RaycastHit2D hit in hits)
                    {
                        // Handle the hit here
                    }
                }
                // Check if the spin duration has been reached
                if (_activeTimer >= _activeDuration)
                {
                    _isFading = true;
                    _activeTimer = 0f;
                    _cooldownTimer = _cooldownDuration;
                }
            }
            else
            {
                _cooldownTimer -= Time.deltaTime;
                if (_cooldownTimer <= 0.0f)
                {
                    Toggle();
                }
            }
            Fade();
        }
        private void Toggle()
        {
            _isActive = !_isActive;
            _spinTimer = 0f;
        }
        private void Fade()
        {
            // Fade the projectile sprite
            if (_isFading)
            {
                _projectileSprite.color = Color.Lerp(_projectileSprite.color, Color.clear, Time.deltaTime * 5f);
                if (_projectileSprite.color.a <= 0.1f)
                {
                    _projectileSprite.color = Color.clear;
                    _projectileSprite.enabled = false;
                    _isFading = false;
                    Toggle();
                }
            }
        }
    }
}