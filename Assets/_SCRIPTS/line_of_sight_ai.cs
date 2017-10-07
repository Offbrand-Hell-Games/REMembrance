using System;
using UnityEngine;
using UnityEngine.AI;

public class line_of_sight_ai : MonoBehaviour {

    public int MAX_DISTANCE; /* Maximum distance raycast should travel */
    
    private GameObject _player;
    private NavMeshAgent _agent;


	// Use this for initialization
	void Start () {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        Physics.Raycast(transform.position, _player.transform.position - transform.position, out hit, MAX_DISTANCE);

        if (hit.collider != null && hit.collider.gameObject != null && hit.collider.gameObject.CompareTag("Player"))
            _agent.SetDestination(_player.transform.position);
	}
}
