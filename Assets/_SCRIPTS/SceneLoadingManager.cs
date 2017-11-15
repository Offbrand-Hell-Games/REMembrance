using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// Author: Joseph Lipinski
/// Used for loading new Unity Scenes


public class SceneLoadingManager : MonoBehaviour {

	/// <summary> Loads the scene that is passed into it </summary>
	/// <param name = SceneName> The name of the scene to be loaded </param>

	void LoadScene(int SceneNumber)
	{
		SceneManager.LoadScene(SceneNumber, LoadSceneMode.Single);
	}
}
