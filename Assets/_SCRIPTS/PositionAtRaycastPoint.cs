using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Position at raycast point2 includes functionality for positioning the vision portal according to the normal of the wall you palce it on.
/// Combined with XRayPortalControl.cs on the Player Object (or anywhere, tbh), now places the wall once when you hit Q, rather
/// than having it constantly move with your camera.
/// </summary>
public class PositionAtRaycastPoint : MonoBehaviour {
	public bool active = true;
	public Camera cam1;
	public float maxDistance;
	public LayerMask Raycast_LayerMask;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (active) {
//			Camera cam1 = gameObject.transform.parent.gameObject.GetComponentsInChildren<Camera> () [0];
//			Camera cam2 = gameObject.transform.parent.gameObject.GetComponentsInChildren<Camera> () [1];

			// point a ray forward through the center of the viewport and report where it hits.
			Ray ray = cam1.ViewportPointToRay (new Vector3 (.5f, .5f, 0));
			// In the future we would probably restrict the range of the vision ability, or choose not to activate it 
			// upon invalid objects. For now though, the mask will go wherever the raycast hits.
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, maxDistance, Raycast_LayerMask)) {
				GetComponent<MeshRenderer> ().enabled = true;
				gameObject.transform.GetChild (0).gameObject.GetComponent<MeshRenderer> ().enabled = true;

				Vector3 newposition = hit.point;
				transform.position = newposition;
//				cam2.farClipPlane = Vector3.Distance (cam1.transform.position, transform.position);
//				cam1.nearClipPlane = cam2.farClipPlane;
			} else {
				GetComponent<MeshRenderer> ().enabled = false;

				gameObject.transform.GetChild (0).gameObject.GetComponent<MeshRenderer> ().enabled = false;
			}
		}
	}
}
