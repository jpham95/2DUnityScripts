using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Interactables;
using Abilities;

namespace Player
{
    public class PlayerStats : MonoBehaviour
    {
        private static float _maxHealth = 100f;
        private static float _health;
        private static float _exp = 0f;
        private static float _nextLevelExp = 50f;
        private static int _level = 1;

        private Dictionary<string, Ability> _playerAbilities = new Dictionary<string, Ability>();

        private void Awake()
        {
            _health = _maxHealth;
        }
        private void Update()
        {
            // handle player death
            if (_health <= 0)
            {
                // die
            }

            // handle pickups
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f, LayerMask.GetMask("Pickup"));
            foreach (Collider2D collider in colliders)
            {
                GameObject gameObject = collider.GetComponent<GameObject>();
                Interactable interactable = gameObject.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.Interact(this);
                }
            }
        }

        public static void Heal(float healAmount)
        {
            _health = Mathf.Min(_health + healAmount, _maxHealth);
        }

        public static void GainExp(float expAmount)
        {
            // gain exp
        }

        public static void TakeDamage(float damageAmount)
        {
            _health = Mathf.Max(_health - damageAmount, 0);
        }

        private void LevelUp()
        {
            // level up
        }

        private void AddAbility(Ability ability)
        {
            AbilityManager.AvailableAbilities.Remove(ability);
            _playerAbilities.Add(ability.Name, ability);
            _playerAbilities[ability.Name].Activate();
        }

        private void RemoveAbility(Ability ability)
        {
            AbilityManager.AvailableAbilities.Add(ability);
            _playerAbilities[ability.Name].Deactivate();
            _playerAbilities.Remove(ability.Name);
        }
    }
}