using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRayPortalControl : MonoBehaviour {
	public GameObject PortalVisualObject;
	public float cooldownTime = 6f;
	private float elapsedCooldownTime = 0f;
	public float portalLifetime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// if we are able to place a new portal and the portal button is pressed, activate the portal positioning object/script.
		//  (and reset the cooldown)
		if (elapsedCooldownTime >= cooldownTime  && Input.GetKeyDown (KeyCode.Q)) {
			PortalVisualObject.SetActive (true);
			PortalVisualObject.GetComponent<PositionAtRaycastPoint> ().Position ();
			StartCoroutine (RemovePortal ());
			elapsedCooldownTime = 0f;
		}
		// Update the cooldown time until the cooldown is complete
		if (elapsedCooldownTime <= cooldownTime) {
			elapsedCooldownTime += Time.deltaTime;
		}

	}

	public IEnumerator RemovePortal() {
		yield return new WaitForSeconds (portalLifetime);
		PortalVisualObject.GetComponent<PositionAtRaycastPoint> ().ShrinkAndDisappear ();
	}
}
