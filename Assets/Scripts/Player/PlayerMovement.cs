using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public delegate void OnApprochGoal();
    public event OnApprochGoal OnApprochGoalEvent = delegate {};

    public float posPercentage = 0f;

    Player owner;
    MotionPath motionPath;
    bool loadPath = false;
    bool pause = false;

	void Start ()
    {
        owner = gameObject.GetComponentInParent<Player>();
        owner.OnPauseEvent += StopControll;
	}
	
    public void Init(MotionPath motionPath)
    {
        this.motionPath = motionPath;
        loadPath = true;
        posPercentage = 0f;
        pause = false;
    }

    void StopControll(bool flag)
    {
        pause = flag;
    }

    void FixedUpdate()
    {
        if (loadPath == false)
            return;

        AutoRotate();

        if (pause == true)
            return;
        
        float newSpeed = 0f;
        if (Input.GetKey(KeyCode.Space) || Input.touchCount > 0)
        {
            newSpeed = owner.GetSpeed();
        }
        else
        {
            newSpeed = -1f * owner.GetSpeed();
        }

        posPercentage += ((newSpeed / motionPath.length) * Time.fixedDeltaTime);
        if (posPercentage < 0f)
            posPercentage = 0;

        if (posPercentage >= 1f)
        {
            posPercentage = 1f;
            owner.ApproachGoal();
            OnApprochGoalEvent();
        }

        if (motionPath.looping)
            posPercentage = (posPercentage < 0 ? 1 + posPercentage : posPercentage) % 1;
        else if (posPercentage > 1)
            enabled = false;

        SetPos(posPercentage);
    }

    void SetPos(float posPercent)
    {
        Vector3 pos = motionPath.PointOnNormalizedPath(posPercent);
        //Vector3 norm = motionPath.NormalOnNormalizedPath(posPercent);

        pos.y += 0.5f;
        transform.position = pos;
        //transform.forward = owner.GetSpeed() > 0 ? norm : -norm;
    }

    void AutoRotate()
    {
        transform.Rotate(new Vector3(0f, owner.GetSpeed(), 0f));
    }
}
