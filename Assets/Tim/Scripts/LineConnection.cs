using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConnection : MonoBehaviour {

    public LineRenderer lineRenderer;

    public MovePoint from;
    public MovePoint to;

    public void Start()
    {
        lineRenderer.SetPosition(0,from.transform.position);
        lineRenderer.SetPosition(1, Vector3.Lerp(from.transform.position, to.transform.position,0.5f));
    }
}
