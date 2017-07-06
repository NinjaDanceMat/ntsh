using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameVariables : MonoBehaviour {

    public bool rotatePlayerWhenRobotToWall;
    public bool rotatePlayerWhenWallToRobot;
    public bool useHeadForAimingToWall;
    public bool useHeadForAimingToMoveRobot;

    public bool useTriggerForRobotMovement;
    public bool throwEye;
    public float throwingVelocity;
    public float numberOfFramesForVelocity;

    public bool blinkWhenTeleporting;

    public static GameVariables instance;

    void Awake()
    {
        if (GameVariables.instance == null)
        {
            GameVariables.instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


}
