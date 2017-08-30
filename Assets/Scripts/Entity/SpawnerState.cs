using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eSpawnerState
{
    INVALID,
    IDLE,
    NORMAL,
    BOSS
}

interface ISpawnState
{
    void OnEnter();
    void OnUpdate(float deltaTime);
    void OnExit();
    eSpawnerState GetState();
}

public class IdleState : ISpawnState
{
    public eSpawnerState GetState()
    {
        return eSpawnerState.IDLE;
    }

    public void OnEnter()
    {

    }

    public void OnUpdate(float deltaTime)
    {

    }

    public void OnExit()
    {

    }
}

public class NormalState : ISpawnState
{
    EntitySpawner owner;
    int totalSpawnCount;
    float nextSpawnTime;

    public NormalState(EntitySpawner spawner)
    {
        owner = spawner;
    }

    bool EnableSpawn()
    {
        int spawnedObjCount = owner.GetCurSpawnedObjCount(eSpawnType.NPC_NORMAL);
        if (owner.totalSpawnCount <= spawnedObjCount)
            return false;

        float curTime = Time.time;
        if (curTime <= nextSpawnTime)
            return false;
        
        return true;
    }

    public eSpawnerState GetState()
    {
        return eSpawnerState.NORMAL;
    }

    public void OnEnter()
    {
        Debug.Log("OnEnter Normal State");
        nextSpawnTime = Time.time;
    }

    public void OnUpdate(float deltaTime)
    {
        if (EnableSpawn())
        {
            owner.SpawnEntity(eSpawnType.NPC_NORMAL
                , owner.FindRandomSpawnPos()
                , GameResourceName.contentsOptionalNpcName);

            nextSpawnTime = Time.time + owner.spawnDelay;
        }

        if (totalSpawnCount <= owner.GetCurSpawnedObjCount(eSpawnType.NPC_NORMAL)
            && owner.enableSpawnBoss == true)
        {
            owner.SetNextState(eSpawnerState.BOSS);
        }
    }

    public void OnExit()
    {
        Debug.Log("OnExit Normal State");
    }
}

public class BossState : ISpawnState
{
    EntitySpawner owner;
    int bossSpawnCount = 1;
    float nextSpawnTime;

    public BossState(EntitySpawner spawner)
    {
        owner = spawner;
    }

    bool EnableSpawn()
    {
        int spawnedObjCount = owner.GetCurSpawnedObjCount(eSpawnType.NPC_BOSS);
        if (bossSpawnCount <= spawnedObjCount)
            return false;

        float curTime = Time.time;
        if (curTime <= nextSpawnTime)
            return false;

        return true;
    }

    public eSpawnerState GetState()
    {
        return eSpawnerState.BOSS;
    }

    public void OnEnter()
    {
        Debug.Log("OnEnter Boss State");
    }

    public void OnUpdate(float deltaTime)
    {
        if (EnableSpawn())
        {
            owner.SpawnEntity(eSpawnType.NPC_BOSS
                , owner.FindRandomSpawnPos()
                , GameResourceName.contentBossNpcName);

            nextSpawnTime = Time.time + owner.spawnDelay;
        }
    }

    public void OnExit()
    {
        Debug.Log("OnExit Boss State");
    }
}


