using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryScript : MonoBehaviour {

    public GameObject Player;
    bool _isheld = false;
    bool _up = true;
    float _baseHeight, step;
    Vector3 targetPosition, _maxHeight, _lowerHeight;

	// Use this for initialization
	void Start () {
        _baseHeight = transform.position.y;
        _maxHeight = new Vector3(transform.position.x, _baseHeight + 0.25f, transform.position.z);
        _lowerHeight = new Vector3(transform.position.x, _baseHeight - 0.25f, transform.position.z);
        step = 0.25f * Time.deltaTime;
    }
	
	// Update is called once per frame
	void Update () {
        if(_isheld == false)
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
            targetPosition = new Vector3(Player.transform.position.x - 1f, _baseHeight, Player.transform.position.z - 1f);
            transform.position = targetPosition;
        }
		
	}
    void Drop()
    {
        print("I SHOULD DROP THE BALL");
        _isheld = false;
        _maxHeight = new Vector3(transform.position.x, _baseHeight + 0.25f, transform.position.z);
        _lowerHeight = new Vector3(transform.position.x, _baseHeight - 0.25f, transform.position.z);
    }
    void OnCollisionEnter(Collision collision)
    {
        //print(collision.gameObject.name);
        if (collision.gameObject.name == "Player")
        {
            _isheld = true;
        }
    }
}
