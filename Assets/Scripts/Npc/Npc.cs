using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : Entity
{
    public Color color;
    public ParticleSystem spawnFx;
    public ParticleSystem destroyFx;

    NpcState curState = null;
    Dictionary<eNPC_STATE, NpcState> states = new Dictionary<eNPC_STATE, NpcState>();

    protected override void Awake()
    {
        base.Awake();
        states.Add(eNPC_STATE.IDLE, new NpcIdleState(gameObject));
        states.Add(eNPC_STATE.SPAWN, new NpcSpawnState(gameObject));
    }

    protected override void OnDestroy()
    {
        if (destroyFx != null)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.y += 0.1f;

            ParticleSystem destoryFx = MonoBehaviour.Instantiate(destroyFx, spawnPos,
                destroyFx.transform.rotation) as ParticleSystem;

            destoryFx.Play();
        }
    }

    void Start ()
    {
        ChangeState(eNPC_STATE.SPAWN);
    }

    public void ChangeState(eNPC_STATE state)
    {
        if (curState != null)
        {
            curState.OnExit();
        }

        NpcState stateHandler;
        if (states.TryGetValue(state, out stateHandler))
        {
            stateHandler.OnEnter();
            curState = stateHandler;
        }
    }

    protected override void OnPause(bool flag)
    {
        NpcController npcController = GetComponent<NpcController>();
        if (npcController)
        {
            npcController.SetPause(flag);
        }
    }

    public float GetSpeed()
    {
        return propMgr.GetValue(ePropertyType.SPEED);
    }

    void FixedUpdate()
    {
        if (pause == true)
            return;

        curState.OnFixedUpdate();
    }

    void OnTriggerEnter(Collider col)
    {
        if (curState == null)
            return;
        
        if (curState.GetState() == eNPC_STATE.SPAWN)
            return;

        if (CommonUtil.IsWall(col.gameObject))
        {
            NpcController npcController = GetComponent<NpcController>();
            if (npcController)
            {
				GameObject target = col.gameObject;
                Vector3 curDir = npcController.GetDir();

                Vector3 bounceVector = Vector3.Reflect(curDir, target.transform.forward);
                npcController.SetDir(bounceVector);
            }
        }
    }
}
