using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour {

	public enum GameState
	{
		Explore,
		Escape
	}

	private GameState _game_state;

	// Use this for initialization
	void Start () {
		_game_state = GameState.Explore;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetGameState(GameState state)
	{
		Debug.Log("Changing Game State to Escape!");
		_game_state = state;
	}

	public GameState GetGameState()
	{
		return _game_state;
	}
}
