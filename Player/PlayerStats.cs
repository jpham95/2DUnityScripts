using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Interactables;
using Abilities;

namespace Player
{
    public class PlayerStats : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private float _maxHealth = 100f, _health;
        private float _exp = 0f, _nextLevelExp = 50f;
        private int _level = 1;
        private float _pickupRadius = 1f;
        private Dictionary<string, Ability> _playerAbilities = new Dictionary<string, Ability>();

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _health = _maxHealth;
        }
        private void FixedUpdate()
        {
            // handle ability updates
            foreach (Ability ability in _playerAbilities.Values)
            {
                ability.Update();
            }

            // handle interactables
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _pickupRadius, LayerMask.GetMask("Interactable"));
            foreach (Collider2D collider in colliders)
            {
                GameObject gameObject = collider.gameObject;
                Interactable interactable = gameObject.GetComponent<Interactable>();
                // Debug.Log("Interacting with " + gameObject.name);
                // Debug.Log("Interactable: " + interactable);
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }

            // handle lerping back to default color
            if (_spriteRenderer.color != Color.white)
            {
                _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, Color.white, 0.05f);
            }
        }

        public void Heal(float healAmount)
        {
            _health = Mathf.Min(_health + healAmount, _maxHealth);
        }

        public void GainExp(float expAmount)
        {
            // gain exp
        }

        public void TakeDamage(float damageAmount)
        {
            _health = Mathf.Max(_health - damageAmount, 0);
            //flash red
            _spriteRenderer.color = Color.red;
            if (_health <= 0)
            {
                Die();
            }
            
        }

        private void Die()
        {
            Debug.Log("Player has died");
        }

        private void LevelUp()
        {
            // level up
        }
        public bool AbilityExists(Ability ability)
        {
            return _playerAbilities.ContainsKey(ability.Name);
        }
        public void AddAbility(Ability ability)
        {
            AbilityManager.AvailableAbilities.Remove(ability);
            Debug.Log("Adding " + ability.Name + " to player abilities");
            _playerAbilities.Add(ability.Name, ability);
            _playerAbilities[ability.Name].Activate();
        }

        public void RemoveAbility(Ability ability)
        {
            AbilityManager.AvailableAbilities.Add(ability);
            _playerAbilities[ability.Name].Deactivate();
            _playerAbilities.Remove(ability.Name);
        }
    }
}