using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapMarkerFollow : MonoBehaviour {
	[SerializeField]
	public float minDistance = .003f;
	[SerializeField]
	public float followCoefficient = .1f;
	[SerializeField]
	public Transform target;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 newPos = transform.position;
		newPos.x = target.position.x;
		newPos.z = target.position.z;
		Vector3 next;

		if (Vector3.Distance (transform.position, newPos) < minDistance) {
			Debug.Log ("Setting Distance Directly");
			next = newPos;
		} else {
			Debug.Log ();
			next = ((newPos - transform.position) * followCoefficient) + newPos;
		}
		transform.position = next;
	}
}
