using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SmoothFollowOTS_Jim : MonoBehaviour
{
//	[SerializeField]
//	private float _cameraSpeed = 5f;	//The rate at which the camera moves forwards and backwards when following the player.
	[SerializeField]
	private float _cameraTurnRate = 10f; // The rate at which the camera rotates by the mouse/right analog stick.
//
	public float cam_speed_coefficient = .5f;
	private float min_cam_distance = .5f;

	public Transform target; // What the camera holder will look at. This should be the player in most cases. Could be a gameobject floating above rem's shoulder.
	public Transform anchor; // When the camera gets pushed by walls, it loses it's original offset. We use this to move the camera back.
	private Transform wrapper; // This is to the parent in Start(). It has its transform anchor on the player, which is used for simple rotation around the player, 
	public Transform player;

	[SerializeField]
	private float _minDistance;	// The closest the camera should get to the target.
	[SerializeField]
	private float _maxDistance; // The farthest the camera should be from the target. The camera will always try to be at max distance.

	[SerializeField]
	public LayerMask _myLayerMask; // A layer mask where only the game layers the camera will try to avoid being behind are checked off
	[HideInInspector]
	public bool freezeCam = false;

	
	// Move the Camera Holder to the target's position and move the camera to our selected offset from the holder.
	void Start(){
		wrapper = anchor.parent;
		Debug.Log ("wha");
	}

	void FixedUpdate()
	{
		if (!freezeCam) { 
			// freezeCam gets set by the prefsPane manager currently, but should later refer to a singleton input manager I wanna make

			float inputRXAxis = Input.GetAxisRaw ("RHorizontal");
			float inputRYAxis = Input.GetAxisRaw ("RVertical");
		
			float rotation_step = _cameraTurnRate * Time.deltaTime;
			Debug.Log (wrapper);
			wrapper.RotateAround (player.position, Vector3.up, inputRXAxis * rotation_step);
			wrapper.RotateAround (player.position, wrapper.right, -1f * inputRYAxis * rotation_step);


			Vector3 target_to_cam = (anchor.position - target.position).normalized; // point toward the cam
			Vector3 nextposition;
			float distanceFromAnchor;
			RaycastHit hit;
			if (Physics.Raycast (target.position, target_to_cam, out hit, _maxDistance, _myLayerMask)) {
				Debug.Log ("Camera positioning raycast hitting " + LayerMask.LayerToName(hit.collider.gameObject.layer));
				distanceFromAnchor = ((target.position + ((hit.point - target.position) * .9f)) - transform.position).magnitude;

				nextposition = Vector3.MoveTowards(
					transform.position,
					(target.position + ((hit.point - target.position) * .9f)),
					Mathf.Min(
						(distanceFromAnchor * cam_speed_coefficient),
						min_cam_distance
					)
				);
				Debug.DrawRay (target.position, target_to_cam, Color.red);
				Debug.DrawLine (target.position, nextposition, Color.cyan);
			} else {
				Debug.Log ("Camera positioning at maximum distance");
				distanceFromAnchor = (anchor.position - transform.position).magnitude;

				nextposition = Vector3.MoveTowards(
					transform.position,
					anchor.position,
					Mathf.Min(
						(distanceFromAnchor * cam_speed_coefficient),
						min_cam_distance
					)
				);
				Debug.DrawRay (target.position, target_to_cam * _maxDistance, Color.red);
				Debug.DrawLine (target.position, nextposition, Color.cyan);
			}

			transform.position = nextposition;
			transform.rotation = anchor.transform.rotation;
		} else {
			Debug.Log ("Camera is frozen");
		}
	}
}
