using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefsPaneManager : MonoBehaviour {
	public static PrefsPaneManager instance;

	public GameObject prefs_container;
	public GameObject slider_prefab;
	public GameObject OTS_camera;
//	public delegate void valueChangedDelegate(float newvalue);
	//float importantValue = .7f;
	// Use this for initialization
	void Start () {
		instance = this;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (prefs_container.activeSelf) {
				prefs_container.SetActive (false);
			} else {
				prefs_container.SetActive (true);
			}
		}

		OTS_camera.GetComponent<SmoothFollowOTS_Jim> ().freezeCam = prefs_container.activeSelf;

//		if (Input.GetKeyDown (KeyCode.I) && false) {
//			GameObject newslider = Instantiate (slider_prefab);
//			newslider.transform.parent = prefs_container.transform;
//			newslider.GetComponent<SliderManager> ().Initialize ("New Slider", .5f, 2.5f, importantValue, valsChanged);
//		}
	}

	public void AddLivePreferenceFloat(string label, float min, float max, float value, SliderManager.valueChangedDelegate vd) {
		GameObject newslider = Instantiate (slider_prefab);
		newslider.transform.parent = prefs_container.transform;
		newslider.GetComponent<SliderManager> ().Initialize (label, min, max, value, vd);
	}
}
