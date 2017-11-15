using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationNotifier : MonoBehaviour {
	
	public GameObject target;

	public void Notify(string message){
		target.SendMessage(message);
	}
}
