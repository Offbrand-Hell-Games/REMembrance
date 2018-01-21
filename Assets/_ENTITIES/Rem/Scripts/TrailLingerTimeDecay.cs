using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailLingerTimeDecay : MonoBehaviour {
	private float elapsed_time = 0f;
	private float initial_lingertime;

	public float decay_coefficient;
	// Use this for initialization
	void Start () {
		initial_lingertime = GetComponent<TrailRenderer> ().time;
	}
	
	// Update is called once per frame
	void Update () {
		elapsed_time += Time.deltaTime;
		GetComponent<TrailRenderer> ().time = initial_lingertime - decay_coefficient * (elapsed_time * elapsed_time);
	}
}
