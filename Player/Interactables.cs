using UnityEngine;

namespace Player.Interactables
{
    public abstract class Interactable : MonoBehaviour
    {
        public abstract void Interact(PlayerStats player);
    }
}