using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviourSingleton<GameStateManager>
{
    Dictionary<eGameState, IGameState> states = new Dictionary<eGameState, IGameState>();

    public  eGameState gameState { get; private set; }
    IGameState curState;

    public void ChangeGameState(eGameState state)
    {
        Debug.Log("change state");

        if (curState != null)
        {
            Debug.Log("OnExit : " + curState.GetString());
            curState.OnExit();
        }
        
        IGameState stateHandler;

        if (states.TryGetValue(state, out stateHandler))
        {
            Debug.Log("OnEnter : " + stateHandler.GetString());
            gameState = state;

            stateHandler.OnEnter();

            curState = stateHandler;
        }
    }

    void Awake()
    {
        states.Add(eGameState.INTRO, new IntroState());
        states.Add(eGameState.LOBBY, new LobbyState());
        states.Add(eGameState.INGAME, new InGameState());
        states.Add(eGameState.STAGE_COMPLETE, new StageCompleteState());
        states.Add(eGameState.GAME_END, new GameEndState());
    }

    public void Start()
    {
    }

    void Update()
    {
        if (curState != null)
            curState.OnUpdate();
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //GameStateManager.Instance.ChangeGameState(GameState.INGAME);
        }
    }

}