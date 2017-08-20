using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(LineRenderer))]
[RequireComponent (typeof(MotionPath))]
public class PathController : MonoBehaviour
{
    public Shader lineTex;

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
	}

    public void VisiblePath()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("include line renderer component");
            return;
        }

        MotionPath motionPath = GetComponent<MotionPath>();
        if (motionPath == null)
            return;
        
        Vector3[] paths = iTween.GetDrawPathHelper(motionPath.controlPoints);;
        for (int i = 0; i < paths.Length; i++)
        {
            paths[i].y += 0.1f;
        }

        lineRenderer.positionCount = paths.Length;
        lineRenderer.SetPositions(paths);
        lineRenderer.widthMultiplier = 0.1f;
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
        Material whiteDiffuseMat = new Material(lineTex);
        whiteDiffuseMat.color = Color.green;

        lineRenderer.material = whiteDiffuseMat;
        //lineRenderer.material.color = Color.green;
    }

    void OnDestroy()
    {
        Destroy(GetComponent<Renderer>().material);
    }
}
