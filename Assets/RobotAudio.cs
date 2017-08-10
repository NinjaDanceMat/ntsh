using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAudio : MonoBehaviour {

	public void FootStep()
    {
        SoundManager.instance.TriggerClip(9);
    }
}
