using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OTSCamera3 : MonoBehaviour
{
	//	[SerializeField]
	//	private float _cameraSpeed = 5f;	//The rate at which the camera moves forwards and backwards when following the player.
	[SerializeField]
	private float _cameraTurnRate = 10f; // The rate at which the camera rotates by the mouse/right analog stick.
	//
	public float cam_speed_coefficient = .5f;

	public Transform target; // What the camera holder will look at. This should be the player in most cases. Could be a gameobject floating above rem's shoulder.
	public Transform anchor; // When the camera gets pushed by walls, it loses it's original offset. We use this to move the camera back.
	public Transform camera;

	[SerializeField]
	private float _minDistance;	// The closest the camera should get to the target.
	[SerializeField]
	private float _maxDistance; // The farthest the camera should be from the target. The camera will always try to be at max distance.

	[SerializeField]
	public LayerMask _myLayerMask; // A layer mask where only the game layers the camera will try to avoid being behind are checked off
	[HideInInspector]
	public bool freezeCam = false;
	[SerializeField]
	public float _cameraVerticalMaxRotation = 155.0f;
	[SerializeField]
	public float _cameraVerticalMinRotation = 35.0f;

	private float x = 0f;
	private float y = 0f;

	// Move the Camera Holder to the target's position and move the camera to our selected offset from the holder.
	void Start(){
		Cursor.visible = false;
	}

	void FixedUpdate()
	{
		if (freezeCam) {
//			Debug.Log ("Camera frozen/paused");
		} else {
			float inputRXAxis = Input.GetAxisRaw ("RHorizontal");
			float inputRYAxis = Input.GetAxisRaw ("RVertical");

			x += (inputRXAxis * _cameraTurnRate * Time.deltaTime);
			y -= (inputRYAxis * _cameraTurnRate * Time.deltaTime);

			if (y > _cameraVerticalMaxRotation) {
				y = _cameraVerticalMaxRotation;
			} else if (y < _cameraVerticalMinRotation) {
				y = _cameraVerticalMinRotation;
			}

			var rotation = Quaternion.Euler (y, x, 0);
			var position = rotation * transform.position;

			transform.rotation = rotation;

			RaycastHit hit;
			Debug.DrawRay (target.position, (anchor.position - target.position), Color.red);

			if (Physics.Raycast (target.position, (anchor.position - target.position), out hit, _maxDistance, _myLayerMask)) {
//				Debug.Log ("Collision: " + hit.point);
				camera.position = target.position + ((hit.point - target.position) * .9f);
			} else {
//				Debug.Log ("No collision: " + hit.point);
				camera.position = anchor.position;
			}
		}
	}



}
