using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Author: Corwin Belser
/// Provides helper methods for game-level information
public class GameInfo : MonoBehaviour {

	public enum GameState
	{
		Explore,
		Escape,
		Won
	}
	private GameState _gameState;

	public GameObject WIN; //Canvas image that shows you won

	// Use this for initialization
	void Start () {
		_gameState = GameState.Explore;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary> Changes the phase of the game </summary>
	/// <param name="state"> enum GameState: game phase to switch to </param>
	public void SetGameState(GameState state)
	{
		Debug.Log("Changing Game State to " + state + "!");
		_gameState = state;
		if (_gameState == GameState.Won)
			OnGameWon();
	}

	/// <summary> Helper method to get the current game phase </summary>
	///	<return> enum GameState: current phase the game is in </return>
	public GameState GetGameState()
	{
		return _gameState;
	}

	/// <summary>
	///		Helper method for handling the win state.  Currently displays
	///			a win image, but nothing else.
	/// </summary>
	public void OnGameWon()
	{
		WIN.SetActive(true);
	}
}
