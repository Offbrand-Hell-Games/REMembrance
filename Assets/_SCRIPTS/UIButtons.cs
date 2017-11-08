using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///	Author: Noah Nam
///	UIButtons contains code called by UI Buttons.
public class UIButtons : MonoBehaviour {
	
	/// <summary> Quits the application. </summary>
	public void QuitGame()
	{
		Debug.LogWarning ("Quitting Game From UI Button");
		#if UNITY_EDITOR
			// Application.Quit() does not work in the editor so
			// UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
	}
}
