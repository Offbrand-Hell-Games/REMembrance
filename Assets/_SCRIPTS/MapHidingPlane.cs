using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHidingPlane : MonoBehaviour {
	// Use this for initialization
	void Start () {
		GetComponent<MeshRenderer> ().enabled = false;
	}

	public void RevealMap() {
		GetComponent<MeshRenderer> ().enabled = true;
	}

	// This is called by Broadcast. if you change the name, look for the broadcast string and change it too.
	public void HideMap() {
		GetComponent<MeshRenderer> ().enabled = false;
	}

	public void OnTriggerEnter(Collider c) {
		RevealMap ();
	}
}
