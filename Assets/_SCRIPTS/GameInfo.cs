using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public void SetGameState(GameState state)
	{
		Debug.Log("Changing Game State to " + state + "!");
		_gameState = state;
		if (_gameState == GameState.Won)
			OnGameWon();
	}

	public GameState GetGameState()
	{
		return _gameState;
	}

	public void OnGameWon()
	{
		WIN.SetActive(true);
	}
}
