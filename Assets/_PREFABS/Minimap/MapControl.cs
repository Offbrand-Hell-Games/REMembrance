using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControl : MonoBehaviour {
	// 0 = absent;
	// 1 = top left corner;
	// 2 = fill screen.
	public int map_position;
	private Camera cam;

	public Material backgroundMat;
	public Material wallOutlineMat;

	bool resetting = false;
	void Start() {
		cam = GetComponent<Camera> ();
	}

	float _x, _y;
	float timeheld = 0f;
	bool clear_position_toggle = false;

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.Tab)) {
			if (clear_position_toggle) {
				clear_position_toggle = false;
			} else {
				map_position++;
				timeheld = 0f;
				if (map_position >= 3) {
					map_position = 0;
				}
			}
		}

		Debug.Log (map_position);

		_x = Mathf.Abs(cam.rect.x);
		if (map_position == 0) {
			_x = 0f;
		} else if (map_position == 1) {
			_x = .5f;
		} else if (map_position == 2) {
			if (Mathf.Abs(_x) < .02f) {
				_x = 0f;                                           
			} else {
				_x = Mathf.Lerp (_x, 0f, .3f);
			}
		}

		if (map_position == 0) {
			cam.rect = new Rect (0f, 0f, 0f, 0f);
		} else if (map_position == 1) {
			cam.rect = new Rect (-_x, _x, 1f, 3f);
		} else if (map_position == 2) {
			cam.rect = new Rect (-_x, _x, 1f, 3f);
		}

		if (Input.GetKey (KeyCode.Tab)) {
			if (timeheld <= .6f) {
				timeheld += Time.deltaTime;
			} else {
				clear_position_toggle = true;
				Color c = backgroundMat.color;
				c.a = Mathf.Lerp (c.a, 1f, .2f);
				backgroundMat.color = c;
				c = wallOutlineMat.color;
				c.a = Mathf.Lerp (c.a, 1f, .2f);
				wallOutlineMat.color = c;
			}
		} else if (Input.GetKeyUp (KeyCode.Tab)) {
			resetting = true;
			timeheld = 0f;
		} else if (resetting) {
			Color c = backgroundMat.color;
			c.a = Mathf.Lerp (c.a, .15f, .2f);
			backgroundMat.color = c;
			c = wallOutlineMat.color;
			c.a = Mathf.Lerp (c.a, .15f, .2f);
			wallOutlineMat.color = c;
		}
	}

}
