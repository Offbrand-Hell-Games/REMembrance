using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MementoPoint : MonoBehaviour {

	[HideInInspector]
	public Memento MEMENTO = null;

	public MementoPoint NEST_TO_TAKE_FROM;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Memento")
		{
			MEMENTO = collider.gameObject.GetComponent<Memento>();
			MEMENTO.IN_NEST = true;
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.tag == "Memento")
		{
			MEMENTO.IN_NEST = false;
			MEMENTO = null;
		}
	}
}
