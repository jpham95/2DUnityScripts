using UnityEngine;
using Player;
using Abilities;
using Player.Interactables;
public class TestSpinningBall : Interactable
{
    private SpinningBall test = new SpinningBall();
    private PlayerStats _playerStats;
    public override void Interact()
    {   
        _playerStats = _player.GetComponent<PlayerStats>();
        Debug.Log("Ability exists: " + _playerStats.AbilityExists(test));
        if (!_playerStats.AbilityExists(test))
        {
            _playerStats.AddAbility(test);
        }
        Destroy(gameObject);
    }
}