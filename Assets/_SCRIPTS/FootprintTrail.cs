using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Author: Corwin Belser
/// Attach to a GameObject to spawn a footprint trail on the ground
public class FootprintTrail : MonoBehaviour {

    public GameObject FOOTPRINT_TO_SPAWN; /* The footprint prefab to be spawned */
    public float SPAWN_FREQUENCY = 10.0f; /* How many seconds between each spawn */
    public int MAX_TO_SPAWN = 25; /* Maximum number of footprints at once */

    private GameObject[] _spawnList; /* Holds references to each spawned footprint. Used to destroy old footprints */
    private int _spawnIndex; /* The current index to use in _spawnList */
    private float _timeLastSpawned; /* Time.time of the last spawn */

    /// <summary>
    /// Called when the script first starts. Used for initialization
    /// </summary>
    void Start()
    {
        _timeLastSpawned = 0.0f;
        _spawnList = new GameObject[MAX_TO_SPAWN];
        _spawnIndex = 0;
    }

	/// <summary>
	/// Called every frame. Checks if enough time has passed, spawning a new footprint if so,
    /// and deleting old ones as necessary.
	/// </summary>
	void Update () {
        if (Time.time - _timeLastSpawned >= SPAWN_FREQUENCY && FOOTPRINT_TO_SPAWN != null)
        {    
            /* Check if the current spawn index needs to be destroyed */
            if (_spawnList[_spawnIndex] != null)
                GameObject.Destroy(_spawnList[_spawnIndex]);

            /* Spawn a new footprint at the current location */
            _spawnList[_spawnIndex] = GameObject.Instantiate(FOOTPRINT_TO_SPAWN);
            _spawnList[_spawnIndex].transform.position = this.transform.position;
            _spawnList[_spawnIndex].transform.rotation = this.transform.rotation;
            
            /* Increment the spawn index, looping as necessary */
            _spawnIndex = (_spawnIndex + 1) % MAX_TO_SPAWN;

            /* Set the time last spawned to the current time */
            _timeLastSpawned = Time.time;
        }

	}
}
