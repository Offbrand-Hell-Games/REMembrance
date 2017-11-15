using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// Author: Noah Nam
/// This is meant to be used in animation events. Notify will send a message to the gameobject with the method to be called.
public class AnimationNotifier : MonoBehaviour {
	
	public GameObject target;

	/// <summary> Notify uses SendMessage to a target gameobject.</summary>
	/// <param name=message> The name of the function you want to be run from the target gameobject.</param>
	public void Notify(string message){
		target.SendMessage(message);
	}
}
