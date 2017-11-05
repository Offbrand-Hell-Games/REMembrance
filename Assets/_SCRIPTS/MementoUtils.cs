using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MementoUtils : MonoBehaviour {

	public GameObject PULSE_EFFECT;
	public Slider MEMENTO_SLIDER;

	public static float PULSE_TIME = 25.0f;
    public static float PULSE_DELAY_AMOUNT = 5.0f;
    public static float MEMENTO_PULSE_RADIUS = 30.0f;
    public static float PLAYER_PULSE_RADIUS = 15.0f;

	private GameObject[] _mementos;
	private PatrolManager _patrolManager;

	// Use this for initialization
	void Start () {
		_mementos = GameObject.FindGameObjectsWithTag("Memento");
		_patrolManager = GameObject.Find("GameManager").GetComponent<PatrolManager>();

		PrefsPaneManager.instance.AddLivePreferenceFloat("Pulse Time", 0f, 120f, MementoUtils.PULSE_TIME, OnPulseTimeChanged);
		PrefsPaneManager.instance.AddLivePreferenceFloat("Pulse Refill", 0f, 20f, MementoUtils.PULSE_DELAY_AMOUNT, OnPulseDelayChanged);
		PrefsPaneManager.instance.AddLivePreferenceFloat("Pulse Big Radius", 0f, 50f, MementoUtils.MEMENTO_PULSE_RADIUS, OnMementoPulseRadiusChanged);
		PrefsPaneManager.instance.AddLivePreferenceFloat("Pulse Small Radius", 0f, 50f, MementoUtils.PLAYER_PULSE_RADIUS, OnPlayerPulseRadiusChanged);

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

	public void OnMementoPulse(GameObject memento, bool isPlayerPulse)
	{
		float radius = isPlayerPulse ? PLAYER_PULSE_RADIUS : MEMENTO_PULSE_RADIUS;

		if (PULSE_EFFECT != null)
		{
			GameObject pulse = GameObject.Instantiate(PULSE_EFFECT);
			pulse.transform.position = memento.transform.position;


			float time = radius / (pulse.GetComponent<ParticleSystem>()).startSpeed;
			Debug.Log("<color=blue>Memento Pulse: Destroying pulse game object after " + time + "seconds</color>");
			Object.Destroy(pulse, time);
		}
		else
		{
			Debug.Log("<color=blue>Memento Warning: Memento pulse lacks the pulse prefab!");
		}
		EnemyController[] enemies = _patrolManager.GetClosestEnemies(memento.transform.position, radius);
		foreach (EnemyController enemy in enemies)
			enemy.OnMementoPulse(memento);
	}

	public void UpdateMementoTimer(float timeLeft, float totalTime)
	{
		MEMENTO_SLIDER.value = timeLeft / totalTime;
	}

	public void OnPulseTimeChanged(float value)
	{
		MementoUtils.PULSE_TIME = value;
	}

	public void OnPulseDelayChanged(float value)
	{
		MementoUtils.PULSE_DELAY_AMOUNT = value;
	}

	public void OnMementoPulseRadiusChanged(float value)
	{
		MementoUtils.MEMENTO_PULSE_RADIUS = value;
	}

	public void OnPlayerPulseRadiusChanged(float value)
	{
		MementoUtils.PLAYER_PULSE_RADIUS = value;
	}

}
