using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Author: Corwin Belser
/// Helper class for EnemyController to access patrol information
public class PatrolManager : MonoBehaviour {

	private Dictionary<int,List<PatrolPoint>> _patrol_groups;
	private GameObject[] _enemies;

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

        if (_enemies != null && _enemies.Length != 0)
        {
		    PrefsPaneManager.instance.AddLivePreferenceFloat("Enemy Speed", 0f, 20f, (_enemies[0].GetComponent<UnityEngine.AI.NavMeshAgent>()).speed, OnEnemySpeedChanged);
		    PrefsPaneManager.instance.AddLivePreferenceFloat("Enemy Pause Time (s)", 0f, 10f, (_enemies[0].GetComponent<EnemyController>()).ENEMY_IDLE_TIME, OnEnemyPauseTimeChanged);
		    PrefsPaneManager.instance.AddLivePreferenceFloat("Enemy Sight Length", 0f, 30, (_enemies[0].GetComponent<EnemyController>()).FOV_CONE_LENGTH, OnEnemySightLengthChanged);
		    PrefsPaneManager.instance.AddLivePreferenceFloat("Enemy Sight Radius", 0f, 180f, (_enemies[0].GetComponent<EnemyController>()).FOV_CONE_RADIUS, OnEnemySightRadiusChanged);
        }
	}

	/// <summary> Finds all enemies within a radius around the source position </summary>
	/// <param name="source"> Vector3: position to search around </param>
	/// <param name="radius"> float: max distance to check around the source </param>
	/// <return> EnemyController[]: Array of EnemyController references for each enemy in range </return>
	public EnemyController[] GetClosestEnemies(Vector3 source, float radius)
	{
		List<EnemyController> list = new List<EnemyController>();

		foreach (GameObject enemy in _enemies)
		{
			if (Vector3.Distance(source, enemy.transform.position) <= radius)
				list.Add(enemy.GetComponent<EnemyController>());
		}

		return list.ToArray();
	}

	/// <summary> Finds all patrol points belonging to a group </summary>
	/// <param name="groupID"> int: group number </param>
	/// <return> List<PatrolPoint>: List of every patrol point in the group </return>
	public List<PatrolPoint> GetPatrolPointsByGroupID(int groupID) {
		return (List<PatrolPoint>)_patrol_groups[groupID];
	}

	/// <summary> Preference Screen OnChange method for changing the movement speed of enemies </summary>
	/// <param name="value"> float: new value to use </param>
	public void OnEnemySpeedChanged(float value)
    {
		foreach (GameObject enemy in _enemies)
		{
			(enemy.GetComponent<UnityEngine.AI.NavMeshAgent>()).speed = value;
		}
    }

	/// <summary> Preference Screen OnChange method for changing the time enemies should pause for </summary>
	/// <param name="value"> float: new value to use </param>
	public void OnEnemyPauseTimeChanged(float value)
	{
		foreach (GameObject enemy in _enemies)
		{
			(enemy.GetComponent<EnemyController>()).ENEMY_IDLE_TIME = value;
		}
	}

	/// <summary> Preference Screen OnChange method for changing the sight distance of player detection </summary>
	/// <param name="value"> float: new value to use </param>
	public void OnEnemySightLengthChanged(float value)
	{
		foreach (GameObject enemy in _enemies)
		{
			(enemy.GetComponent<EnemyController>()).FOV_CONE_LENGTH = value;
			(enemy.GetComponent<EnemyController>()).visionLight.range = value;
		}
	}

	/// <summary> Preference Screen OnChange method for changing the radius of player detection </summary>
	/// <param name="value"> float: new value to use </param>
	public void OnEnemySightRadiusChanged(float value)
	{
		foreach (GameObject enemy in _enemies)
		{
			(enemy.GetComponent<EnemyController>()).FOV_CONE_RADIUS = value;
			(enemy.GetComponent<EnemyController>()).visionLight.spotAngle = value;
		}
	}
}
