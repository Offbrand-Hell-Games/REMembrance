using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MementoManager : MonoBehaviour {

	private GameObject[] _mementos;

	// Use this for initialization
	void Start () {
		_mementos = GameObject.FindGameObjectsWithTag("Memento");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public GameObject GetClosestMemento(Vector3 pos)
	{
		GameObject closest = _mementos[0];
		float min_distance = Vector3.Distance(pos, closest.transform.position);
        foreach (GameObject m in _mementos) {
            float d = Vector3.Distance(pos, m.transform.position);
            if(d < min_distance) {
                closest = m;
                min_distance = d;
            }
        }
		return closest;
	}

}
