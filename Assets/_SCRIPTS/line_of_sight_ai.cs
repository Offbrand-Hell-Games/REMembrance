using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class line_of_sight_ai : MonoBehaviour {

    public int MAX_DISTANCE = 10; /* Maximum distance raycast should travel */
    public float MIN_DISTANCE = 0.5f; /* Minimun distance before moving to next patrol point */
    public int PATROL_GROUP = 0; /* AI will only follow patrols in the assigned group */

    public PatrolPoint PATROL_START;
    public MementoPoint MEMENTO_DROPOFF;

    private PatrolPoint _patrol_current;
    private bool _targeting_player;
    private bool _has_memento;
    
    private GameObject _player;
    private NavMeshAgent _agent;
    private PatrolManager _patrol_manager;
    private MementoManager _memento_manager;
    private PhaseManager _phase_manager;


	// Use this for initialization
	void Start () {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _patrol_manager = GameObject.Find("GameManager").GetComponent<PatrolManager>();
        _memento_manager = GameObject.Find("GameManager").GetComponent<MementoManager>();
        _phase_manager = GameObject.Find("GameManager").GetComponent<PhaseManager>();
        _targeting_player = false;
        _has_memento = false;
        
        if (PATROL_START != null) {
            /* Initialize patrol start point */
            _agent.SetDestination(PATROL_START.transform.position);
            _patrol_current = PATROL_START;
        } else {
            //Debug.Log("<color=blue>AI Error: This AI does not have a patrol set!</color>");
        }
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        Physics.Raycast(transform.position, _player.transform.position - transform.position, out hit, MAX_DISTANCE);

        if (hit.collider != null && hit.collider.gameObject != null && hit.collider.gameObject.CompareTag("Player")) {
            SetTargetToPlayer();
        } else if (!_has_memento && _phase_manager.GetGameState() == PhaseManager.GameState.Escape && !MEMENTO_DROPOFF.HAS_MEMENTO) {
            //Check for any memento's out of MementoPoints
            GameObject memento = _memento_manager.GetClosestMemento(transform.position);
            MemoryScript ms = memento.GetComponent<MemoryScript>();
            if(ms != null && ms.GetHeldBy() == MemoryScript.HeldBy.None /* || ms.GetHeldBy() == MemoryScript.HeldBy.Player */) {
                //Debug.Log("<color=blue>AI: Setting Target To Memento</color>");
                _agent.SetDestination(memento.transform.position);
            }
        }
        if (!_agent.pathPending && _agent.remainingDistance < MIN_DISTANCE) {

            if (_targeting_player && !_has_memento) {
                _targeting_player = false;
                SetTargetToNearestPoint();
            } else if (_has_memento) {
                MemoryScript ms = _memento_manager.GetClosestMemento(transform.position).GetComponent<MemoryScript>();
                StartCoroutine(ms.Release(MEMENTO_DROPOFF));
                _has_memento = false;
                MEMENTO_DROPOFF.HAS_MEMENTO = true;
                SetTargetToNearestPoint();
            } else if (_patrol_current != null) { // Otherwise, move to next patrol point
                if (_patrol_current.NEXT != null)
                    _patrol_current = _patrol_current.NEXT; // Move to next point
                else
                    _patrol_current = PATROL_START; // Restart the patrol
                //Debug.Log("<color=blue>AI: Setting Target to Next Patrol Point " + _patrol_current.name + "</color>");
                _agent.SetDestination(_patrol_current.transform.position);
            }
        }
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Memento" && collision.gameObject.GetComponent<MemoryScript>().GetHeldBy() != MemoryScript.HeldBy.Cooldown) {
            //Debug.Log("<color=blue>AI: Has Memento</color>");
            _has_memento = true;
            _agent.SetDestination(MEMENTO_DROPOFF.transform.position);
        }
    }

    public void SetTargetToPlayer() {
        //If we have the memento, drop it
        if (_has_memento)
        {
            //Debug.Log("<color=blue>AI: Dropping Memento</color>");
            MemoryScript ms = _memento_manager.GetClosestMemento(transform.position).GetComponent<MemoryScript>();
            StartCoroutine(ms.Release(null));
            _has_memento = false;
        }
        //Debug.Log("<color=blue>AI: Setting Target to Player</color>");
        _agent.SetDestination(_player.transform.position);
        _targeting_player = true;
    }

    public void SetTargetToNearestPoint() {
        List<PatrolPoint> possible_points = _patrol_manager.GetPatrolPointsByGroupID(PATROL_GROUP);
        if(possible_points.Count == 0) {
            //Debug.Log("<color=blue>AI Error:</color>Found no patrol points belonging to this AI's Patrol Group!"
                //+ "Did you remember to assign a group to this AI, as well as the patrol points you want it tied to?", gameObject);
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
        //Debug.Log("<color=blue>AI: Setting Target to Patrol Point " + _patrol_current.name + "</color>");
        _agent.SetDestination(_patrol_current.transform.position);
    }
}
