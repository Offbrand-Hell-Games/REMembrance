using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    [SerializeField]
    private Rigidbody _rb;
    public float SPEED; 				//Player speed
    public float JUMP_THRUST = 1850; 	//Player jump thrust
	public float JUMP_DURATION;			//Player jump duration
    public bool _isDashing = false;
    public Transform SAFEROOM_TRANSFORM;
	
	public GameObject DASH_NOT_READY_ICON;
	
	private Memento _memento = null;
    private float _timeReleasedMemento;
    public float MEMENTO_PICKUP_COOLDOWN = 2.0f;
    Vector3 dashForce;
    Transform[] array;
    bool dashReady = true;
    public float DASH_COOLDOWN = 3f;
	
	enum VIEW {TD,OTS}; //Top-down, Over-the-shoulder
	private VIEW _view;
	
	public GameObject OTS_CAMERA;
	public GameObject TD_CAMERA;

	public GameObject xRayVisionA;
	public GameObject xRayVisionB;

	public GameObject RemDashTrail;
	
	public GameObject REM_MODEL;


    // Use this for initialization
    void Start()
    {
//		dashTrailParticleEmitter.GetComponent<ParticleSystem> ().Stop ();
        PrefsPaneManager.instance.AddLivePreferenceFloat("Player Speed", 0f, 20f, SPEED, playerSpeedChanged);
        PrefsPaneManager.instance.AddLivePreferenceFloat("Dash Cooldown", 0f, 10f, DASH_COOLDOWN, updateDASH_COOLDOWN);
		EnterViewOTS();
        _timeReleasedMemento = Time.time;
    }
	
	// Switch from Topdown camera to Overthe soulder
	void EnterViewOTS()
	{
		_view = VIEW.OTS;
		OTS_CAMERA.SetActive(true);
		TD_CAMERA.SetActive(false);
	}
	
	//Switch from Over the shoulder camera to top down
	void EnterViewTD()
	{
		_view = VIEW.TD;
		OTS_CAMERA.SetActive(false);
		TD_CAMERA.SetActive(true);
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		float inputHorizontal = Input.GetAxisRaw("Horizontal");
		float inputVertical = Input.GetAxisRaw("Vertical");
		float inputRHorizontal = Input.GetAxisRaw("RHorizontal");
		float inputRVertical = Input.GetAxisRaw("RVertical");
        if (_isDashing == false)
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("DashableWall"), false);
			
			
			//Get "forward" and "right" from the perspective of the camera. Used by OTS Camera.
			Vector3 cameraForward = OTS_CAMERA.transform.forward;
			Vector3 cameraRight = OTS_CAMERA.transform.right;
			cameraForward.y = 0f;
			cameraRight.y = 0f;
			cameraForward.Normalize();
			cameraRight.Normalize();
			
			switch(_view)
			{
				case VIEW.OTS:
					//If the player presses W / Left analog up, the player should move forward relative to the camera.
					Vector3 direction = cameraForward*inputVertical+cameraRight*inputHorizontal;
					direction = direction.normalized;
					_rb.velocity = new Vector3(SPEED * direction.x, 0, SPEED * direction.z);
					if(direction != Vector3.zero){
						Vector3 remLook = new Vector3(transform.position.x + direction.x, REM_MODEL.transform.position.y, transform.position.z + direction.z);
						REM_MODEL.transform.LookAt(remLook);
					}
					break;
				case VIEW.TD:
					//If the player presses W / Left Ananlog UP, the player should move north.
					direction = new Vector3(inputHorizontal,0f,inputVertical);
					direction = direction.normalized;
					_rb.velocity = new Vector3(SPEED * direction.x, 0, SPEED * direction.z);
					if(direction != Vector3.zero){
						Vector3 remLook = new Vector3(transform.position.x + direction.x, REM_MODEL.transform.position.y, transform.position.z + direction.z);
						REM_MODEL.transform.LookAt(remLook);
					}
					break;
			}
			
            if (Input.GetButtonDown("Vision"))
            {
                if (_memento != null)
                {
                    _memento.OnPlayerAbilityUsed();
                }
				xRayVisionA.SetActive (!xRayVisionA.activeSelf);
				xRayVisionB.SetActive (!xRayVisionB.activeSelf);
//                StartCoroutine(FadeWalls(this.gameObject.transform.position, 5.0f));
            }

            if (dashReady && Input.GetButtonDown("Dash"))
            {
				GameObject trail = Instantiate (RemDashTrail, transform);
				trail.SetActive (true);

                if (_memento != null)
                {
                    _memento.OnPlayerAbilityUsed();
                }
				_isDashing = true;
				switch(_view)
				{
					case VIEW.OTS:
						dashForce = cameraForward*inputVertical+cameraRight*inputHorizontal;
						dashForce = dashForce.normalized;
						break;
					case VIEW.TD:
						dashForce = new Vector3(inputHorizontal, 0, inputVertical);
						dashForce = dashForce.normalized;
						break;
				}

				StartCoroutine(StopDashEffects());   // dashReady = false;
				DASH_NOT_READY_ICON.SetActive(true);
				StartCoroutine(DashCountdown());
			}
			
            if (Input.GetButtonDown("Door"))
            {
                StartCoroutine(OpenDoors(this.gameObject.transform.position, 5.0f));
            }
			
			if (Input.GetButtonDown("Drop"))
            {
                StartCoroutine(OpenDoors(this.gameObject.transform.position, 5.0f));
				if(_memento != null)
				{
					_memento.Release();
                    _memento = null;
                    _timeReleasedMemento = Time.time;
				}
            }
        }
        else
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("DashableWall"), true);
            _rb.AddForce(dashForce * JUMP_THRUST);
        }
		
		
		
		
		if(Input.GetKeyDown("r"))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
		if(Input.GetKeyDown("v"))
		{
			if(_view == VIEW.OTS)
				EnterViewTD();
			else
				EnterViewOTS();
		}

    }

    public IEnumerator FadeWalls(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Wall")
            {
                //print(hitColliders[i].name);
                hitColliders[i].SendMessage("Fade");
                if (_isDashing)
                {
                    //hitColliders[i].SendMessage("Disable_Collision");
                    //print("Disable collision");
                }
            }
            if (hitColliders[i].tag == "Enemy")
            {
                hitColliders[i].SendMessage("ChangeMat");
            }
            i++;
            yield return null;
        }
        yield return new WaitForSeconds(JUMP_DURATION);
        _isDashing = false;
		StopCoroutine(FadeWalls(this.gameObject.transform.position, 2.0f));
    }
	
    public IEnumerator OpenDoors(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Door")
            {
                //print(hitColliders[i].name);
                hitColliders[i].SendMessage("ChangeDoorState");    
            }
            i++;
            yield return null;
        }
        yield return null;
        StopCoroutine(OpenDoors(center, radius));
    }

    public IEnumerator DashCountdown()
    {
        yield return new WaitForSeconds(DASH_COOLDOWN);
        dashReady = true;
		DASH_NOT_READY_ICON.SetActive(false);
        print("Dash is ready after waiting " + DASH_COOLDOWN);
        yield return null;
    }



	public IEnumerator StopDashEffects() {
//		Debug.Log ("ooooh");
		yield return new WaitForSeconds(JUMP_DURATION);
		_isDashing = false;
//		Debug.Log ("hey");
//		yield return new WaitForSeconds (dashTrailParticleEmitter.GetComponent<ParticleSystem> ().main.duration - JUMP_DURATION); 
//		Debug.Log ("hey again");
//		dashTrailParticleEmitter.GetComponent<ParticleSystem> ().Clear();
//		dashTrailParticleEmitter.GetComponent<ParticleSystem> ().Stop();
	}

    public void playerSpeedChanged(float value)
    {
        SPEED = value;
    }
    public void updateDASH_COOLDOWN(float value)
    {
        DASH_COOLDOWN = value;
    }
	
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
			if(_memento != null)
			{
				_memento.Release();
                _memento = null;
			}
            transform.position = SAFEROOM_TRANSFORM.position;
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
         /* This code feels dirty.
            Memento's trigger collider is on the child object, so we need
              to access its parent through its transform
        */
        Transform parent = collider.gameObject.transform.parent;
        if (parent != null && parent.tag == "Memento" && Time.time - _timeReleasedMemento >= MEMENTO_PICKUP_COOLDOWN)
        {
            _memento = parent.gameObject.GetComponent<Memento>();
            _memento.Bind(this.gameObject);
        }
    }

    public void OnMementoCollisionWithWall(Memento memento)
    {
        if (_memento == memento)
        {
            _memento.Release();
            _memento = null;
        }
    }
}
