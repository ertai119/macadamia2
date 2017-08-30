using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eNPC_STATE
{
    IDLE,
    SPAWN
}

public abstract class NpcState
{
    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void OnFixedUpdate();

    public eNPC_STATE GetState()
    {
        return eState;
    }

    protected eNPC_STATE eState;
    protected GameObject owner;
}

public class NpcIdleState : NpcState
{
    public NpcIdleState(GameObject obj)
    {
        eState = eNPC_STATE.IDLE;
        owner = obj;
    }

    public override void OnEnter()
    {
        Npc obstacle = owner.GetComponent<Npc>();
        if (obstacle == null)
            return;

        NpcController npcController = owner.GetComponent<NpcController>();
        if (npcController == null)
            return;

        npcController.StartMovement();
    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnExit()
    {
    }
}

public class NpcSpawnState : NpcState
{
    int delayTime = 2;
    float spawnTime = 0f;

    string holderName = "fx holder";
    Transform fxHolder;

    public NpcSpawnState(GameObject obj)
    {
        eState = eNPC_STATE.SPAWN;
        owner = obj;
    }

    public override void OnEnter()
    {
        spawnTime = 0f;

        Npc obstacle = owner.GetComponent<Npc>();
        if (obstacle == null)
            return;

        obstacle.GetComponent<Renderer>().enabled = false;

        fxHolder = new GameObject (holderName).transform;
        fxHolder.parent = obstacle.transform;

        if (obstacle.spawnFx != null)
        {
            Vector3 spawnPos = obstacle.transform.position;
            spawnPos.y += 0.2f;

            ParticleSystem spawnFx = MonoBehaviour.Instantiate(obstacle.spawnFx, spawnPos,
                obstacle.spawnFx.transform.rotation) as ParticleSystem;

            spawnFx.gameObject.transform.SetParent(fxHolder);

            bool isBoss = false;
            PropertyManager propMgr = obstacle.GetComponent<PropertyManager>();
            if (propMgr)
            {
                float value = propMgr.GetValue(ePropertyType.SCALE);
                if (value >= 2f)
                {
                    isBoss = true;
                }
            }

            ParticleExtensions.Scale(spawnFx, isBoss ? 0.75f : 0.5f);

            spawnFx.Play();
        }
    }

    public override void OnFixedUpdate()
    {
        if (spawnTime < delayTime)
        {
            spawnTime += Time.deltaTime;
            return;
        }

        Npc obstacle = owner.GetComponent<Npc>();
        if (obstacle)
        {
            obstacle.ChangeState(eNPC_STATE.IDLE);
        }
    }

    public override void OnExit()
    {
        if (fxHolder != null)
            MonoBehaviour.DestroyImmediate(fxHolder.gameObject);

        Npc obstacle = owner.GetComponent<Npc>();
        if (obstacle == null)
            return;

        obstacle.GetComponent<Renderer>().enabled = true;
        obstacle.GetComponent<Renderer>().material.color = obstacle.color;
    }
}
