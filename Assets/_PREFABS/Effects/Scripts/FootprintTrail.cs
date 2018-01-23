using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Author: Corwin Belser
/// Attach to a GameObject to spawn a footprint trail on the ground
public class FootprintTrail : MonoBehaviour {

    public GameObject FOOTPRINT_TO_SPAWN; /* The footprint prefab to be spawned */
    public int MAX_TO_SPAWN = 25; /* Maximum number of footprints at once */

    private GameObject[] _spawnList; /* Holds references to each spawned footprint. Used to destroy old footprints */
    private int _spawnIndex; /* The current index to use in _spawnList */
    private GameObject _parent; /* Empty parent gameObject the footprints are made children of. Keeps heirarchy clean */

    /// <summary>
    /// Called when the script first starts. Used for initialization
    /// </summary>
    void Start()
    {
        _spawnList = new GameObject[MAX_TO_SPAWN];
        _spawnIndex = 0;

        /* Create an empty gameObject to place the footprints in. This is to keep the hierarchy clean during runtime */
        _parent = new GameObject("footprintHolder");

        for (int i = 0; i < _spawnList.Length; i++)
        {
            _spawnList[i] = GameObject.Instantiate(FOOTPRINT_TO_SPAWN);
            _spawnList[i].transform.SetParent(_parent.transform, true);
            _spawnList[i].SetActive(false);
        }
    }

    /// <summary>
    ///     Called from animation event in REM@WalkingForward.
    ///     
    /// Spawns a footprint at the player's location
    /// </summary>
    /// <param name="foot">
    ///     1 - left foot
    ///     2 - right foot
    /// </param>
    public void Footprint(int foot)
    {
        /* Move the current footprint in the list to the player's position and rotation */
        _spawnList[_spawnIndex].transform.position = this.transform.position;
        _spawnList[_spawnIndex].transform.rotation = this.transform.rotation;

        /* Make the footprint active */
		_spawnList[_spawnIndex].SetActive(false);
        _spawnList[_spawnIndex].SetActive(true);

        /* Increment the index */
        _spawnIndex = (_spawnIndex + 1) % MAX_TO_SPAWN;
    }

}
