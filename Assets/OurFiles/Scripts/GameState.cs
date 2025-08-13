using UnityEngine;

public class GameState : Singleton<GameState>
{
	private State currentState = State.PLAYING;
	public State CurrentState => currentState;


	public enum State
	{
		PLAYING,
		OUT_OF_TIME,
		KILLED_TOO_MANY_NPCS,
		COMPLETED,
		TARGET_ESCAPED,
		ARRESTED,
	}

	void Start()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void UpdateState(State newState)
	{
		if (CurrentState != newState)
			currentState = newState;
	}
}