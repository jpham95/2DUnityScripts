using UnityEngine;
using Player;
using Player.Interactables;
public class HealthPickup : Interactable
{
    public float healthAmount = 10f;
    public override void Interact()
    {
        _player.GetComponent<PlayerStats>().Heal(healthAmount);
        Destroy(gameObject);
    }
}