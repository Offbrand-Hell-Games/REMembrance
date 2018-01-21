using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

///	Author: Noah Nam
///	UIButtons contains code called by UI Buttons.
public class UIButtons : MonoBehaviour {
    public static GameObject hoveredButton;
    public Button options;      // used to reset highlight when closing options menu
    public Slider bgm;          // used to correctly select when opening options menu
    public GameObject highlight;    // only set this in inspector if you want the button to be highlighted

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

    /// <summary> Gets initial highlight for refocusing later. </summary>
    void Start(){
        hoveredButton = EventSystem.current.currentSelectedGameObject;
    }

    /// <summary> For the highlight on buttons, including refocus. </summary>
    void Update(){
        if (EventSystem.current.currentSelectedGameObject == null){
            EventSystem.current.SetSelectedGameObject(hoveredButton);
        }
        hoveredButton = EventSystem.current.currentSelectedGameObject;

        if (highlight && hoveredButton == gameObject){
				float step = 20f * Time.deltaTime;
				Vector3 target = new Vector3(highlight.transform.position.x,hoveredButton.transform.position.y,highlight.transform.position.z);
				highlight.transform.position = Vector3.Lerp(highlight.transform.position, target, step);
		}
	}
	
	/// <summary> Sets a menu button to be highlighted. </summary>
	/// <param name=b> The gameobject(menu button) to be highlighted </param>
	public void TriggerHovered(GameObject b){
		hoveredButton = b;
	}

    /// <summary> Sets a menu button to be highlighted. </summary>
    /// <param name=b> The menu button to be highlighted </param>
    public void MouseTriggerHovered(Button b){
        b.Select();
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
        // the following makes this method lose its generic-ness, find a better solution
        bgm.Select();
    }

    /// <summary> Closes a menu by setting its gameobject dormant. </summary>
    /// <param name=menu> The gameobject(menu button) to be closed. </param>
    public void CloseMenu(GameObject menu){
		menu.SetActive(false);
        // the following makes this method lose its generic-ness, find a better solution
        options.Select();
	}
	
	/// <summary> Hides the cursor. Used in conjuction with CloseMenu() when needed. </summary>
	public void CursorInvisible(){
		Cursor.visible = false;
	}
}
