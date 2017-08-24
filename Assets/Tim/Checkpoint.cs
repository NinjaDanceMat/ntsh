using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    public bool isTutorialProgeression;
    public bool isTutorialEnd;
    public bool slingShotOn;

	// Use this for initialization
	void Start () {
        InputController.instance.checkpoints.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
