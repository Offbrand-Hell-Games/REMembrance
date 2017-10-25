using UnityEngine;
using System.Collections;

public class SmoothFollowOTS : MonoBehaviour
	{

	[SerializeField]
	private float _cameraSpeed = 0f;
	
	private float _xOffset;
	private float _yOffset;
	private float _zOffset;
	
	
	// The transforms of the players. Used to determine their midpoint.
	public Transform playerTransform;
	
	void Start(){
		_xOffset = 2f;
		_yOffset = transform.position.y - playerTransform.localPosition.y;
		_zOffset = transform.position.z - playerTransform.localPosition.z;
		//PrefsPaneManager.instance.AddLivePreferenceFloat ("Camera Speed", 0f, 10f, _cameraSpeed, cameraSpeedChanged);
	}
		

	void Update(){
		Vector3 target = new Vector3(
			playerTransform.position.x+_xOffset,
			playerTransform.position.y+_yOffset,
			playerTransform.position.z+_zOffset
		);
		
		float step = _cameraSpeed * Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, target, step);
	}

	public void cameraSpeedChanged(float value) {
		_cameraSpeed = value;
	}

}