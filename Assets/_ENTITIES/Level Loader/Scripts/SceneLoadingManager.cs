using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// Author: Joseph Lipinski
/// Used for loading new Unity Scenes

public class SceneLoadingManager : MonoBehaviour 
{

    public float LEVEL_LOAD_DELAY = 5f;
	int BuildIndex, nextBuildIndex;
	
	// Use this for initialization
	void Start () 
	{
		BuildIndex = SceneManager.GetActiveScene().buildIndex;
		nextBuildIndex = BuildIndex + 1;
	}

	/// The prefab has a collider. When the memento touches it the next level in the build index
	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.tag == "Memento")
		{
            StartCoroutine("LoadScene");
		}
	}
	
	/// <summary> Loads the scene that is passed into it </summary>
	/// <param name = SceneName> The name of the scene to be loaded </param>

	IEnumerator LoadScene()
	{
        yield return new WaitForSeconds(LEVEL_LOAD_DELAY); // CB: Added a delay to allow player victory animation to play
        SceneManager.LoadScene(nextBuildIndex, LoadSceneMode.Single);
	}
}
