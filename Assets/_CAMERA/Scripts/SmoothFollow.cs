using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
	{

	[SerializeField]
	private float _xMin = 0f;
	[SerializeField]
	private float _xMax = 0f;
	[SerializeField]
	private float _zMin = 0f;
	[SerializeField]
	private float _zMax = 0f;
	[SerializeField]
	private float _cameraSpeed = 0f;
	
	private float _yOffset;
	private float _zOffset;
	
	
	// The transforms of the players. Used to determine their midpoint.
	public Transform playerTransform;
	
	void Start(){
		_yOffset = transform.position.y - playerTransform.position.y;
		_zOffset = transform.position.z - playerTransform.position.z;
		PrefsPaneManager.instance.AddLivePreferenceFloat ("Camera Speed", 0f, 10f, _cameraSpeed, cameraSpeedChanged);
	}
		

	void Update(){
		Vector3 target = new Vector3(
			playerTransform.position.x,
			playerTransform.position.y+_yOffset,
			playerTransform.position.z+_zOffset
		);
		
		float step = _cameraSpeed * Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, target, step);
		
		// Apply clamps to position
		transform.position = new Vector3(
			Mathf.Clamp(transform.position.x, _xMin, _xMax),
			_yOffset,
			Mathf.Clamp(transform.position.z, _zMin, _zMax)
		);
	}

	public void cameraSpeedChanged(float value) {
		_cameraSpeed = value;
	}

}