using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolManager : MonoBehaviour {

	private Dictionary<int,List<PatrolPoint>> _patrol_groups;

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
	}
	
	// Update is called once per frame
	void Update () {

	}

	public List<PatrolPoint> GetPatrolPointsByGroupID(int groupID) {
		return (List<PatrolPoint>)_patrol_groups[groupID];
	}
}
