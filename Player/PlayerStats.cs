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
        private static float _pickupRadius = 1f;

        private static Dictionary<string, Ability> _playerAbilities = new Dictionary<string, Ability>();

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

            foreach (Ability ability in _playerAbilities.Values)
            {
                ability.Update();
            }

            // handle pickups
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _pickupRadius, LayerMask.GetMask("Interactable"));
            foreach (Collider2D collider in colliders)
            {
                GameObject gameObject = collider.gameObject;
                Interactable interactable = gameObject.GetComponent<Interactable>();
                Debug.Log("Interacting with " + gameObject.name);
                Debug.Log("Interactable: " + interactable);
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
        public static bool AbilityExists(Ability ability)
        {
            return _playerAbilities.ContainsKey(ability.Name);
        }
        public static void AddAbility(Ability ability)
        {
            AbilityManager.AvailableAbilities.Remove(ability);
            Debug.Log("Adding " + ability.Name + " to player abilities");
            _playerAbilities.Add(ability.Name, ability);
            _playerAbilities[ability.Name].Activate();
        }

        public static void RemoveAbility(Ability ability)
        {
            AbilityManager.AvailableAbilities.Add(ability);
            _playerAbilities[ability.Name].Deactivate();
            _playerAbilities.Remove(ability.Name);
        }
    }
}