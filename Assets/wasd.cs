using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wasd : MonoBehaviour {
	public float speed = 5f;

	// Use this for initialization
	void Start () {
		
	}


	// Update is called once per frame
	void Update () {
		float frontBackTarget = 0f;
		if (Input.GetKey(KeyCode.W)) {
			frontBackTarget += 1f;
		}
		if (Input.GetKey(KeyCode.S)) {
			frontBackTarget -= 1;
		}

		float leftRightTarget = 0f; 
		if (Input.GetKey(KeyCode.A)) {
			leftRightTarget -= 1;
		}
		if (Input.GetKey(KeyCode.D)) {
			leftRightTarget += 1;
		}
			
		Vector3 newv3 = (Vector3.forward * frontBackTarget) + (Vector3.right * leftRightTarget);
		gameObject.GetComponent<Rigidbody>().velocity =  newv3.normalized * speed;
	}
}
