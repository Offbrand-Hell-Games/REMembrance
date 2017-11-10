using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Author: Corwin Belser
///	Used by EnemyController and Memento to make decisions. Provides
///		access to the memento held in this nest (if any), and access
///		to the nest linked to this one (if one is linked)
public class MementoPoint : MonoBehaviour {

	[HideInInspector]
	public Memento MEMENTO = null; /* The memento currently in this nest (null if none) */

	public MementoPoint NEST_TO_TAKE_FROM; /* Nest linked to this one. */

	/// <summary> Checks if a memento enters this nest, storing a reference if so </summary>
	/// <param name="collider"> Collider: collider that is currently colliding with this nest </param>
	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Memento")
		{
			MEMENTO = collider.gameObject.GetComponent<Memento>();
			MEMENTO.IN_NEST = true;
		}
	}

	/// <summary> Checks if a memento exits this nest, removing the reference if so </summary>
	/// <param name="collider"> Collider: collider that is no longer colliding with this nest </param>
	void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.tag == "Memento")
		{
			MEMENTO.IN_NEST = false;
			MEMENTO = null;
		}
	}
}
