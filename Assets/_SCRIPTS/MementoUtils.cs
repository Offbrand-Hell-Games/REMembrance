using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// Author: Corwin Belser
///	Provides helper methods for finding mementos, as well as handling
///		the visual display for the memento pulse timer.
public class MementoUtils : MonoBehaviour {

	public GameObject PULSE_EFFECT;
	public Slider MEMENTO_SLIDER;

	public static float PULSE_TIME = 15.0f;
    public static float PULSE_DELAY_AMOUNT = 1.0f;
    public static float MEMENTO_PULSE_RADIUS = 25.0f;
    public static float PLAYER_PULSE_RADIUS = 7.5f;
    public float PULSE_SPEED = 20f;

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

	/// <summary>
	///		Given a position vector, returns the closest memento (if any)
	/// </summary>
	/// <param name="pos"> Vector3: location to check distance from </param>
	/// <return> GameObject: game object of the closest memento </return>
	public GameObject GetClosestMemento(Vector3 pos)
	{
		if (_mementos == null)
			return null;
		
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

	/// <summary>
	///		Helper method for processing a memento pulse event. Notifies enemies
	///			near the pulse, and instantiates the pulse prefab
	/// </summary>
	/// <param name="memento"> GameObject: the memento causing the pulse </param>
	/// <param name="isPlayerPulse"> bool: is the pulse caused by the player power? false if caused by memento timer </param>
	public void OnMementoPulse(GameObject memento, bool isPlayerPulse)
	{
		float radius = isPlayerPulse ? PLAYER_PULSE_RADIUS : MEMENTO_PULSE_RADIUS;

		if (PULSE_EFFECT != null)
		{
			GameObject pulse = GameObject.Instantiate(PULSE_EFFECT);
			pulse.transform.position = memento.transform.position;

            /* CB: pulse now has its own script. We can set the max size and speed there */
            ScaleThenDestroy script = pulse.GetComponent<ScaleThenDestroy>();
            script.MAX_DISTANCE = radius;
            script.EXPANSION_SPEED = PULSE_SPEED;
		}
		else
		{
			Debug.Log("<color=blue>Memento Warning: Memento pulse lacks the pulse prefab!");
		}
		EnemyController[] enemies = _patrolManager.GetClosestEnemies(memento.transform.position, radius);
		foreach (EnemyController enemy in enemies)
			enemy.OnMementoPulse(memento);
	}

	/// <summary>
	///		Helper method for updating the UI pulse timer
	/// </summary>
	/// <param name="timeLeft"> float: the time remaining before a pulse </param>
	/// <param name="totalTime"> float: the maximum value of the pulse timer </param>
	public void UpdateMementoTimer(float timeLeft, float totalTime)
	{
		MEMENTO_SLIDER.value = timeLeft / totalTime;
	}

	/// <summary> Parameter Screen OnChange method for changing time before pulse</summary>
	/// <param name="value"> float: the new value </param>
	public void OnPulseTimeChanged(float value)
	{
		MementoUtils.PULSE_TIME = value;
	}

	/// <summary> Parameter Screen OnChange method for changing the time refunded when the player uses an ability</summary>
	/// <param name="value"> float: the new value </param>
	public void OnPulseDelayChanged(float value)
	{
		MementoUtils.PULSE_DELAY_AMOUNT = value;
	}

	/// <summary> Parameter Screen OnChange method for changing the radius of the pulse caused by the timer</summary>
	/// <param name="value"> float: the new value </param>
	public void OnMementoPulseRadiusChanged(float value)
	{
		MementoUtils.MEMENTO_PULSE_RADIUS = value;
	}

	/// <summary> Parameter Screen OnChange method for changing the radius of the pulse caused by the player using an ability</summary>
	/// <param name="value"> float: the new value </param>
	public void OnPlayerPulseRadiusChanged(float value)
	{
		MementoUtils.PLAYER_PULSE_RADIUS = value;
	}

}
