using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHidingPlanesContainer : MonoBehaviour {
	public void ResetAll() {
		BroadcastMessage ("HideMap");
	}
}
