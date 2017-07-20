using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

	// Use this for initialization
	void Start () {
        InputController.instance.checkpoints.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
