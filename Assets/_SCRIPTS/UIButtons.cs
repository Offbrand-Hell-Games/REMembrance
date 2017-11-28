using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

///	Author: Noah Nam
///	UIButtons contains code called by UI Buttons.
public class UIButtons : MonoBehaviour {
	
	public bool affectsHighlight = false;
	private bool wasHovered;
	private GameObject hoveredButton;
	
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
	
	/// <summary> For the highlight on buttons. </summary>
	void Update(){
		if (wasHovered){
			if(affectsHighlight){
				float step = 20f * Time.deltaTime;
				Vector3 target = new Vector3(transform.position.x,hoveredButton.transform.position.y,transform.position.z);
				transform.position = Vector3.Lerp(transform.position, target, step);
			}
			
		}
	}
	
	/// <summary> Sets a menu button to be highlighted. </summary>
	/// <param name=b> The gameobject(menu button) to be highlighted </param>
	public void TriggerHovered(GameObject b){
		wasHovered = true;
		hoveredButton = b;
	}
	
	/// <summary> Starts the first level of the game. </summary>
	public void StartGame(){
		SceneManager.LoadScene(1, LoadSceneMode.Single);
	}
	
	/// <summary> Returns to main menu. </summary>
	public void MainMenu(){
		SceneManager.LoadScene(0, LoadSceneMode.Single);
	}
	
	/// <summary> Opens a menu by setting its gameobject active. </summary>
	/// <param name=menu> The gameobject(menu button) to be opened. </param>
	public void OpenMenu(GameObject menu){
		menu.SetActive(true);
	}
	
	/// <summary> Closes a menu by setting its gameobject dormant. </summary>
	/// <param name=menu> The gameobject(menu button) to be closed. </param>
	public void CloseMenu(GameObject menu){
		menu.SetActive(false);
	}
}
