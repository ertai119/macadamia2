using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerStateManager
{
    Dictonary<eSpawnerState, ISpawnerState> states = new Dictonary<eSpawnerState, ISpawnerState>();
    ISpawnerState curState;

    public void Init()
    {
        states.Add(eSpawnerState.NORMAL, new NormalSpawnerState());
        states.Add(eSpawnerState.BOSS, new BossSpanwerState());
    }

    public void ChangeState(eSpawnerState state)
    {
        if (states.ContainsKey(state) == false)
            return;

        if (curState != null)
        {
            curState.OnExit();
        }

        curState = states[state];
        curState.OnEnter();
    }

    public void Update()
    {
        if (curState == null)
            return;

        curState.OnUpdate();
    }
}