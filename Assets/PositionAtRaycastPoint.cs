using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionAtRaycastPoint : MonoBehaviour {
	public bool active = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (active) {
			Camera cam1 = gameObject.transform.parent.gameObject.GetComponentsInChildren<Camera> () [0];
//			Camera cam2 = gameObject.transform.parent.gameObject.GetComponentsInChildren<Camera> () [1];

			// point a ray forward through the center of the viewport and report where it hits.
			Ray ray = cam1.ViewportPointToRay (new Vector3 (.5f, .5f, 0));
			// In the future we would probably restrict the range of the vision ability, or choose not to activate it 
			// upon invalid objects. For now though, the mask will go wherever the raycast hits.
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				Vector3 newposition = hit.point;
				transform.position = newposition;
//				cam2.farClipPlane = Vector3.Distance (cam1.transform.position, transform.position);
//				cam1.nearClipPlane = cam2.farClipPlane;
			};
		}
	}
}
