using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class line_of_sight_ai : MonoBehaviour {

    public int MAX_DISTANCE = 10; /* Maximum distance raycast should travel */
    public float MIN_DISTANCE = 0.5f; /* Minimun distance before moving to next patrol point */
    public int PATROL_GROUP = 0; /* AI will only follow patrols in the assigned group */

    public PatrolPoint PATROL_START;
    private PatrolPoint _patrol_current;
    private bool _targeting_player;
    
    private GameObject _player;
    private NavMeshAgent _agent;
    private PatrolManager _patrol_manager;


	// Use this for initialization
	void Start () {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _patrol_manager = GameObject.Find("GameManager").GetComponent<PatrolManager>();;
        _targeting_player = false;
        
        if (PATROL_START != null) {
            /* Initialize patrol start point */
            _agent.SetDestination(PATROL_START.transform.position);
            _patrol_current = PATROL_START;
        } else {
            Debug.Log("<color=blue>AI Error: This AI does not have a patrol set!</color>");
        }
        
        _agent.autoBraking = false;
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        Physics.Raycast(transform.position, _player.transform.position - transform.position, out hit, MAX_DISTANCE);

        if (hit.collider != null && hit.collider.gameObject != null && hit.collider.gameObject.CompareTag("Player")) {
            _agent.SetDestination(_player.transform.position);
            _targeting_player = true;
        }
        else if (!_agent.pathPending && _agent.remainingDistance < MIN_DISTANCE) {
            if (_targeting_player) {
                _targeting_player = false;
                SetTargetToNearestPoint();
            } else if (_patrol_current != null) { // Otherwise, move to next patrol point
                if (_patrol_current.NEXT != null)
                    _patrol_current = _patrol_current.NEXT; // Move to next point
                else
                    _patrol_current = PATROL_START; // Restart the patrol
                
                _agent.SetDestination(_patrol_current.transform.position);
            }
        }
	}

    public void SetTargetToPlayer() {
        _agent.SetDestination(_player.transform.position);
        _targeting_player = true;
    }

    public void SetTargetToNearestPoint() {
        List<PatrolPoint> possible_points = _patrol_manager.GetPatrolPointsByGroupID(PATROL_GROUP);
        if(possible_points.Count == 0) {
            Debug.Log("<color=blue>AI Error:</color>Found no patrol points belonging to this AI's Patrol Group!"
                + "Did you remember to assign a group to this AI, as well as the patrol points you want it tied to?", gameObject);
            return;
        }
        PatrolPoint closest = possible_points[0];
        float min_distance = Vector3.Distance(transform.position, closest.transform.position);
        foreach (PatrolPoint p in possible_points) {
            float d = Vector3.Distance(transform.position, p.transform.position);
            if(d < min_distance) {
                closest = p;
                min_distance = d;
            }
        }
        _patrol_current = closest;
        PATROL_START = _patrol_current;
        _agent.SetDestination(_patrol_current.transform.position);
    }
}
