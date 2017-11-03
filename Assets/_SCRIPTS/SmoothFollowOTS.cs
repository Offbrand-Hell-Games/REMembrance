using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SmoothFollowOTS : MonoBehaviour
{
	[SerializeField]
	private float _cameraSpeed = 5f;	//The rate at which the camera moves forwards and backwards when following the player.
	[SerializeField]
	private float _cameraTurnRate = 20f; // The rate at which the camera rotates by the mouse/right analog stick.
	public Transform CAMERA_HOLDER;	//The camera's parent. We use a camera holder so the camera itself can always be offset.
	public Transform TARGET; // What the camera holder will look at. This should be the player in most cases.
	
	private bool _isTouchingWall = false;
	[SerializeField]
	private float _minDistance;	// The closest the camera should get to the target.
	[SerializeField]
	private float _maxDistance; // The farthest the camera should be from the target.
	[SerializeField]
	private Vector3 _offset; // Offset of the camera. Edit the X value in the editor to move the camera left/right of the target.


	public bool freezeCam = true;
	//public Text distance;
	
	// Move the Camera Holder to the target's position and move the camera to our selected offset from the holder.
	void Start(){
		CAMERA_HOLDER.position = TARGET.position;
		transform.position = new Vector3(CAMERA_HOLDER.position.x+_offset.x,CAMERA_HOLDER.position.y+_offset.y,CAMERA_HOLDER.position.z+_offset.z);
	}
	
	void FixedUpdate()
	{
//		if (Input.GetKeyDown (KeyCode.Escape)) {
//			freezeCam = !freezeCam;
//		} 
			
		if (!freezeCam) {
			//distance.text = Vector3.Distance(CAMERA_HOLDER.position,TARGET.position).ToString();
			float inputRXAxis = Input.GetAxisRaw ("RHorizontal");
			float inputRYAxis = Input.GetAxisRaw ("RVertical");
		
			float step = _cameraSpeed * Time.deltaTime;
		
		
			if (_isTouchingWall && Vector3.Distance (CAMERA_HOLDER.position, TARGET.position) > _minDistance) {
				//Move towards the player
//			CAMERA_HOLDER.position = Vector3.Lerp(CAMERA_HOLDER.position, TARGET.position, step);
			} else if (Vector3.Distance (CAMERA_HOLDER.position, TARGET.position) < _maxDistance || Vector3.Distance (CAMERA_HOLDER.position, TARGET.position) > _maxDistance) {
				//Move away from the player
				Vector3 backwardsVector = new Vector3 (CAMERA_HOLDER.position.x - TARGET.position.x, CAMERA_HOLDER.position.y - TARGET.position.y, CAMERA_HOLDER.position.z - TARGET.position.z).normalized * _maxDistance;
				Vector3 inverseTarget = TARGET.position + backwardsVector;
				CAMERA_HOLDER.position = Vector3.Lerp (CAMERA_HOLDER.position, inverseTarget, step);
			}
		
			//Rotate around the player according to the right analog.
			float rotationStep = _cameraTurnRate * Time.deltaTime;
			CAMERA_HOLDER.RotateAround (TARGET.position, Vector3.up, inputRXAxis * rotationStep);
			CAMERA_HOLDER.RotateAround (TARGET.position, Vector3.right, inputRYAxis * rotationStep);
			CAMERA_HOLDER.LookAt (TARGET.position);
		}
	}

//	if (_isTouchingWall && Vector3.Distance (CAMERA_HOLDER.position, TARGET.position) > _minDistance) {
//		//Move towards the player
//		//			CAMERA_HOLDER.position = Vector3.Lerp(CAMERA_HOLDER.position, TARGET.position, step);
//	} else if (Vector3.Distance (transform.position, TARGET.position) < _maxDistance || Vector3.Distance (transform.position, TARGET.position) > _maxDistance) {
//		//Move away from the player
//		//				Vector3 backwardsVector = new Vector3 (CAMERA_HOLDER.position.x - TARGET.position.x, CAMERA_HOLDER.position.y - TARGET.position.y, CAMERA_HOLDER.position.z - TARGET.position.z).normalized * _maxDistance;
//		//				Vector3 inverseTarget = TARGET.position + backwardsVector;
//		//				CAMERA_HOLDER.position = Vector3.Lerp (CAMERA_HOLDER.position, inverseTarget, step);
//		Vector3 backwardsVector = TARGET.transform.forward * -1f;
//		Vector3 inverseTarget = TARGET.position + backwardsVector * _maxDistance;
//		RaycastHit hit;
//		Physics.Raycast (TARGET.position, backwardsVector, out hit, _maxDistance);
//		Vector3 nextposition = TARGET.position + (backwardsVector * _maxDistance * .9f);
//		Debug.DrawRay (TARGET.position, nextposition);
//		Debug.DrawRay (TARGET.position, hit.point);
//		transform.position = Vector3.Lerp(transform.position, TARGET.position + (backwardsVector * _maxDistance * .9f), step);
//		//				CAMERA_HOLDER.position = Vector3.Lerp (CAMERA_HOLDER.position, inverseTarget, step);
//	}
//
//	//Rotate around the player according to the right analog.
//	float rotationStep = _cameraTurnRate * Time.deltaTime;
//	transform.RotateAround (TARGET.position, Vector3.up, inputRXAxis * rotationStep);
//	transform.RotateAround (TARGET.position, Vector3.right, inputRYAxis * rotationStep);
//	transform.LookAt (TARGET.position);
	
	void OnColliderEnter(Collision coll)
	{
		_isTouchingWall = true;
	}
	
	void OnColliderExit(Collision coll)
	{
		_isTouchingWall = false;
	}
	
	void OnTriggerEnter(Collider coll)
	{
		_isTouchingWall = true;
	}
	
	void OnTriggerExit(Collider coll)
	{
		_isTouchingWall = false;
	}
}
