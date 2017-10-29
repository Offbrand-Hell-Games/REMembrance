using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryScript : MonoBehaviour {
    
    PlayerController _player;
    Transform _target; //The current holder of this memento
    
    public enum HeldBy //Enum in case we want to know what type of entity is holding this memento
    {
        None,
        Player,
        Enemy,
        MementoPoint,
        Cooldown
    }
    private HeldBy _heldBy = HeldBy.None;

    bool _up = true;
    float _baseHeight, step;
    Vector3 targetPosition, _maxHeight, _lowerHeight;
    private MementoPoint _memento_point = null;

    private PhaseManager _phase_manager;
	
	public GameObject WIN; //Canvas image that shows you won

	// Use this for initialization
	void Start () {
        _baseHeight = transform.position.y;
        _maxHeight = new Vector3(transform.position.x, _baseHeight + 0.25f, transform.position.z);
        _lowerHeight = new Vector3(transform.position.x, _baseHeight - 0.25f, transform.position.z);
        step = 0.25f * Time.deltaTime;

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _phase_manager = GameObject.Find("GameManager").GetComponent<PhaseManager>();
    }
	
	public void SetHeldBy(HeldBy state)
	{
		_heldBy = state;
	}
	
	// Update is called once per frame
	void Update () {
        if(_heldBy == HeldBy.None || _heldBy == HeldBy.Cooldown || _heldBy == HeldBy.MementoPoint)
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
            targetPosition = new Vector3(_target.position.x, _baseHeight+.5f, _target.position.z);
            transform.position = targetPosition;
        }
		
	}
    void OnCollisionEnter(Collision collision)
    {
		//Temporary fuck it fix
		if(collision.gameObject.tag == "Player" && _heldBy != HeldBy.Player)
		{
			Debug.Log("Memento: Following Player");
			_phase_manager.SetGameState(PhaseManager.GameState.Escape);
			_heldBy = HeldBy.Player;
			_target = collision.gameObject.transform;
			if(_memento_point != null) {
				_memento_point.HAS_MEMENTO = false;
				_memento_point = null;
			}
		}
        //If we collide without a current owner, then check the collider
        if(_heldBy == HeldBy.None || _heldBy == HeldBy.MementoPoint)
        {
            string touchedBy = collision.gameObject.tag;
            /*
                Currently the player can only take the ball if they are not dashing.
                
                Changing this could allow for dashing through the ball to pick it up,
                  or even dashing through the enemy to steal the ball.
            */
            /* if(touchedBy == "Player" && !_player._isDashing)
            {
                Debug.Log("Memento: Following Player");
                _phase_manager.SetGameState(PhaseManager.GameState.Escape);
                _heldBy = HeldBy.Player;
                _target = collision.gameObject.transform;
                if(_memento_point != null) {
                    _memento_point.HAS_MEMENTO = false;
                    _memento_point = null;
                } 
            }*/ /* else */ if (touchedBy == "Enemy")
            {
                Debug.Log("Memento: Following Enemy");
                _heldBy = HeldBy.Enemy;
                _target = collision.gameObject.transform;
                if(_memento_point != null) {
                    _memento_point.HAS_MEMENTO = false;
                    _memento_point = null;
                }
            }
        }
        //If the player is the current owner, check if we're colliding with a wall
        else if (_heldBy == HeldBy.Player)
        {
			// oh no
            if (collision.gameObject.tag == "Wall" && _player._isDashing){
                StartCoroutine(Release(null));
            }            
        }
    }
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Saferoom")
		{
			WIN.SetActive(true);
		}
	}

    public IEnumerator Release(MementoPoint point)
    {
        _heldBy = HeldBy.Cooldown;
        _maxHeight = new Vector3(transform.position.x, _baseHeight + 0.25f, transform.position.z);
        _lowerHeight = new Vector3(transform.position.x, _baseHeight - 0.25f, transform.position.z);

        yield return new WaitForSeconds(10.0f);
        if (point != null) {
            _heldBy = HeldBy.MementoPoint;
            _memento_point = point;
        } else
            _heldBy = HeldBy.None;

        yield return null;
    }

    public HeldBy GetHeldBy()
    {
        return _heldBy;
    }
}
