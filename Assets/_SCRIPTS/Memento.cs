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
	
	// Use this for initialization
	void Start () {
        _baseHeight = transform.position.y;
        _maxHeight = new Vector3(transform.position.x, _baseHeight + 0.25f, transform.position.z);
        _lowerHeight = new Vector3(transform.position.x, _baseHeight - 0.25f, transform.position.z);
        step = 0.25f * Time.deltaTime;

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _gameInfo = GameObject.Find("GameManager").GetComponent<GameInfo>();
        _mementoUtils = GameObject.Find("GameManager").GetComponent<MementoUtils>();
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
        /* CB: This is no longer needed, as we make this game object a child of
         *  the gameobject claiming it.
        else
        {
            targetPosition = new Vector3(_transformToFollow.position.x, _baseHeight+.5f, _transformToFollow.position.z);
            transform.position = targetPosition;
        }*/

        /* Memento Pulse Code */
        if (_heldBy == HeldBy.Player && !IN_NEST)
        {
            _timeLeftBeforePulse -= Time.deltaTime;
            if (_timeLeftBeforePulse <= 0.0f)
            {
                _mementoUtils.OnMementoPulse(this.gameObject, false);
                _timeLeftBeforePulse = MementoUtils.PULSE_TIME;
            }
            // Update the UI
            _mementoUtils.UpdateMementoTimer(_timeLeftBeforePulse, MementoUtils.PULSE_TIME);
        }
		
	}
    void OnCollisionEnter(Collision collision)
    {
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
                    this.transform.SetParent(owner.transform, true);
                    this.transform.localPosition = new Vector3(0f, 0.5f, 0.6f);
                    break;
                case "Player":
                    _heldBy = HeldBy.Player;
                    _gameInfo.SetGameState(GameInfo.GameState.Escape);
                    this.transform.SetParent(owner.transform, true);
                    this.transform.localPosition = new Vector3(0f, 0.6f, 0.4f);
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
        this.transform.SetParent(null);
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
        if (_timeLeftBeforePulse > MementoUtils.PULSE_TIME)
            _timeLeftBeforePulse = MementoUtils.PULSE_TIME;

        _mementoUtils.OnMementoPulse(this.gameObject, true);
        //Update UI
        _mementoUtils.UpdateMementoTimer(_timeLeftBeforePulse, MementoUtils.PULSE_TIME);
    }

}
