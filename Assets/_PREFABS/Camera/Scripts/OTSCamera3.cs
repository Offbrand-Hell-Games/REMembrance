using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Rewired;

public class OTSCamera3 : MonoBehaviour
{
    //	[SerializeField]
    //	private float _cameraSpeed = 5f;	//The rate at which the camera moves forwards and backwards when following the player.

    public int REWIRED_PLAYERID = 0;    //used for controller mapping
    public int REWIRED_PLAYERID_MOUSECAMERA = 1;    //used for controller mapping
    private Player _player;             //used for controller mapping
    private Player _player_mousecamera;             //used for controller mapping

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


    private void Awake()
    {
        _player = ReInput.players.GetPlayer(REWIRED_PLAYERID);   //controller mapping
        _player_mousecamera = ReInput.players.GetPlayer(REWIRED_PLAYERID_MOUSECAMERA);   //controller mapping
    }

    // Move the Camera Holder to the target's position and move the camera to our selected offset from the holder.
    void Start(){
		Cursor.visible = false;
	}

	void FixedUpdate()
	{
		if (freezeCam) {
//			Debug.Log ("Camera frozen/paused");
		} else {

            float inputRXAxis = _player.GetAxis("CameraHorizontal");
            float inputRYAxis = _player.GetAxis("CameraVertical");

            // no input from joystick camera, look for mouse instead
            if (inputRXAxis == 0 && inputRYAxis == 0)
            {
                inputRXAxis = _player_mousecamera.GetAxis("CameraHorizontal");
                inputRYAxis = _player_mousecamera.GetAxis("CameraVertical");

                x += (inputRXAxis * _cameraTurnRate * Time.deltaTime);
                y -= (inputRYAxis * _cameraTurnRate * Time.deltaTime);
            }
            // joystick
            else {
                x += (inputRXAxis * _cameraTurnRate * 15.0f * Time.deltaTime);
                y -= (inputRYAxis * _cameraTurnRate * 15.0f * Time.deltaTime);
            }

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

			if (Physics.Raycast (target.position, (anchor.position - target.position), out hit, Vector3.Distance(anchor.position, target.position), _myLayerMask)) {
//				Debug.Log ("Collision: " + hit.point);
				camera.position = target.position + ((hit.point - target.position) * .95f);
			} else {
//				Debug.Log ("No collision: " + hit.point);
				camera.position = anchor.position;
			}
		}
	}



}
