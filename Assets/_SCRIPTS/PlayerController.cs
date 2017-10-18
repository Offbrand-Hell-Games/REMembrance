using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    [SerializeField]
    private Rigidbody _rb;
    public float speed;
    public float jumpThrust = 1850;
    public bool _isDashing = false;
    public Transform SAFEROOM_TRANSFORM;
    Vector3 dashForce;
    Transform[] array;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_isDashing == false)
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("DashableWall"), false);
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                _rb.velocity = new Vector3(speed, 0, _rb.velocity.z);
            }
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                _rb.velocity = new Vector3(-speed, 0, _rb.velocity.z);
            }
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, 0, speed);
            }
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, 0, -speed);
            }
            if (Input.GetKeyDown("e"))
            {
                StartCoroutine(FadeWalls(this.gameObject.transform.position, 5.0f));
            }
            if (Input.GetKeyDown("space"))
            {
                _isDashing = true;
                dashForce = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                StartCoroutine(FadeWalls(this.gameObject.transform.position, 5.0f));
            }
            if (Input.GetKeyDown("f"))
            {
                StartCoroutine(OpenDoors(this.gameObject.transform.position, 5.0f));
            }
        }
        else
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("DashableWall"), true);
            _rb.AddForce(dashForce * jumpThrust);
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
	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Enemy")
		{
			transform.position = SAFEROOM_TRANSFORM.position;
		}
	}
	
	
}
