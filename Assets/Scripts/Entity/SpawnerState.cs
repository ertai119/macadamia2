using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eSpanwerState
{
    NORMAL,
    BOSS
}

interface ISpawnState
{
    void OnEnter();
    void OnUpdate();
    void OnExit();
    eSpanwerState GetType();
}

public class NormalState : ISpawnState
{
    public eSpanwerState GetType()
    {
        return eSpanwerState.NORMAL;
    }

    public void OnEnter()
    {

    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }
}

public class BossState : ISpawnState
{
    public eSpanwerState GetType()
    {
        return eSpanwerState.BOSS;
    }

    public void OnEnter()
    {

    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }
}



