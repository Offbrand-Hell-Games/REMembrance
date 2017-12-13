using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Memento : MonoBehaviour {
    
    PlayerController _player;
    
    public enum HeldBy //Enum in case we want to know what type of entity is holding this memento
    {
        None,
        Player,
        Enemy
    }
    private HeldBy _heldBy = HeldBy.None;
    [HideInInspector]
    public bool IN_NEST;

    private float _timeLeftBeforePulse = 25.0f;

    bool _up = true;
    float _baseHeight, step;
    Vector3 targetPosition, _maxHeight, _lowerHeight;

    private GameInfo _gameInfo;
    private MementoUtils _mementoUtils;
    private Transform _transformToFollow; // CB: The target transform to move this memento to
    private MeshRenderer _meshRenderer; // CB: The meshRenderer that renders through walls
	
	// Use this for initialization
	void Start () {
        _baseHeight = transform.position.y;
        _maxHeight = new Vector3(transform.position.x, _baseHeight + 0.25f, transform.position.z);
        _lowerHeight = new Vector3(transform.position.x, _baseHeight - 0.25f, transform.position.z);
        step = 0.25f * Time.deltaTime;

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _gameInfo = GameObject.Find("GameManager").GetComponent<GameInfo>();
        _mementoUtils = GameObject.Find("GameManager").GetComponent<MementoUtils>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if(_heldBy == HeldBy.None)
        {
            if (_up)
            {
                targetPosition = _maxHeight;
                if (targetPosition == transform.position)
                    _up = false; 
            }
            else
            {
                targetPosition = _lowerHeight;
                if (targetPosition == transform.position)
                    _up = true;
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        }
        else // CB: Bringing this back, as using SetParent was ignoring physics during a dash
        {
            this.transform.position = _transformToFollow.position;
        }

        /* Memento Pulse Code */
        if (_heldBy == HeldBy.Player && !IN_NEST)
        {
            _timeLeftBeforePulse -= Time.deltaTime;
            if (_timeLeftBeforePulse <= 0.0f)
            {
                _mementoUtils.OnMementoPulse(this.gameObject, false);
                _timeLeftBeforePulse = _mementoUtils.PULSE_TIME;
            }
            // Update the UI
            _mementoUtils.UpdateMementoTimer(_timeLeftBeforePulse);
        }

        /* Toggle the rendering through walls depending on visibility of the memento */
        CheckAndToggleVisibility();
	}

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("<color=pink>Memento collided with: " + collision.gameObject.name + "</color>");
        //If the player is the current owner, check if we're colliding with a wall
        if (_heldBy == HeldBy.Player)
        {
            if (collision.gameObject.tag == "Wall" && _player._isDashing){
                _player.OnMementoCollisionWithWall(this);
            }            
        }
    }
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Saferoom" || other.gameObject.name == "LevelLoader")
		{
			_gameInfo.SetGameState(GameInfo.GameState.Won);
            _player.OnWin(); // CB: Notify the playerController to play to win animation
		}
	}

    public void Bind(GameObject owner)
    {
        if (owner == null)
        {
            _heldBy = HeldBy.None;
        }
        else
        {
            switch (owner.tag)
            {
                case "Enemy":
                    _heldBy = HeldBy.Enemy;
                    //this.transform.SetParent(owner.transform, true); // CB: No longer setting the parent, as it was ignoring physics collisions when dashing
                    _transformToFollow = owner.transform.Find("CarryLocation");
                    this.transform.position = _transformToFollow.position;
                    break;
                case "Player":
                    _heldBy = HeldBy.Player;
                    _gameInfo.SetGameState(GameInfo.GameState.Escape);
                    //this.transform.SetParent(owner.transform, true); // CB: No longer setting the parent, as it was ignoring physics collisions when dashing
                    _transformToFollow = owner.transform.Find("CarryLocation");
                    this.transform.position = _transformToFollow.position;
                    break;
                default:
                    Debug.Log("<color=blue>Memento Error: Memento told to bind to something other than Enemy or Player!</color>");
                    _heldBy = HeldBy.None;
                    break;
            }
        }
    }

    public void Release()
    {
        _heldBy = HeldBy.None;
        //this.transform.SetParent(null); // CB: No longer setting the parent, as it was ignoring physics collisions when dashing
        _transformToFollow = null;
        _maxHeight = new Vector3(transform.position.x, _baseHeight + 0.25f, transform.position.z);
        _lowerHeight = new Vector3(transform.position.x, _baseHeight - 0.25f, transform.position.z);
    }

    public HeldBy GetHeldBy()
    {
        return _heldBy;
    }

    public void OnPlayerAbilityUsed()
    {
        _timeLeftBeforePulse += MementoUtils.PULSE_DELAY_AMOUNT;
        if (_timeLeftBeforePulse > _mementoUtils.PULSE_TIME)
            _timeLeftBeforePulse = _mementoUtils.PULSE_TIME;

        _mementoUtils.OnMementoPulse(this.gameObject, true);
        //Update UI
        _mementoUtils.UpdateMementoTimer(_timeLeftBeforePulse);
    }

    /// <summary>
    /// Checks if the memento can be seen by the current camera. If so,
    ///   turns off the meshRenderer component to stop rendering through walls.
    ///   Otherwise, turns the meshRenderer on.
    /// </summary>
    private void CheckAndToggleVisibility()
    {
        /* Check if the memento is in the camera's frustrum */
        Vector3 positionRelativeToViewport = Camera.main.WorldToViewportPoint(transform.position);

        if (positionRelativeToViewport.z > 0 && positionRelativeToViewport.x > 0 && positionRelativeToViewport.x < 1 && positionRelativeToViewport.y > 0 && positionRelativeToViewport.y < 1)
        {
            /* Check if the memento is currently obstructed from view */
            Vector3 direction = this.transform.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hitInfo;
            float maxDistance = Mathf.Abs(Vector3.Distance(transform.position, Camera.main.transform.position));

            Debug.DrawRay(Camera.main.transform.position, direction);

            RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, direction, maxDistance);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider != null)
                {
                    if (hit.collider.tag == "Wall" || hit.collider.tag == "Door")
                    {
                        _meshRenderer.enabled = true;
                        return;
                    }
                    if (hit.collider.tag == "Memento")
                    {
                        _meshRenderer.enabled = false;
                        return;
                    }
                }
            }
        }
        else /* The memento cannot be seen. Make sure it's rendering through walls */
        {
            _meshRenderer.enabled = true;
        }

    }

}
