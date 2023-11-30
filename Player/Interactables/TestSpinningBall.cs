using UnityEngine;
using Player;
using Abilities;
using Player.Interactables;
public class TestSpinningBall : Interactable
{
    private SpinningBall test = new SpinningBall();
    public override void Interact(PlayerStats player)
    {   
        Debug.Log("Ability exists: " + PlayerStats.AbilityExists(test));
        if (!PlayerStats.AbilityExists(test))
        {
            PlayerStats.AddAbility(test);
        }
        Destroy(gameObject);
    }
}