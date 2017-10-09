using System;
using UnityEngine;
using UnityEngine.AI;

public class line_of_sight_ai : MonoBehaviour {

    public int MAX_DISTANCE = 10; /* Maximum distance raycast should travel */
    public float MIN_DISTANCE = 0.5f; /* Minimun distance before moving to next patrol point */
    public PatrolPoint PATROL_START;
    private PatrolPoint _patrol_current;
    
    private GameObject _player;
    private NavMeshAgent _agent;


	// Use this for initialization
	void Start () {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player");
        
        if (PATROL_START != null) {
            _agent.SetDestination(PATROL_START.transform.position);
            _patrol_current = PATROL_START;
        }
        
        _agent.autoBraking = false;
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        Physics.Raycast(transform.position, _player.transform.position - transform.position, out hit, MAX_DISTANCE);

        if (hit.collider != null && hit.collider.gameObject != null && hit.collider.gameObject.CompareTag("Player"))
            _agent.SetDestination(_player.transform.position);
        else if (!_agent.pathPending && _agent.remainingDistance < MIN_DISTANCE) {
            if (_patrol_current.NEXT != null)
                _patrol_current = _patrol_current.NEXT; // Move to next point
            else
                _patrol_current = PATROL_START; // Restart the patrol
            
            _agent.SetDestination(_patrol_current.transform.position);
        }
	}
}
