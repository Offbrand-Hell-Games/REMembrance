using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolManager : MonoBehaviour {

	private Dictionary<int,List<PatrolPoint>> _patrol_groups;
	private GameObject[] _enemies;
	[SerializeField]
	public static float ENEMY_IDLE_TIME = 5.0f;

	// Use this for initialization
	void Start () {
		_patrol_groups = new Dictionary<int,List<PatrolPoint>>();
		PatrolPoint[] points = FindObjectsOfType(typeof(PatrolPoint)) as PatrolPoint[];

		foreach (PatrolPoint p in points)
		{
			if (!_patrol_groups.ContainsKey(p.PATROL_GROUP)) {
				_patrol_groups.Add(p.PATROL_GROUP, new List<PatrolPoint>());
			}

			((List<PatrolPoint>)_patrol_groups[p.PATROL_GROUP]).Add(p);
		}

		_enemies = GameObject.FindGameObjectsWithTag("Enemy");

		PrefsPaneManager.instance.AddLivePreferenceFloat("Enemy Speed", 0f, 20f, (_enemies[0].GetComponent<UnityEngine.AI.NavMeshAgent>()).speed, enemySpeedChanged);
		PrefsPaneManager.instance.AddLivePreferenceFloat("Enemy Pause Time (s)", 0f, 10f, ENEMY_IDLE_TIME, PauseTimeChanged);
		PrefsPaneManager.instance.AddLivePreferenceFloat("Enemy Sight Length", 0f, 30, (_enemies[0].GetComponent<EnemyController>()).FOV_CONE_LENGTH, EnemySightLengthChanged);
		PrefsPaneManager.instance.AddLivePreferenceFloat("Enemy Sight Radius", 0f, 180f, (_enemies[0].GetComponent<EnemyController>()).FOV_CONE_RADIUS, EnemySightRadiusChanged);
	}
	
	// Update is called once per frame
	void Update () {

	}

	public List<PatrolPoint> GetPatrolPointsByGroupID(int groupID) {
		return (List<PatrolPoint>)_patrol_groups[groupID];
	}

	public void enemySpeedChanged(float value)
    {
		foreach (GameObject enemy in _enemies)
		{
			(enemy.GetComponent<UnityEngine.AI.NavMeshAgent>()).speed = value;
		}
    }

	public void PauseTimeChanged(float value)
	{
		ENEMY_IDLE_TIME = value;
	}

	public void EnemySightLengthChanged(float value)
	{
		foreach (GameObject enemy in _enemies)
		{
			(enemy.GetComponent<EnemyController>()).FOV_CONE_LENGTH = value;
			(enemy.GetComponent<EnemyController>()).visionLight.range = value;
		}
	}

	public void EnemySightRadiusChanged(float value)
	{
		foreach (GameObject enemy in _enemies)
		{
			(enemy.GetComponent<EnemyController>()).FOV_CONE_RADIUS = value;
			(enemy.GetComponent<EnemyController>()).visionLight.spotAngle = value;
		}
	}
}
