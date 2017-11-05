using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MementoUtils : MonoBehaviour {

	public GameObject PULSE_EFFECT;
	public Slider MEMENTO_SLIDER;

	private GameObject[] _mementos;
	private PatrolManager _patrolManager;

	// Use this for initialization
	void Start () {
		_mementos = GameObject.FindGameObjectsWithTag("Memento");
		_patrolManager = GameObject.Find("GameManager").GetComponent<PatrolManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public GameObject GetClosestMemento(Vector3 pos)
	{
		GameObject closest = _mementos[0];
		float min_distance = Vector3.Distance(pos, closest.transform.position);
        foreach (GameObject m in _mementos) {
            float d = Vector3.Distance(pos, m.transform.position);
            if(d < min_distance) {
                closest = m;
                min_distance = d;
            }
        }
		return closest;
	}

	public void OnMementoPulse(GameObject memento, float radius)
	{
		if (PULSE_EFFECT != null)
		{
			GameObject pulse = GameObject.Instantiate(PULSE_EFFECT);
			pulse.transform.position = memento.transform.position;
			Object.Destroy(pulse, radius); // TODO: radius should be changed to a time
		}

		EnemyController[] enemies = _patrolManager.GetClosestEnemies(memento.transform.position, radius);
		foreach (EnemyController enemy in enemies)
			enemy.OnMementoPulse(memento);
	}

	public void UpdateMementoTimer(float timeLeft, float totalTime)
	{
		MEMENTO_SLIDER.value = timeLeft / totalTime;
	}
}
