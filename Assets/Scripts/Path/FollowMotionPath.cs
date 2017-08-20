using UnityEngine;
using System.Collections;

public class FollowMotionPath : MonoBehaviour
{
	public MotionPath motionPath;
	public float startPosition;
	public float speed;										// Realworld units per second you want your object to travel
	public bool loop;
	float uv;
	
	void Start()
	{
		uv = startPosition;
		if (motionPath == null)
			enabled = false;
	}
	
	
	void FixedUpdate()
	{
		uv += ((speed / motionPath.length) * Time.fixedDeltaTime);			// This gets you uv amount per second so speed is in realworld units
		if (loop)
			uv = (uv<0?1+uv:uv) %1;
		else if (uv > 1)
			enabled = false;
		Vector3 pos = motionPath.PointOnNormalizedPath(uv);
		Vector3 norm = motionPath.NormalOnNormalizedPath(uv);
		
		transform.position = pos;
		transform.forward = speed >0?norm:-norm;
	}
	
}
