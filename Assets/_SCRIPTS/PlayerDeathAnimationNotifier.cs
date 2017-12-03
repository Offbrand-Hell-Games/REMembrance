using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Author: Corwin Belser
/// Attach this script to the same object as the player's
///     animator to call Respawn() message to PlayerController.cs
///     
/// This script is a poor replacement of AnimationNotifier.cs, as
///   the nested prefab plugin removes the ability for the reference
///   to the player to persist. This is because a child prefab
///   cannot store a reference to its parent when it does not
///   exist in the child prefab.
public class PlayerDeathAnimationNotifier : MonoBehaviour {

    private GameObject player;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}

    /// <summary>
    /// This is called from a notification event on Rem's
    ///     death animation. Calls the Respawn function
    ///     on PlayerController
    /// </summary>
    public void OnDeathFinished()
    {
        player.SendMessage("Respawn");
    }
}
