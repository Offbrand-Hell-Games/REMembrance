using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour {
	public GameObject label;
	public GameObject value;
	public GameObject slider;

	public delegate void valueChangedDelegate(float newvalue);


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Initialize(string _label, float _min, float _max, float _value, valueChangedDelegate vd) {
		label.GetComponent<Text> ().text = _label;
		value.GetComponent<Text> ().text = _value.ToString();
		slider.GetComponent<Slider> ().minValue = _min;
		slider.GetComponent<Slider> ().maxValue = _max;
		slider.GetComponent<Slider> ().value = _value;
		slider.GetComponent<Slider> ().onValueChanged.AddListener (delegate {
			vd (slider.GetComponent<Slider> ().value);
			value.GetComponent<Text> ().text = slider.GetComponent<Slider> ().value.ToString("F2");
		});
	}
}


