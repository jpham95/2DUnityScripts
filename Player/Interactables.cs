using UnityEngine;

namespace Player.Interactables
{
    public abstract class Interactable : MonoBehaviour
    {
        protected GameObject _player;
        private void OnEnable()
        {
            _player = GameObject.FindWithTag("Player");
        }
        public abstract void Interact();
    }
}