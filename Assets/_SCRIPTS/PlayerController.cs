using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    [SerializeField]
    private Rigidbody _rb;
    public float SPEED;
    public float JUMP_THRUST = 1850;
    public bool _isDashing = false;
    public Transform SAFEROOM_TRANSFORM;
	
	public MemoryScript MEMENTO; //TEMPORARY QUICK FIX DO NOT KEEP FOREVER
    Vector3 dashForce;
    Transform[] array;
    bool dashReady = true;
    float dashCooldown = 3f;


    // Use this for initialization
    void Start()
    {
        PrefsPaneManager.instance.AddLivePreferenceFloat("Player Speed", 0f, 20f, speed, playerSpeedChanged);
        PrefsPaneManager.instance.AddLivePreferenceFloat("Dash Cooldown", 0f, 10f, dashCooldown, updateDashCooldown);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDashing == false)
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("DashableWall"), false);
			Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"),0f,Input.GetAxisRaw("Vertical"));
			
			direction = direction.normalized;
			_rb.velocity = new Vector3(SPEED * direction.x, 0, SPEED * direction.z);
			
            if (Input.GetButton("Vision"))
            {
                StartCoroutine(FadeWalls(this.gameObject.transform.position, 5.0f));
            }

            if (dashReady)
            {
                if (Input.GetKeyDown("space"))
                {
                    _isDashing = true;
                    dashForce = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                    StartCoroutine(FadeWalls(this.gameObject.transform.position, 5.0f));
                    dashReady = false;
                    StartCoroutine(DashCountdown());
                }

            if (Input.GetButton("Dash"))
            {
                _isDashing = true;
                dashForce = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
				dashForce = dashForce.normalized;
                StartCoroutine(FadeWalls(this.gameObject.transform.position, 5.0f));
            }
            if (Input.GetButton("Interact"))
            {
                StartCoroutine(OpenDoors(this.gameObject.transform.position, 5.0f));
            }
        }
        else
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("DashableWall"), true);
            _rb.AddForce(dashForce * JUMP_THRUST);
        }
		if(Input.GetKey("r"))
		{
			Application.LoadLevel(Application.loadedLevel);
		}

    }
	
	void FixedUpdate()
	{
		
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
        yield return new WaitForSeconds(0.2f);
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
        yield return new WaitForSeconds(dashCooldown);
        dashReady = true;
        print("Dash is ready after waiting " + dashCooldown);
        yield return null;
    }
	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Enemy")
		{
			transform.position = SAFEROOM_TRANSFORM.position;
		}
	}
	
    public void playerSpeedChanged(float value)
    {
        speed = value;
    }
    public void updateDashCooldown(float value)
    {
        dashCooldown = value;
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
