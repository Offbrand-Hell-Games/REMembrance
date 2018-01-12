using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefsPaneManager : MonoBehaviour {
	public static PrefsPaneManager instance;

	public GameObject pause_menu;
	public GameObject prefs_container;
	public GameObject slider_prefab;
	public GameObject OTS_camera;
	//public GameObject prefs_ui;	/// <param name=prefs_ui> The parent object that contains buttons used in the Prefs Pane.</param>
//	public delegate void valueChangedDelegate(float newvalue);
	//float importantValue = .7f;
	// Use this for initialization
	void Start () {
		if (instance != null) {
			Destroy (gameObject);
		} else {
			instance = this;
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (pause_menu.activeSelf) {
				pause_menu.SetActive (false);
				prefs_container.SetActive(false);
				//prefs_ui.SetActive(false);
				Cursor.visible = false;
			} else {
				prefs_container.SetActive(true);
				pause_menu.SetActive (true);
				//prefs_ui.SetActive(true);
				Cursor.visible = true;
			}
		}

		OTS_camera.GetComponent<OTSCamera3> ().freezeCam = pause_menu.activeSelf;

//		if (Input.GetKeyDown (KeyCode.I) && false) {
//			GameObject newslider = Instantiate (slider_prefab);
//			newslider.transform.parent = prefs_container.transform;
//			newslider.GetComponent<SliderManager> ().Initialize ("New Slider", .5f, 2.5f, importantValue, valsChanged);
//		}
	}

	public void AddLivePreferenceFloat(string label, float min, float max, float value, SliderManager.valueChangedDelegate vd) {
		GameObject newslider = Instantiate (slider_prefab);
		newslider.transform.SetParent(prefs_container.transform);
		newslider.GetComponent<SliderManager> ().Initialize (label, min, max, value, vd);
	}
}
