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
    public float slingshotVelocity;
    public float numberOfFramesForVelocity;

    public bool canSlingShotEye;

    public bool blinkWhenTeleporting;

    public bool freeMovement;

    public static GameVariables instance;

    public float recallAngle;
    public float recallSpeed;

    public float AISightAngle;
    public float AISightDistance;

    public float AIWaitTime;

    public float AIDefaultSpeed;
    public float AIChasingSpeed;

    public float AIMeleeRange;

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