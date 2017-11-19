using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// Author: Noah Nam
/// This class is placed on environment objects that should look like they're floating mid-air.
public class FloatingObject : MonoBehaviour {
	public float RISE_FALL_SPEED = 0.01f; 
	public float ROTATION_SPEED = 5f;
	private float sinX;
	private Vector3 randomRotation;
	
	/// <summary> Start will set a random starting X value for to calculate sin(X) from, and a random direction for the object to rotate.
	void Start() {
		sinX= Random.Range(0f,Mathf.PI*2f);
		randomRotation = new Vector3(Random.value,Random.value,Random.value).normalized;
	}
	
	/// <summary> Update will smoothly translate the object up or down, depending on sin(sinX), and will rotate the object.
	void Update () {
		sinX += Mathf.PI/60f;
		if (sinX >= Mathf.PI * 2f) // To prevent an eventual but unlikely overflow?
			sinX = 0f;	
		Vector3 translationTarget = new Vector3(0,Mathf.Sin(sinX),0);
		transform.Translate(translationTarget * RISE_FALL_SPEED * Time.deltaTime, Space.World);
		transform.Rotate(randomRotation * ROTATION_SPEED * Time.deltaTime);
	}
}
