using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerMovement))]
[RequireComponent (typeof(PropertyManager))]
public class Player : Entity
{
    PlayerMovement playerMovement;

    public bool debugTouch = false;
    bool gameEnd = false;

    public delegate void OnPlayerEvent(bool flag);
    public event OnPlayerEvent OnPauseEvent;

    protected override void Awake()
    {
        base.Awake();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        float scale = propMgr.GetValue(ePropertyType.SCALE);
        if (scale != 0f)
        {
            Vector3 curScale = transform.localScale;
            gameObject.transform.localScale = curScale * scale;
        }

        GameManager gameMgr = FindObjectOfType<GameManager>();
        if (gameMgr)
        {
            gameMgr.OnPauseEvent += SetPause;
        }
    }

    protected override void OnDestroy()
    {
        GameManager gameMgr = FindObjectOfType<GameManager>();
        if (gameMgr)
        {
            gameMgr.OnPauseEvent -= SetPause;
        }
    }

    public bool IsGameEnd()
    {
        return gameEnd;
    }

    public void StartGame(MotionPath motionPath)
    {
        if (playerMovement != null)
        {
            playerMovement.Init(motionPath);
        }

        gameEnd = false;
    }

    protected override void OnPause(bool flag)
    {
        OnPauseEvent(flag);
    }

    public float GetSpeed()
    {
        float speed = propMgr.GetValue(ePropertyType.SPEED);
        if (speed <= 0f)
            speed = 1f;
        
        return speed;
    }

    public void ApproachGoal()
    {
        GameStateManager.Instance.ChangeGameState(eGameState.STAGE_COMPLETE);
    }

    public void GameEnd()
    {
        GameStateManager.Instance.ChangeGameState(eGameState.GAME_END);
    }

    void OnTriggerEnter(Collider col)
    {
        if (debugTouch == true)
            return;

        if (CommonUtil.IsBuff(col.gameObject))
        {
            BuffEntity buffEntity = col.gameObject.GetComponent<BuffEntity>();
            if (buffEntity)
            {
                buffEntity.GiveBuff(this.gameObject);

                DestroyObject(buffEntity.gameObject);
            }
        }
    }
}
