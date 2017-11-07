using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlerpFollow : MonoBehaviour {
	[SerializeField]
	public float minDistance;
	[SerializeField]
	public float followCoefficient;
	[SerializeField]
	public Transform target;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		Vector3 a = transform.position;
		Vector3 b = target.position;
		Vector3 next;
		if (Vector3.Distance (a, b) < minDistance) {
			Debug.Log ("Setting Distance Directly");
			next = b;
		} else {
			next = ((b - a) * followCoefficient) + a;
		}
		transform.position = next;
	}
}
