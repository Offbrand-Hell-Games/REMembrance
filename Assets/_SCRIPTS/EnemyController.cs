using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    public enum EnemyState
    {
        Idle,
        Patrolling,
        TargetingPlayer,
        TargetingMemento,
        TransportingMemento
    }
    private EnemyState _enemyState;

    public float FOV_CONE_LENGTH = 10.0f; /* Maximum distance raycast should travel */
    public float FOV_CONE_RADIUS = 30.0f; /* Maximum angle between enemy's forward vector and the player */
    public float FOV_RADIUS = 3.0f; /* Radius around enemy to detect player */
    public float MIN_DISTANCE = 0.5f; /* Minimun distance before moving to next patrol point */
    public int PATROL_GROUP = 0; /* AI will only follow patrols in the assigned group */

    public PatrolPoint PATROL_START;
    public MementoPoint NEST;

    private PatrolPoint _patrolCurrent;
    private Memento _memento = null;
    
    private GameObject _player;
    private NavMeshAgent _navAgent;
    private PatrolManager _patrolManager;
    private MementoUtils _mementoUtils;
    private GameInfo _gameInfo;

    private float _lastTargetedPlayer = 0f;

    [HideInInspector]
    public Light visionLight;

	// Use this for initialization
	void Start () {
        _navAgent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _patrolManager = GameObject.Find("GameManager").GetComponent<PatrolManager>();
        _mementoUtils = GameObject.Find("GameManager").GetComponent<MementoUtils>();
        _gameInfo = GameObject.Find("GameManager").GetComponent<GameInfo>();

        if (PATROL_START != null) {
            /* Initialize patrol start point */
            _navAgent.SetDestination(PATROL_START.transform.position);
            _patrolCurrent = PATROL_START;
            _enemyState = EnemyState.Patrolling;
        } else {
            Debug.Log("<color=blue>AI Warning: This AI does not have a patrol set! (" + gameObject.name + ")</color>");
        }

        //Find and store a reference to the vision light
        Light[] lights = gameObject.GetComponentsInChildren<Light>();
		foreach (Light light in lights)
		{
			if (light.tag == "EnemyVisionLight")
				visionLight = light;
		}

	}
	
	// Update is called once per frame
	void Update ()
    {
        /*
            Check for state changes
                (Priority) <condition> -> <state to move to>
            All States EXCEPT Idle:
                (P1) See Player -> Targeting Player
            
            Idle:
                (P1) Time passed && See Player -> Targeting Player
                (P2) Time passed && Nearby Memento -> Targeting Memento
                (P3) Time passed -> Patrolling

            Patrolling:
                (P2) Pulse Nearby && Memento Point Empty -> Targeting Memento
                (P3) Memento Point Empty && Linked Memento Point full -> Targeting Memento
                (P3) Destination Reached -> Patrolling (next patrol point)
            
            Targeting Player:
                (P2) Destination Reached -> Patrolling
            
            Targeting Memento:
                (P2) Collision With Memento -> Transporting Memento
                (P3) Destination Reached -> Patrolling
                        
            Transporting Memento:
                (P2) Collision With Memento Point -> Patrolling
        */

        if (CanSeePlayer() && (_enemyState != EnemyState.Idle || Time.time - _lastTargetedPlayer <= PatrolManager.ENEMY_IDLE_TIME))
            SetTargetToPlayer();
        else
        {
            switch (_enemyState)
            {
                case EnemyState.Idle:
                    if (Time.time - _lastTargetedPlayer <= PatrolManager.ENEMY_IDLE_TIME)
                    {
                        if (!CheckMemento())
                            SetTargetToNearestPoint();
                    }
                    break;
                case EnemyState.Patrolling:
                    if (!CheckMemento() && !_navAgent.pathPending && _navAgent.remainingDistance < MIN_DISTANCE)
                        SetTargetToNextPatrolPoint();
                    break;
                case EnemyState.TransportingMemento:
                    //Drop-off is handled in the collision detection
                    break;
                default:
                    if (!_navAgent.pathPending && _navAgent.remainingDistance < MIN_DISTANCE)
                    {
                        SetTargetToNearestPoint();
                    }
                    break;
            }
        }
	}

    private bool CheckMemento()
    {
        if (NEST.MEMENTO == null)
        {
            //Check for any memento's out of MementoPoints
            GameObject memento = _mementoUtils.GetClosestMemento (transform.position);
            Memento mc = memento.GetComponent<Memento> ();
            if (mc != null && mc.GetHeldBy () == Memento.HeldBy.None && !mc.IN_NEST)
            {
                //Debug.Log("<color=blue>AI Debug: State Change: Patrolling -> TargetingMemento (nearby)</color>");
                _enemyState = EnemyState.TargetingMemento;
                _navAgent.SetDestination (memento.transform.position);
                return true;
            }
            else if (NEST.NEST_TO_TAKE_FROM != null && NEST.NEST_TO_TAKE_FROM.MEMENTO != null)
            {
                //Debug.Log("<color=blue>AI Debug: State Change: Patrolling -> TargetingMemento (other nest)</color>");
                _enemyState = EnemyState.TargetingMemento;
                _navAgent.SetDestination(NEST.NEST_TO_TAKE_FROM.MEMENTO.transform.position);
                return true;
            }
        }
        return false;
    }

    void OnTriggerEnter(Collider collider)
    {
        /* This code feels dirty.
            Memento's trigger collider is on the child object, so we need
              to access its parent through its transform
        */
        Transform parent = collider.gameObject.transform.parent;
        if (parent != null && parent.tag == "Memento" && NEST.MEMENTO == null
            && (_enemyState != EnemyState.TargetingPlayer
                && _enemyState != EnemyState.TransportingMemento))
        {
            _memento = parent.gameObject.GetComponent<Memento>();
            _memento.Bind(this.gameObject);
            _enemyState = EnemyState.TransportingMemento;
            _navAgent.SetDestination(NEST.transform.position);
        }
    }

    public bool CanSeePlayer()
    {
		RaycastHit hit;
		Physics.Raycast (transform.position, _player.transform.position - transform.position, out hit, FOV_CONE_LENGTH);
		if (hit.collider != null && hit.collider.gameObject != null && hit.collider.gameObject.CompareTag ("Player"))
        {
            if (Vector3.Distance(transform.position, _player.transform.position) <= FOV_RADIUS
                || Vector3.Angle(_player.transform.position - transform.position, transform.forward) <= FOV_CONE_RADIUS)
                return true;
        }
        return false;
    }

    public void SetTargetToPlayer()
    {
        //If we have the memento, drop it
        if (_memento != null)
        {
           //Debug.Log("<color=blue>AI: Dropping Memento</color>");
            _memento.Release();
            _memento = null;
        }
        //Debug.Log("<color=blue>AI: Setting Target to Player</color>");
        _enemyState = EnemyState.TargetingPlayer;
        _navAgent.SetDestination(_player.transform.position);
        _lastTargetedPlayer = Time.time;
    }

    public void SetTargetToNextPatrolPoint()
    {
        if (_patrolCurrent.NEXT != null)
        {
			_patrolCurrent = _patrolCurrent.NEXT; // Move to next point
            //Debug.Log("<color=blue>AI: Setting Target to Next Patrol Point " + _patrolCurrent.name + "</color>");
		    _navAgent.SetDestination (_patrolCurrent.transform.position);
        }
        else if (_patrolCurrent != PATROL_START) // Return to initial patrol point (unless we only have one)
		{
        	_patrolCurrent = PATROL_START; // Restart the patrol
            //Debug.Log("<color=blue>AI: Setting Target to Next Patrol Point " + _patrolCurrent.name + "</color>");
		    _navAgent.SetDestination (_patrolCurrent.transform.position);
        }
    }

    public void SetTargetToNearestPoint()
    {
        List<PatrolPoint> possible_points = _patrolManager.GetPatrolPointsByGroupID(PATROL_GROUP);
        if(possible_points.Count == 0) {
            Debug.Log("<color=blue>AI Warning:</color>Found no patrol points belonging to this AI's Patrol Group!"
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
        _patrolCurrent = closest;
        PATROL_START = _patrolCurrent;
        //Debug.Log("<color=blue>AI: Setting Target to Patrol Point " + _patrolCurrent.name + "</color>");
        _enemyState = EnemyState.Patrolling;
        _navAgent.SetDestination(_patrolCurrent.transform.position);
    }

    public void OnMementoPulse(GameObject memento)
    {
        if (_enemyState != EnemyState.TargetingPlayer && _enemyState != EnemyState.TransportingMemento)
        {
            _enemyState = EnemyState.TargetingMemento;
            _navAgent.SetDestination(memento.transform.position);
        }
    }
}
