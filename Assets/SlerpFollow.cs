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
		transform.position = Vector3.MoveTowards (a, b, Mathf.Max((Vector3.Distance(a, b) * followCoefficient), minDistance));
//		transform.rotation = new Quaternion (0,0,0,0);
	}
}
