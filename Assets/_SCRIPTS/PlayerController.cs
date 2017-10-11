using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( BoxCollider2D ), typeof( Rigidbody2D ) )]
public class PlayerController : MonoBehaviour {
	[SerializeField]
	private Rigidbody _rb;
	public float speed;
    public float jumpDistance = 10;
    bool _isDashing = false;
    Vector3 targetPosition;
    Transform[] array;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update ()
    {
	    if(_isDashing == false)
        {
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
                
                float left_to_right = (transform.position.x + (jumpDistance * Input.GetAxisRaw("Horizontal")));
                float up_to_down = (transform.position.z + (jumpDistance * Input.GetAxisRaw("Vertical")));

                _isDashing = true;
                targetPosition = new Vector3(left_to_right, 0, up_to_down);
                StartCoroutine(FadeWalls(this.gameObject.transform.position, 5.0f));
                //print(targetPosition);
            }
        }
        else
        {
            if(transform.position != targetPosition)
            {
                float step = (3 *speed) * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            }
            else
            {
                _isDashing = false;
            }

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
                    hitColliders[i].SendMessage("Disable_Collision");
                    //print("Disable collision");
                } 
            }
            if(hitColliders[i].tag == "Memory")
            {
                if (_isDashing)
                    hitColliders[i].SendMessage("Drop");
            }
            i++;
            yield return null;
        }
        StopCoroutine(FadeWalls(this.gameObject.transform.position, 4.0f));
    }
}
