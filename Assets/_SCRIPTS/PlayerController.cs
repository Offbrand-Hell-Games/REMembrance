using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( BoxCollider2D ), typeof( Rigidbody2D ) )]
public class PlayerController : MonoBehaviour {
	[SerializeField]
	private Rigidbody _rb;
	public float speed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetAxisRaw("Horizontal")>0 )
		{
			_rb.velocity = new Vector3(speed,0,_rb.velocity.z);
        }
		if( Input.GetAxisRaw("Horizontal")<0 )
		{
			_rb.velocity = new Vector3(-speed,0,_rb.velocity.z);
        }
		if( Input.GetAxisRaw("Vertical")>0)
		{
			_rb.velocity = new Vector3(_rb.velocity.x,0,speed);
		}
		if( Input.GetAxisRaw("Vertical")<0)
		{
			_rb.velocity = new Vector3(_rb.velocity.x,0,-speed);
		}
	}
}
