using UnityEngine;

namespace Player.Interactables
{
    public abstract class Interactable : MonoBehaviour
    {
        public abstract void Interact(PlayerStats player);
    }
    public class HealthPickup : Interactable
    {
        public float healthAmount = 10f;
        public override void Interact(PlayerStats player)
        {
            PlayerStats.Heal(healthAmount);
        }
    }

}