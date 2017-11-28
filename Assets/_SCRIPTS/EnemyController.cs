using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// Author: Corwin Belser
/// Controller to patrol, chase players, and horde mementos
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour {

    public enum EnemyState
    {
        Idle,
        Patrolling,
        TargetingPlayer,
        TargetingMemento,
        TransportingMemento
    }
    private EnemyState _enemyState; /* The current state enemy is in */
    private EnemyState _previousState; /* Previous state enemy was in. Used to resume a patrol after idling */

    /****************************** Player Detection ******************************/
    public float FOV_CONE_LENGTH = 10.0f; /* Maximum distance raycast should travel */
    public float FOV_CONE_RADIUS = 30.0f; /* Maximum angle between enemy's forward vector and the player */
    public float FOV_RADIUS = 3.0f; /* Radius around enemy to detect player */
	public Color TARGETING_PLAYER_COLOR; /* Color of the vision light when the enemy is targeting the player */
	private Color _neutralColor; /* Color of the vision light by default */
	public AudioClip TARGETING_PLAYER_AUDIO; /* Audio clip to play when targeting the player */
	private AudioClip _neutralAudio; /* Default audio clip */
    private GameObject _player; /* Reference to the player gameObject */
    private float _timeOnTargetPlayer; /* Time in seconds since game start that TargetingPlayer state was last entered */

    /******************************* Patrol Settings ******************************/
    public float MIN_DISTANCE = 0.5f; /* Minimun distance before moving to next patrol point */
    public int PATROL_GROUP = 0; /* AI will only follow patrols in the assigned group */
    public PatrolPoint PATROL_START; /* Starting Patrol Point */
    private PatrolPoint _patrolCurrent; /* The current patrol point */

    /******************************* Memento Settings ******************************/
    public float SPEED_DEFAULT = 5f;
    public float SPEED_TARGETING_PLAYER = 8f;
    public Nest NEST; /* Nest to take found mementos to */
    private Memento _memento = null; /* Reference to the memento being held, if one exists */

    /***************************** Movement Settings ******************************/
    public float ENEMY_IDLE_TIME = 2.0f; /* How long enemies should pause after losing track of the player or memento, or reaching a patrol point */
    public float MEMENTO_SEARCH_RADIUS = 5.0f; /* Radius around enemy to detect dropped mementos */
    public float DELAY_BEFORE_TAKING_FROM_LINKED_NEST = 15.0f; /* Time to wait after a memento enters a linked nest before trying to steal it */
    private float _timeOnEnterIdle; /* Time in seconds since game start that Idle state was last entered */

    /*************************** GameObject Components ****************************/
    private NavMeshAgent _navAgent;
    private PatrolManager _patrolManager;
    private MementoUtils _mementoUtils;
    private GameInfo _gameInfo;
	private Animator _animator;
    [HideInInspector]
    public Light visionLight;
    private AudioSource _audioSource;


	/// <summary> Initializes required components </summary>
	void Start () {
        _navAgent = GetComponent<NavMeshAgent>();
		_audioSource = GetComponent<AudioSource>();
		_animator = GetComponentInChildren<Animator>();
		_neutralAudio = _audioSource.clip;
        _player = GameObject.FindGameObjectWithTag("Player");
        _patrolManager = GameObject.Find("GameManager").GetComponent<PatrolManager>();
        _mementoUtils = GameObject.Find("GameManager").GetComponent<MementoUtils>();
        _gameInfo = GameObject.Find("GameManager").GetComponent<GameInfo>();

        _timeOnEnterIdle = Time.time;
        _timeOnTargetPlayer = Time.time;

        if (PATROL_START != null) {
            /* Initialize patrol start point */
            _navAgent.SetDestination(PATROL_START.transform.position);
            _patrolCurrent = PATROL_START;
            ChangeState(EnemyState.Patrolling);
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
        if (visionLight != null)
        {
            visionLight.range = FOV_CONE_LENGTH;
            visionLight.spotAngle = FOV_CONE_RADIUS;
			_neutralColor = visionLight.color;
        }

	}
	
	/// <summary> Update is called once per frame </summary>
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

        if (CanSeePlayer() && (_enemyState != EnemyState.Idle || Time.time - _timeOnTargetPlayer >= ENEMY_IDLE_TIME))
            SetTargetToPlayer();
        else
        {
            switch (_enemyState)
            {
                case EnemyState.Idle:
                    if (Time.time - _timeOnEnterIdle >= ENEMY_IDLE_TIME)
                    {
                        if (!CheckMemento())
                        {
                            if (_previousState == EnemyState.Patrolling)
                                SetTargetToNextPatrolPoint();
                            else
                                SetTargetToNearestPoint();
                        }
                    }
                    break;
                case EnemyState.Patrolling:
                    if (!CheckMemento() && !_navAgent.pathPending && _navAgent.remainingDistance < MIN_DISTANCE)
                        ChangeState(EnemyState.Idle);
                    break;
                case EnemyState.TransportingMemento:
                    if (_navAgent.remainingDistance < MIN_DISTANCE)
                    {
                        _memento.Release();
                        _memento = null;
                        SetTargetToNearestPoint();
                    }
                    break;
                case EnemyState.TargetingPlayer:
                    if (!_navAgent.pathPending && _navAgent.remainingDistance < MIN_DISTANCE)
                    {
                        ChangeState(EnemyState.Idle);
						//Debug.Log("<color=blue>AI: Lost player. Entering Idle</color>");
                    }
                    break;
                case EnemyState.TargetingMemento:
                    if (!_navAgent.pathPending && _navAgent.remainingDistance < MIN_DISTANCE)
                    {
                        ChangeState(EnemyState.Idle);

                    }
                    break;
                default:
                    if (!_navAgent.pathPending && _navAgent.remainingDistance < MIN_DISTANCE)
                    {
                        //SetTargetToNearestPoint();
                        ChangeState(EnemyState.Idle); // CB: Enter idle for a bit after reaching a destination
                    }
                    break;
            }
        }
	}

    /// <summary>
    ///     Handles state changes related to mementos
    /// 
    ///     If this enemy's nest is empty, it checks nearby for any abandoned mementos.
    ///         If a memento is found, it changes the enemy state to TrackingMemento and
    ///         sets the navAgent's destination
    ///     If this enemy's nest is empty, and it's linked to another nest that does
    ///         have a memento, it changes the enemy state to TargetingMemento and sets
    ///         the navAgent's destination
    /// </summary>
    /// <return> true if the enemy state was changed, false otherwise </return>
    /// <thoughts>
    ///     This method is called Check, but it does more than that, and it shouldn't.
    ///     TODO: Break the contents of this function into two pieces:
    ///         AttemptStateChange()
    ///         ProcessStateChange()
    /// </thoughts>
    private bool CheckMemento()
    {
        if (NEST != null && NEST.MEMENTO == null)
        {
            //Check for any memento's out of Nests
            GameObject memento = _mementoUtils.GetClosestMemento (transform.position);
            Memento mc = memento.GetComponent<Memento> ();
            if (mc != null && Vector3.Distance(mc.transform.position, transform.position) <= MEMENTO_SEARCH_RADIUS && mc.GetHeldBy () == Memento.HeldBy.None && !mc.IN_NEST)
            {
                //Debug.Log("<color=blue>AI Debug: State Change: Patrolling -> TargetingMemento (nearby)</color>");
                ChangeState(EnemyState.TargetingMemento);
                _navAgent.SetDestination (memento.transform.position);
                return true;
            }
            else if (NEST != null && NEST.NEST_TO_TAKE_FROM != null && NEST.NEST_TO_TAKE_FROM.MEMENTO != null && Time.time - NEST.TIME_MEMENTO_ENTERED >= DELAY_BEFORE_TAKING_FROM_LINKED_NEST)
            {
                //Debug.Log("<color=blue>AI Debug: State Change: Patrolling -> TargetingMemento (other nest)</color>");
                ChangeState(EnemyState.TargetingMemento);
                _navAgent.SetDestination(NEST.NEST_TO_TAKE_FROM.MEMENTO.transform.position);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    ///     Handles the collision check with the memento, picking it up and
    ///         changing the enemy state to TransportingMemento
    /// </summary>
    /// <param name="collider"> Collider: collider that is now colliding with this enemy</param>
    void OnTriggerEnter(Collider collider)
    {
        /* This code feels dirty.
            Memento's trigger collider is on the child object, so we need
              to access its parent through its transform
        */
        Transform parent = collider.gameObject.transform.parent;
        if (parent != null && parent.tag == "Memento" && NEST != null && NEST.MEMENTO == null
            && (_enemyState != EnemyState.TargetingPlayer
                && _enemyState != EnemyState.TransportingMemento))
        {
            _memento = parent.gameObject.GetComponent<Memento>();
            _memento.Bind(this.gameObject);
            ChangeState(EnemyState.TransportingMemento);
            _navAgent.SetDestination(NEST.transform.position);
        }
    }

    /// <summary> Helper function for checking player detection </summary>
    /// <return> true if the player is within visible range, false otherwise </return>
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

    /// <summary>
    ///     Helper function to target the player.
    ///         Drops the memento if it is held.
    ///         Changes enemy state to TargetingPlayer.
    ///         Sets the navAgent's destination.
    /// </summary>
    public void SetTargetToPlayer()
    {
        //If we have the memento, drop it
        if (_memento != null)
        {
           //Debug.Log("<color=blue>AI: Dropping Memento</color>");
            _memento.Release();
            _memento = null;
        }
		// Only set the audio if we are entering this state for the first time
        if (_enemyState != EnemyState.TargetingPlayer)
            ChangeState(EnemyState.TargetingPlayer);

        _navAgent.SetDestination(_player.transform.position);
        _timeOnTargetPlayer = Time.time;
    }

    /// <summary>
    ///     Helper function to change the navAgent's destination to the next
    ///         patrol point.
    ///     If there is no next patrol point, go back to the initial point
    ///     Otherwise, move to the next point
    ///</summary>
    public void SetTargetToNextPatrolPoint()
    {
        if (_patrolCurrent.NEXT != null)
        {
			_patrolCurrent = _patrolCurrent.NEXT; // Move to next point
            ChangeState(EnemyState.Patrolling);
            //Debug.Log("<color=blue>AI: Setting Target to Next Patrol Point " + _patrolCurrent.name + "</color>");
		    _navAgent.SetDestination (_patrolCurrent.transform.position);
        }
        else if (_patrolCurrent != PATROL_START) // Return to initial patrol point (unless we only have one)
		{
        	_patrolCurrent = PATROL_START; // Restart the patrol
            ChangeState(EnemyState.Patrolling);
            //Debug.Log("<color=blue>AI: Setting Target to Next Patrol Point " + _patrolCurrent.name + "</color>");
		    _navAgent.SetDestination (_patrolCurrent.transform.position);
        }
    }

    /// <summary>
    ///     Helper function to change the navAgent's destination to the
    ///         closest patrol point in the patrol group.
    ///     Utilizes the PatrolManager to find all patrol points in the
    ///         group.
    ///     ***If no points are returned, the function exits.***
    ///     Determines the closest point, setting the navAgent's
    ///         destination
    ///     Changes enemy state to Patrolling
    /// </summary>
    public void SetTargetToNearestPoint()
    {
        List<PatrolPoint> possible_points = _patrolManager.GetPatrolPointsByGroupID(PATROL_GROUP);
        if(possible_points.Count == 0) {
            //Debug.Log("<color=blue>AI Warning:</color>Found no patrol points belonging to this AI's Patrol Group!"
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
        _patrolCurrent = closest;
        PATROL_START = _patrolCurrent;
        //Debug.Log("<color=blue>AI: Setting Target to Patrol Point " + _patrolCurrent.name + "</color>");
        ChangeState(EnemyState.Patrolling);
        _navAgent.SetDestination(_patrolCurrent.transform.position);
    }

    /// <summary>
    ///     Called from Memento class to tell this enemy a memento has pulsed nearby.
    ///
    ///     If this enemy's state is not TargetingPlayer or TransportingMemento, the
    ///         state is changed to TargetingMemento and the navAgent's destination
    ///         is set to the location of the memento
    /// </summary>
    /// <param name="memento"> GameObject: game object of the memento that pulsed</param>
    public void OnMementoPulse(GameObject memento)
    {
        if (_enemyState != EnemyState.TargetingPlayer && _enemyState != EnemyState.TransportingMemento)
        {
            ChangeState(EnemyState.TargetingMemento);
            _navAgent.SetDestination(memento.transform.position);
        }
    }

    /// <summary>
    ///     Helper method to handle state change events like audio and animation
    /// </summary>
    /// <param name="newState">The new state to transition to</param>
    public void ChangeState(EnemyState newState)
    {
        _previousState = _enemyState;
        switch (newState)
        {
            case EnemyState.Idle:
                _enemyState = EnemyState.Idle;
                _timeOnEnterIdle = Time.time;
				visionLight.color = _neutralColor; // Return the vision light to its original color
				_audioSource.clip = _neutralAudio;
				_audioSource.Play();
				_animator.SetBool("isTargetingPlayer", false);
                _animator.SetBool("isIdle", true);
                _navAgent.speed = SPEED_DEFAULT; // CB: Change the movement speed to match the state
                break;
            case EnemyState.Patrolling:
                _enemyState = EnemyState.Patrolling;
                _animator.SetBool("isIdle", false);
                _navAgent.speed = SPEED_DEFAULT; // CB: Change the movement speed to match the state
                break;
            case EnemyState.TargetingMemento:
                _enemyState = EnemyState.TargetingMemento;
                _animator.SetBool("isIdle", false);
                _navAgent.speed = SPEED_DEFAULT; // CB: Change the movement speed to match the state
                break;
            case EnemyState.TargetingPlayer:
			    _enemyState = EnemyState.TargetingPlayer;
			    visionLight.color = TARGETING_PLAYER_COLOR; // Change the color of the vision light
			    _audioSource.clip = TARGETING_PLAYER_AUDIO;
			    _audioSource.Play();
			    _animator.SetBool("isTargetingPlayer", true);
                _animator.SetBool("isIdle", false);
                _navAgent.speed = SPEED_TARGETING_PLAYER; // CB: Change the movement speed to match the state
                break;
            case EnemyState.TransportingMemento:
                _enemyState = EnemyState.TransportingMemento;
                _animator.SetBool("isIdle", false);
                _navAgent.speed = SPEED_DEFAULT; // CB: Change the movement speed to match the state
                break;
        }
    }
}
