using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    MotionPath motionPath;
    Animator anim;

    public float posPercentage = 0f;
    public float speedRollbackDelay = 0f;

    public bool debugTouch = false;
    bool loadPath = false;
    bool gameEnd = false;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
    }

    public void StartGame(MotionPath motionPath)
    {
        this.motionPath = motionPath;
        posPercentage = 0f;
        SetPos(posPercentage);

        loadPath = true;
        gameEnd = false;

        SetAnimation("RUN00_F");
    }

    void SetAnimation(string animName)
    {
        if (anim == null)
            return;

        anim.Play(animName, -1, 0f);
    }

    protected override void OnPause(bool flag)
    {
        anim.speed = flag ? 0f : 1f;
    }

    float GetSpeed()
    {
        float speed = propMgr.GetValue(ePropertyType.SPEED);
        if (speed <= 0f)
            speed = 1f;
        
        return speed;
    }

    void SetPos(float posPercent)
    {
        Vector3 pos = motionPath.PointOnNormalizedPath(posPercent);
        Vector3 norm = motionPath.NormalOnNormalizedPath(posPercent);

        transform.position = pos;
        transform.forward = GetSpeed() > 0 ? norm : -norm;
    }

    void FixedUpdate()
    {
        if (loadPath == false)
            return;

        if (gameEnd == true)
            return;

        if (pause == true)
            return;
        
        float newSpeed = 0f;
        if (Input.GetKey(KeyCode.Space) || Input.touchCount > 0)
        {
            newSpeed = GetSpeed();
        }
        else
        {
            newSpeed = -1f * GetSpeed();
        }

        posPercentage += ((newSpeed / motionPath.length) * Time.fixedDeltaTime);
        if (posPercentage < 0f)
            posPercentage = 0;

        if (posPercentage >= 1f)
        {
            posPercentage = 1f;
            ApproachGoal();
        }
        
        if (motionPath.looping)
            posPercentage = (posPercentage < 0 ? 1 + posPercentage : posPercentage) % 1;
        else if (posPercentage > 1)
            enabled = false;

        SetPos(posPercentage);
    }

    void ApproachGoal()
    {
        gameEnd = true;
        GameStateManager.Instance.ChangeGameState(eGameState.STAGE_COMPLETE);
        SetAnimation("WIN00");
    }

    void GameEnd()
    {
        gameEnd = true;
        GameStateManager.Instance.ChangeGameState(eGameState.GAME_END);
        SetAnimation("LOSE00");
    }

    void OnTriggerEnter(Collider col)
    {
        if (debugTouch == true)
            return;
        
        if (CommonUtil.IsNpc(col.gameObject))
        {
            Npc npc = col.gameObject.GetComponent<Npc>();
            if (npc != null)
            {
                GameEnd();
                return;
            }
        }

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
