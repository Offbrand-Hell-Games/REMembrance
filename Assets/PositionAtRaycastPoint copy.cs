using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionAtRaycastPoint : MonoBehaviour {
	public bool active = true;
	public Camera cam1;
	public float maxDistance;
	public LayerMask Raycast_LayerMask;
	private Vector3 initialScale;
	public float sizeUpDuration = .25f;
	private bool sizingUp = false;
	private bool sizingDown = false;

	// Use this for initialization
	void Start () {
		initialScale = transform.localScale;
		Debug.Log (initialScale);
		gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (sizingUp) {
			Debug.Log (transform.localScale);
			Debug.Log (initialScale);
			transform.localScale += initialScale * (Time.deltaTime / sizeUpDuration);
			sizingUp = transform.localScale.magnitude < initialScale.magnitude;
		} else if (sizingDown) {
			transform.localScale -= initialScale * (Time.deltaTime / sizeUpDuration);
			sizingDown = transform.localScale.magnitude > 0.1  && transform.localScale.x > 0.1;
			if (!sizingDown) {
				gameObject.SetActive (false);
			}
		}
	}

	public void ShrinkAndDisappear() {
		sizingDown = true;
	}

	public void Position () {
		if (active) {
			//			Camera cam1 = gameObject.transform.parent.gameObject.GetComponentsInChildren<Camera> () [0];
			//			Camera cam2 = gameObject.transform.parent.gameObject.GetComponentsInChildren<Camera> () [1];

			// point a ray forward through the center of the viewport and report where it hits.
			Ray ray = cam1.ViewportPointToRay (new Vector3 (.5f, .5f, 0));
			// In the future we would probably restrict the range of the vision ability, or choose not to activate it 
			// upon invalid objects. For now though, the mask will go wherever the raycast hits.
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, maxDistance, Raycast_LayerMask)) {
				//				GetComponent<MeshRenderer> ().enabled = true;
				//				gameObject.transform.GetChild (0).gameObject.GetComponent<MeshRenderer> ().enabled = true;

				Vector3 newposition = hit.point;
				transform.position = newposition + (-hit.normal * .1f);
				transform.LookAt (hit.point + hit.normal);
				transform.Rotate (90f, 0f, 0f);
				transform.localScale = Vector3.zero;

				Debug.DrawRay (hit.point, new Vector3 (hit.normal.x, hit.normal.z, hit.normal.y) * 20, Color.red);
				Debug.DrawRay (hit.point, hit.normal * 20, Color.cyan); 
				//				cam2.farClipPlane = Vector3.Distance (cam1.transform.position, transform.position);
				//				cam1.nearClipPlane = cam2.farClipPlane;
				sizingUp = true;
				Debug.Log ("ay");
			} else {
				//				GetComponent<MeshRenderer> ().enabled = false;
				//				gameObject.transform.GetChild (0).gameObject.GetComponent<MeshRenderer> ().enabled = false;
			}
		}
	}
}
