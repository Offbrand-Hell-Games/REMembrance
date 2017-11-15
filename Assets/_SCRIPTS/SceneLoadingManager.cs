using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// Author: Joseph Lipinski
/// Used for loading new Unity Scenes

public class SceneLoadingManager : MonoBehaviour 
{

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
			LoadScene(nextBuildIndex);
		}
	}
	
	/// <summary> Loads the scene that is passed into it </summary>
	/// <param name = SceneName> The name of the scene to be loaded </param>

	void LoadScene(int SceneNumber)
	{
		SceneManager.LoadScene(SceneNumber, LoadSceneMode.Single);
	}
}
