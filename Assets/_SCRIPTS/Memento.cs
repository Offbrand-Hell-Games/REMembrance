using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memento : MonoBehaviour {
    
    PlayerController _player;
    Transform _transformToFollow; //The current holder of this memento
    
    public enum HeldBy //Enum in case we want to know what type of entity is holding this memento
    {
        None,
        Player,
        Enemy
    }
    private HeldBy _heldBy = HeldBy.None;
    [HideInInspector]
    public bool IN_NEST;

    bool _up = true;
    float _baseHeight, step;
    Vector3 targetPosition, _maxHeight, _lowerHeight;

    private GameInfo _gameInfo;
	
	// Use this for initialization
	void Start () {
        _baseHeight = transform.position.y;
        _maxHeight = new Vector3(transform.position.x, _baseHeight + 0.25f, transform.position.z);
        _lowerHeight = new Vector3(transform.position.x, _baseHeight - 0.25f, transform.position.z);
        step = 0.25f * Time.deltaTime;

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _gameInfo = GameObject.Find("GameManager").GetComponent<GameInfo>();
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
        else
        {
            targetPosition = new Vector3(_transformToFollow.position.x, _baseHeight+.5f, _transformToFollow.position.z);
            transform.position = targetPosition;
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
		if(other.gameObject.tag == "Saferoom")
		{
			_gameInfo.SetGameState(GameInfo.GameState.Won);
		}
	}

    public void Bind(GameObject owner)
    {
        if (owner == null)
        {
            _heldBy = HeldBy.None;
            _transformToFollow = null;
        }
        else
        {
            switch (owner.tag)
            {
                case "Enemy":
                    _heldBy = HeldBy.Enemy;
                    _transformToFollow = owner.transform;
                    break;
                case "Player":
                    _heldBy = HeldBy.Player;
                    _transformToFollow = owner.transform;
                    _gameInfo.SetGameState(GameInfo.GameState.Escape);
                    break;
                default:
                    Debug.Log("<color=blue>Memento Error: Memento told to bind to something other than Enemy or Player!</color>");
                    _heldBy = HeldBy.None;
                    _transformToFollow = null;
                    break;
            }
        }
    }

    public void Release()
    {
        _heldBy = HeldBy.None;
        _transformToFollow = null;
        _maxHeight = new Vector3(transform.position.x, _baseHeight + 0.25f, transform.position.z);
        _lowerHeight = new Vector3(transform.position.x, _baseHeight - 0.25f, transform.position.z);
    }

    public HeldBy GetHeldBy()
    {
        return _heldBy;
    }

}
