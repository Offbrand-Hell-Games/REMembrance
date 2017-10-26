using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    [SerializeField]
    private Rigidbody _rb;
    public float SPEED;
    public float JUMP_THRUST = 1850;
	public float JUMP_DURATION;
    public bool _isDashing = false;
    public Transform SAFEROOM_TRANSFORM;
	
	public GameObject DASH_NOT_READY_ICON;
	
	public MemoryScript MEMENTO; //TEMPORARY QUICK FIX DO NOT KEEP FOREVER
    Vector3 dashForce;
    Transform[] array;
    bool dashReady = true;
    public float DASH_COOLDOWN = 3f;
	
	enum VIEW {TD,OTS}; //Top-down, Over-the-shoulder
	private VIEW _view;
	
	public Camera OTS_CAMERA;
	public Camera TD_CAMERA;
	public Transform OTS_CAMERA_OFFSET;
	Vector3 offset;
	private Vector3 _offsetOffset;


    // Use this for initialization
    void Start()
    {
        PrefsPaneManager.instance.AddLivePreferenceFloat("Player Speed", 0f, 20f, SPEED, playerSpeedChanged);
        PrefsPaneManager.instance.AddLivePreferenceFloat("Dash Cooldown", 0f, 10f, DASH_COOLDOWN, updateDASH_COOLDOWN);
		EnterViewTD();
    }
	
	void EnterViewOTS()
	{
		_view = VIEW.OTS;
		OTS_CAMERA.gameObject.SetActive(true);
		TD_CAMERA.gameObject.SetActive(false);
		offset = new Vector3(-.5f,.8f,-4f);
		_offsetOffset = new Vector3(-.5f,0,0);
	}
	
	void EnterViewTD()
	{
		_view = VIEW.TD;
		OTS_CAMERA.gameObject.SetActive(false);
		TD_CAMERA.gameObject.SetActive(true);
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_isDashing == false)
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("DashableWall"), false);
			
			float inputHorizontal = Input.GetAxisRaw("Horizontal");
			float inputVertical = Input.GetAxisRaw("Vertical");
			float inputRHorizontal = Input.GetAxisRaw("RHorizontal");
			float inputRVertical = Input.GetAxisRaw("RVertical");
			
			Vector3 cameraForward = OTS_CAMERA.transform.forward;
			Vector3 cameraRight = OTS_CAMERA.transform.right;
			cameraForward.y = 0f;
			cameraRight.y = 0f;
			cameraForward.Normalize();
			cameraRight.Normalize();
			
			switch(_view)
			{
				case VIEW.OTS:
					Vector3 direction = cameraForward*inputVertical+cameraRight*inputHorizontal;
					direction = direction.normalized;
					_rb.velocity = new Vector3(SPEED * direction.x, 0, SPEED * direction.z);
					
					
					
					offset = Quaternion.AngleAxis(inputRHorizontal*2f,Vector3.up) * offset;
					_offsetOffset= Quaternion.AngleAxis(inputRHorizontal*2f,Vector3.up) * _offsetOffset;
					OTS_CAMERA.gameObject.transform.position = transform.position+offset;
					OTS_CAMERA_OFFSET.position = transform.position+_offsetOffset;
					Vector3 targetPosition = new Vector3(OTS_CAMERA_OFFSET.position.x, OTS_CAMERA.gameObject.transform.position.y,OTS_CAMERA_OFFSET.position.z);
					OTS_CAMERA.gameObject.transform.LookAt(targetPosition);
					//CAMERA.gameObject.transform.RotateAround(transform.position,Vector3.up,20f*Time.deltaTime);
					break;
				case VIEW.TD:
					direction = new Vector3(inputHorizontal,0f,inputVertical);
					direction = direction.normalized;
					_rb.velocity = new Vector3(SPEED * direction.x, 0, SPEED * direction.z);
					break;
			}
			
            if (Input.GetButton("Vision"))
            {
                StartCoroutine(FadeWalls(this.gameObject.transform.position, 5.0f));
            }

            if (dashReady && Input.GetButton("Dash"))
            {
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
				
				StartCoroutine(FadeWalls(this.gameObject.transform.position, 5.0f));
				dashReady = false;
				DASH_NOT_READY_ICON.SetActive(true);
				StartCoroutine(DashCountdown());
			}
			
            if (Input.GetButton("Interact"))
            {
                StartCoroutine(OpenDoors(this.gameObject.transform.position, 5.0f));
				if(MEMENTO.GetHeldBy() == MemoryScript.HeldBy.Player)
				{
					MEMENTO.SetHeldBy(MemoryScript.HeldBy.None);
				}
            }
        }
        else
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("DashableWall"), true);
            _rb.AddForce(dashForce * JUMP_THRUST);
			OTS_CAMERA.gameObject.transform.position = transform.position+offset;
			OTS_CAMERA_OFFSET.position = transform.position+_offsetOffset;
			Vector3 targetPosition = new Vector3(OTS_CAMERA_OFFSET.position.x, OTS_CAMERA.gameObject.transform.position.y,OTS_CAMERA_OFFSET.position.z);
			OTS_CAMERA.gameObject.transform.LookAt(targetPosition);
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
			if(MEMENTO.GetHeldBy() == MemoryScript.HeldBy.Player)
			{
				MEMENTO.SetHeldBy(MemoryScript.HeldBy.None);
			}
            transform.position = SAFEROOM_TRANSFORM.position;
        }
    }
}
