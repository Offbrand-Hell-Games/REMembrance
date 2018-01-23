using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class XRayPortalControl : MonoBehaviour {
	public GameObject PortalVisualObject;
	public float cooldownTime = 6f;
	private float elapsedCooldownTime = 0f;
	public float portalLifetime = 8f;
	Coroutine portalRemove;

    private Player _player;

    // Use this for initialization
    void Start () {
        _player = ReInput.players.GetPlayer(0);   //controller mapping
    }
	
	// Update is called once per frame
	void Update () {
		// if we are able to place a new portal and the portal button is pressed, activate the portal positioning object/script.
		//  (and reset the cooldown)
		if (elapsedCooldownTime >= cooldownTime  && _player.GetButtonDown("Xray")) {
			PortalVisualObject.SetActive (true);
			PortalVisualObject.GetComponent<PositionAtRaycastPoint2> ().Position ();
			if (portalRemove != null) StopCoroutine (portalRemove);
			portalRemove = StartCoroutine (RemovePortal ());
			elapsedCooldownTime = 0f;
		}
		// Update the cooldown time until the cooldown is complete
		if (elapsedCooldownTime <= cooldownTime) {
			elapsedCooldownTime += Time.deltaTime;
		}

	}

	public IEnumerator RemovePortal() {
		yield return new WaitForSeconds (portalLifetime);
		PortalVisualObject.GetComponent<PositionAtRaycastPoint2> ().ShrinkAndDisappear ();
	}
}
