using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eNpcMovementType
{
    INVALID = 0,
    FOLLOW_PLAYER,
    STRAIGHT_DIR,
}

public enum eStraightType
{
    INVALID = 0,
    LEFT,
    RIGHT,
    TOP,
    BOTTOM,
    RANDOM,
}

[RequireComponent(typeof(Rigidbody))]
public class NpcController : MonoBehaviour
{
	Rigidbody myRigidbody;
    Npc owner;
	public eNpcMovementType movementType;
    public eStraightType straightType;
	public Vector3 dir = Vector3.zero;
    public bool pause;
    public float tumple = 5f;

	// Use this for initialization
	void Start ()
	{
        owner = GetComponentInParent<Npc>();
		myRigidbody = GetComponent<Rigidbody>();
        pause = false;
    }

    public void RandomRotate()
    {
        myRigidbody.angularVelocity = Random.insideUnitSphere * tumple;
    }

    public void SetPause(bool flag)
    {
        pause = flag;
    }

    bool CanMove()
    {
        if (pause == true || myRigidbody == null || dir == Vector3.zero)
            return false;

        return true;
    }

    void FixedUpdate()
    {
        if (CanMove() == false)
            return;
        
        float speed = 1f;
        if (owner)
        {
            speed = owner.GetSpeed();
        }

		myRigidbody.MovePosition(myRigidbody.position + dir * speed * Time.fixedDeltaTime);
    }

    public void SetDir(Vector3 newDir)
    {
        dir = newDir;
    }

    public Vector3 GetDir()
    {
        return dir;
    }

    public void StartMovement()
    {
		Player player = MonoBehaviour.FindObjectOfType<Player>();
        if (player == null)
            return;

        switch(movementType)
        {
            case eNpcMovementType.FOLLOW_PLAYER:
                {
					Vector3 dirVel = CommonUtil.GetDirToTargetAdjustHeight(
						owner.transform.position, player.gameObject.transform.position);

					dir = dirVel.normalized;
                }
                break;
            case eNpcMovementType.STRAIGHT_DIR:
                {
                    dir = CommonUtil.GetStraightDir(straightType);
                }
                break;
            default:
                {
                    dir = Vector3.zero;
                }
                break;
        }

        RandomRotate();
    }
}
