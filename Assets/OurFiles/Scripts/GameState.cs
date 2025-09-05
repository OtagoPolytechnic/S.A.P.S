using UnityEngine;

public class GameState : Singleton<GameState>
{
	private State currentState = State.PLAYING;
	public State CurrentState
	{
		get => currentState;
		set => currentState = value;
	}

	private ContractState currentContractState = ContractState.BEGINNING;
	public ContractState CurrentContractState
	{
		get => currentContractState;
		set => currentContractState = value;
	}


	public enum State
	{
		PLAYING,
		OUT_OF_TIME,
		KILLED_TOO_MANY_NPCS,
		COMPLETED,
		TARGET_ESCAPED,
		ARRESTED,
	}

	public enum ContractState
	{
		BEGINNING,
		SEEKING_TARGET,
		RETURNING_TO_BASE,
	}

	void Start()
	{
		DontDestroyOnLoad(gameObject);
	}
}
