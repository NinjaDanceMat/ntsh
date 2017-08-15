using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InputController : MonoBehaviour
{
    public Vector3 chestOffset;

    public bool isInSlingShot;

    public bool dead;

    public LayerMask validWalls;
    public LayerMask validFloors;
    public LayerMask defaultLayerMask;
    public LayerMask validMovePoint;
    public GameObject head;
    public GameObject cameraRig;
    public GameObject robotModel;
    public Animator robotAnimator;
    public GameObject rightController;
    public GameObject leftController;
    public GameObject dominantController;
    public GameObject weakController;
    public GameObject weakStuff;
    public GameObject dominantStuff;


    public NavMeshAgent robotAgent;

    public GameObject optic;

    public bool clutchingEye;
    public GameObject throwingEyeModel;
    public GameObject clutchingEyeModel;
    public GameObject onWallEyeModel;

    public GameObject chestModel;

    //public Vector3 robotPosition;
    public Quaternion robotRotation;

    public bool eyeThrown;

    public float eyeRecallTimer;

    public List<Vector3> dominantHandTransforms = new List<Vector3>();

    public MovePoint currentMovePoint;
    public bool MovingToPoint;

    public GameObject currentLeftHandModel;
    public GameObject currentRightHandModel;

    public Controller leftHandInputController;
    public Controller rightHandInputController;

    public GameObject leftHandModelSpawnPoint;
    public GameObject rightHandModelSpawnPoint;

    public GameObject lHandDefault;
    public GameObject lHandGrab;
    public GameObject lHandHold;
    public GameObject lHandPoint;
    public GameObject lHandRecall;

    public GameObject rHandDefault;
    public GameObject rHandGrab;
    public GameObject rHandHold;
    public GameObject rHandPoint;
    public GameObject rHandRecall;

    public GameObject armButton;

    public bool recallingEye;

    public GameObject deathText;

    public List<Enemy> enemies = new List<Enemy>();
    public Vector3 rigCheckpointPos;
    public Quaternion rigCheckpointRot;

    public List<Checkpoint> checkpoints = new List<Checkpoint>();

    public List<Vector3> thrownEyeTransforms = new List<Vector3>();
    public int currentRecallEyeTransform = 0;

    public bool eyeFromChest;

    public LayerMask cantMoveRobotThroughLayerMask;

    public enum CameraMode
    {
        Robot,
        Wall,
    }
    public CameraMode currentMode = CameraMode.Robot;

    public static InputController instance;
    public bool eyeOnWall;

    public bool oldLeftHandMode;

    public bool transitioning;

    public bool newRobotDestination;

    // Use this for initialization
    void Awake()
    {
        if (InputController.instance == null)
        {
            InputController.instance = this;

            currentLeftHandModel = Instantiate(lHandDefault, leftHandModelSpawnPoint.transform);
            currentRightHandModel = Instantiate(rHandDefault, rightHandModelSpawnPoint.transform);
        }
        else
        {
            Destroy(this);
        }
        rigCheckpointPos = cameraRig.transform.position;
        rigCheckpointRot = cameraRig.transform.rotation;

        ChangeHandMode(GameVariables.instance.leftHandMode);
    }

    public void ChangeHandMode(bool leftHand)
    {
        if (leftHand)
        {
            dominantController = leftController;
            weakController = rightController;
        }
        else
        {
            dominantController = rightController;
            weakController = leftController;
        }

        weakStuff.transform.parent = weakController.transform;
        weakStuff.transform.localPosition = Vector3.zero;
        weakStuff.transform.localRotation = Quaternion.identity;
        dominantStuff.transform.parent = dominantController.transform;
        dominantStuff.transform.localPosition = Vector3.zero;
        dominantStuff.transform.localRotation = Quaternion.identity;

        oldLeftHandMode = GameVariables.instance.leftHandMode;
    }

    private void Update()
    {
        if (newRobotDestination)
        {
            if (Vector3.Distance(robotAgent.destination, robotAgent.transform.position) < 0.1f)
            {
                robotAgent.destination = robotAgent.transform.position;
                SoundManager.instance.TriggerClip(9);
                newRobotDestination = false;
            }
        }
        if (oldLeftHandMode != GameVariables.instance.leftHandMode)
        {
            ChangeHandMode(GameVariables.instance.leftHandMode);
        }
        Destroy(currentLeftHandModel);
        Destroy(currentRightHandModel);
        bool hasChangedDominantHandModel = false;

        if (!dead)
        {
            if (recallingEye)
            {
                if (!SoundManager.instance.soundEffects[5].source.isPlaying)
                {
                    SoundManager.instance.TriggerClip(5);
                }

                float speed = GameVariables.instance.eyeToHandSpeed;
                if (!eyeFromChest)
                {
                    speed = GameVariables.instance.recallSpeed;
                    if (Vector3.Distance(throwingEyeModel.transform.position, dominantController.transform.position) < 0.5f)
                    {
                        throwingEyeModel.transform.position = dominantController.transform.position;
                    }
                }
                if (Vector3.Distance(throwingEyeModel.transform.position, dominantController.transform.position) < 0.05f)
                {
                    vibrate(true, 0.06f, 3000);

                    eyeFromChest = false;
                    eyeThrown = false;
                    clutchingEyeModel.SetActive(true);
                    clutchingEye = true;
                    recallingEye = false;
                    dominantHandTransforms.Clear();

                    throwingEyeModel.transform.parent = dominantStuff.transform;
                    throwingEyeModel.SetActive(false);
                    throwingEyeModel.transform.localPosition = Vector3.zero;

                    SoundManager.instance.TriggerClip(6);

                    
                    TutorialManager.instance.SetTutorial(1);
                    
                }
                else
                {
                    if (!Physics.Linecast(throwingEyeModel.transform.position, dominantController.transform.position, defaultLayerMask))
                    {
                        Vector3 fromThrowingBallToController = dominantController.transform.position - throwingEyeModel.transform.position;
                        throwingEyeModel.GetComponent<Rigidbody>().velocity = (fromThrowingBallToController.normalized * speed);
                    }
                    else
                    {
                        currentRecallEyeTransform -= 1;
                        throwingEyeModel.transform.position = thrownEyeTransforms[currentRecallEyeTransform];
                        thrownEyeTransforms.RemoveAt(thrownEyeTransforms.Count - 1);
                        throwingEyeModel.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    }
                }
            }

            else
            {
                if (SoundManager.instance.soundEffects[5].source.isPlaying)
                {
                    SoundManager.instance.soundEffects[5].source.Stop();
                }
            }

            if (MovingToPoint)
            {
                if (Vector3.Distance(robotAgent.transform.position, currentMovePoint.transform.position) < 0.1)
                {
                    MovingToPoint = false;
                }
            }

            if (GameVariables.instance.throwEye)
            {
                if (clutchingEye)
                {
                    dominantHandTransforms.Add(dominantController.transform.position);
                    if (dominantHandTransforms.Count > GameVariables.instance.numberOfFramesForVelocity)
                    {
                        dominantHandTransforms.RemoveAt(0);
                    }
                }
            }
            RaycastHit hit;
            Vector3 forward = new Vector3();
            Vector3 position = new Vector3();

            optic.SetActive(false);
            if (currentMode == CameraMode.Robot)
            {
                if (GameVariables.instance.useHeadForAimingToWall)
                {
                    position = head.transform.position;
                    forward = head.transform.forward;
                }
                else
                {
                    position = dominantController.transform.position;
                    forward = dominantController.transform.forward;
                }
                if (Physics.Raycast(position, forward, out hit, 100f) && !GameVariables.instance.throwEye)
                {
                    if (validWalls == (validWalls | (1 << hit.collider.gameObject.layer)))
                    {
                        optic.SetActive(true);
                        optic.transform.position = hit.point;
                    }
                }

                if (clutchingEye)
                {
                    if (GameVariables.instance.leftHandMode)
                    {
                        currentLeftHandModel = Instantiate(lHandHold, leftHandModelSpawnPoint.transform);
                    }
                    else
                    {
                        currentRightHandModel = Instantiate(rHandHold, rightHandModelSpawnPoint.transform);
                    }
                    hasChangedDominantHandModel = true;
                }
                if (!hasChangedDominantHandModel)
                {
                    if (eyeOnWall || eyeThrown)
                    {
                        GameObject eyeModel = null;
                        if (eyeOnWall)
                        {
                            eyeModel = onWallEyeModel;
                        }
                        else
                        {
                            eyeModel = throwingEyeModel;
                        }
                        Vector3 fromControllerToEyeOnWall = eyeModel.transform.position - dominantController.transform.position;

                        Vector3 openHandVector = -dominantController.transform.up;
                        if (Vector3.Angle(openHandVector, fromControllerToEyeOnWall) < GameVariables.instance.recallAngle)
                        {
                            if (GameVariables.instance.leftHandMode)
                            {
                                currentLeftHandModel = Instantiate(lHandRecall, leftHandModelSpawnPoint.transform);
                            }
                            else
                            {
                                currentRightHandModel = Instantiate(rHandRecall, rightHandModelSpawnPoint.transform);
                            }
                            hasChangedDominantHandModel = true;
                        }
                    }
                }
                if (!hasChangedDominantHandModel)
                {
                    if (eyeOnWall)
                    {
                        if (Vector3.Distance(dominantController.transform.position, armButton.transform.position) < 0.1f)
                        {
                            if (GameVariables.instance.leftHandMode)
                            {
                                currentLeftHandModel = Instantiate(lHandPoint, leftHandModelSpawnPoint.transform);
                            }
                            else
                            {
                                currentRightHandModel = Instantiate(rHandPoint, rightHandModelSpawnPoint.transform);
                            }
                            hasChangedDominantHandModel = true;
                        }
                    }
                }
                if (!hasChangedDominantHandModel)
                {
                    if (!eyeOnWall && !eyeThrown)
                    {
                        bool controllerInArea = false;

                        Collider[] colliders = Physics.OverlapCapsule(chestModel.transform.position + new Vector3(0, 0.15f, 0), chestModel.transform.position + new Vector3(0, -0.2f, 0), GameVariables.instance.chestAreaRadius);
                        foreach (Collider collider in colliders)
                        {
                            if (GameVariables.instance.leftHandMode)
                            {
                                if (collider.tag == "LeftController")
                                {
                                    controllerInArea = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (collider.tag == "RightController")
                                {
                                    controllerInArea = true;
                                    break;
                                }
                            }
                        }

                        if (controllerInArea)
                        {
                            Vector3 fromControllerToEyeInChest = chestModel.transform.position - dominantController.transform.position;

                            Vector3 openHandVector = -dominantController.transform.up;

                            if (GameVariables.instance.leftHandMode)
                            {
                                currentLeftHandModel = Instantiate(lHandGrab, leftHandModelSpawnPoint.transform);
                            }
                            else
                            {
                                currentRightHandModel = Instantiate(rHandGrab, rightHandModelSpawnPoint.transform);
                            }
                            hasChangedDominantHandModel = true;

                        }
                    }
                }
                if (!hasChangedDominantHandModel)
                {
                    hasChangedDominantHandModel = true;
                    if (GameVariables.instance.leftHandMode)
                    {
                        currentLeftHandModel = Instantiate(lHandDefault, leftHandModelSpawnPoint.transform);
                    }
                    else
                    {
                        currentRightHandModel = Instantiate(rHandDefault, rightHandModelSpawnPoint.transform);
                    }
                }

                if (isInSlingShot && !eyeThrown)
                {
                    if (!GameVariables.instance.leftHandMode)
                    {
                        currentLeftHandModel = Instantiate(lHandRecall, leftHandModelSpawnPoint.transform);
                    }
                    else
                    {
                        currentRightHandModel = Instantiate(rHandRecall, rightHandModelSpawnPoint.transform);
                    }
                }
                else
                {
                    if (!GameVariables.instance.leftHandMode)
                    {
                        currentLeftHandModel = Instantiate(lHandDefault, leftHandModelSpawnPoint.transform);
                    }
                    else
                    {
                        currentRightHandModel = Instantiate(rHandDefault, rightHandModelSpawnPoint.transform);
                    }
                }

                if (eyeThrown && !recallingEye)
                {
                    bool addIt = false;
                    if (thrownEyeTransforms.Count <= 1)
                    {
                        addIt = true;
                    }
                    else
                    {
                        if (Vector3.Distance(thrownEyeTransforms[currentRecallEyeTransform - 1], throwingEyeModel.transform.position) > 0.2f)
                        {
                            addIt = true;
                        }
                    }
                    if (addIt)
                    {
                        thrownEyeTransforms.Add(throwingEyeModel.transform.position);
                        currentRecallEyeTransform += 1;
                    }
                }
            }
            else
            {

                if (!GameVariables.instance.leftHandMode)
                {
                    currentLeftHandModel = Instantiate(lHandDefault, leftHandModelSpawnPoint.transform);
                }
                else
                {
                    currentRightHandModel = Instantiate(rHandDefault, rightHandModelSpawnPoint.transform);
                }

                robotAnimator.SetFloat("WalkingBlend", robotAgent.velocity.magnitude / robotAgent.speed);

                bool pointingAtButton = false;

                if (Vector3.Distance(dominantController.transform.position, armButton.transform.position) < 0.1f)
                {
                    pointingAtButton = true;
                    if (GameVariables.instance.leftHandMode)
                    {
                        currentLeftHandModel = Instantiate(lHandPoint, leftHandModelSpawnPoint.transform);
                    }
                    else
                    {
                        currentRightHandModel = Instantiate(rHandPoint, rightHandModelSpawnPoint.transform);
                    }
                }

                else
                {
                    if (GameVariables.instance.leftHandMode)
                    {
                        currentLeftHandModel = Instantiate(lHandDefault, leftHandModelSpawnPoint.transform);
                    }
                    else
                    {
                        currentRightHandModel = Instantiate(rHandDefault, rightHandModelSpawnPoint.transform);
                    }
                }
                optic.SetActive(false);
                if (!pointingAtButton)
                {
                    if (GameVariables.instance.useHeadForAimingToMoveRobot)
                    {
                        position = head.transform.position;
                        forward = head.transform.forward;
                    }
                    else
                    {
                        position = dominantController.transform.position;
                        forward = dominantController.transform.forward;
                    }
                    if (GameVariables.instance.freeMovement)
                    {
                        if (Physics.Raycast(position, forward, out hit, 100f, cantMoveRobotThroughLayerMask))
                        {
                            if (validFloors == (validFloors | (1 << hit.collider.gameObject.layer)))
                            {
                                optic.SetActive(true);
                                optic.transform.position = hit.point;
                            }
                        }
                    }
                    else
                    {

                        if (Physics.Raycast(position, forward, out hit, 100f, validMovePoint))
                        {
                            //Highlight Point
                        }
                        Physics.Raycast(position, forward, out hit, 100f, cantMoveRobotThroughLayerMask);
                        if (validFloors == (validFloors | (1 << hit.collider.gameObject.layer)))
                        {
                            optic.transform.position = hit.point;
                            optic.SetActive(true);
                        }
                    }
                }
            }
            //chestModel.transform.parent = head.transform;
            chestModel.transform.position = head.transform.position;

            Vector3 headFacing = head.transform.forward;
            Vector3 backwardsFromHead = -headFacing;

            backwardsFromHead = new Vector3(backwardsFromHead.x, 0, backwardsFromHead.z);

            chestModel.transform.Translate(backwardsFromHead.normalized * chestOffset.x);

            chestModel.transform.Translate(new Vector3(0, chestOffset.y, 0));
            //chestModel.transform.parent = null;

            //chestModel.transform.position

            foreach (Checkpoint point in checkpoints)
            {
                if (Vector3.Distance(point.transform.position, robotAgent.transform.position) < point.GetComponent<SphereCollider>().radius)
                {
                    rigCheckpointPos = point.transform.position;
                    if (point.isTutorialProgeression)
                    {
                        if (TutorialManager.instance.currentTutorial == 5)
                        {
                            TutorialManager.instance.SetTutorial(6);
                        }
                    }
                }
            }
        }
    }

    public void TriggerReleased()
    {
        if (!dead)
        {
            if (recallingEye)
            {

                TutorialManager.instance.SetTutorial(3);


                eyeFromChest = false;
                throwingEyeModel.GetComponent<Rigidbody>().velocity = throwingEyeModel.GetComponent<Rigidbody>().velocity.normalized;
            }
            recallingEye = false;

            if (GameVariables.instance.throwEye)
            {
                if (clutchingEye)
                {
                    /*
                    bool controllerInArea = false;
                    Collider[] colliders = Physics.OverlapCapsule(chestModel.transform.position + new Vector3(0, 0.15f, 0), chestModel.transform.position + new Vector3(0, -0.2f, 0), 0.15f);
                    foreach (Collider collider in colliders)
                    {
                        if (collider.tag == "RightController")
                        {
                            controllerInArea = true;
                            break;
                        }
                    }

                    if (controllerInArea)
                    {
                        RecallEye();
                    }
                    */

                    
                    TutorialManager.instance.SetTutorial(3);
                    

                    throwingEyeModel.transform.parent = null;
                    throwingEyeModel.SetActive(true);
                    clutchingEye = false;
                    clutchingEyeModel.SetActive(false);
                    eyeThrown = true;
                    eyeRecallTimer = 0;
                    eyeOnWall = false;

                    currentRecallEyeTransform = 0;
                    thrownEyeTransforms.Clear();

                    SoundManager.instance.TriggerClip(4);

                    if (GameVariables.instance.canSlingShotEye && isInSlingShot)
                    {
                        isInSlingShot = false;
                        throwingEyeModel.GetComponent<Rigidbody>().velocity = (weakController.transform.position - dominantController.transform.position) * GameVariables.instance.slingshotVelocity;
                    }
                    else
                    {
                        Vector3 average = new Vector3();
                        for (int i = 1; i < dominantHandTransforms.Count; i++)
                        {
                            average += (dominantHandTransforms[i] - dominantHandTransforms[i - 1]);
                        }
                        average /= dominantHandTransforms.Count - 1;
                        throwingEyeModel.GetComponent<Rigidbody>().velocity = average * GameVariables.instance.throwingVelocity;
                    }

                }
            }
        }
    }

    public void TriggerShoot()
    {
        if (!dead)
        {
            if (currentMode == CameraMode.Robot)
            {
                if (!GameVariables.instance.throwEye)
                {
                    ShootToWall();
                    vibrate(true, 0.06f, 750);
                }
                else
                {
                    if (eyeOnWall)
                    {
                        if (!transitioning && Vector3.Distance(dominantController.transform.position, armButton.transform.position) < 0.1f)
                        {
                            ShootToTarget(onWallEyeModel.transform.position);
                            vibrate(true, 0.06f, 750);
                            SoundManager.instance.TriggerClip(7);

                            TutorialManager.instance.SetTutorial(5);
                        }
                    }
                }
            }
            else
            {
                if (Vector3.Distance(dominantController.transform.position, armButton.transform.position) < 0.1f)
                {
                    vibrate(true, 0.06f, 750);
                    SoundManager.instance.TriggerClip(7);
                    TutorialManager.instance.SetTutorial(0);
                    Recall();
                }
                else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(dominantController.transform.position, dominantController.transform.forward, out hit, 100f, cantMoveRobotThroughLayerMask))
                    {
                        if (validFloors == (validFloors | (1 << hit.collider.gameObject.layer)))
                        {
                            MoveRobot(true);
                            SoundManager.instance.TriggerClip(8);
                        }
                    }
                }
            }
        }
    }

    public void TriggerHeld()
    {
        if (!dead)
        {
            if (currentMode == CameraMode.Robot && !transitioning)
            {
                if (GameVariables.instance.throwEye)
                {
                    if (eyeOnWall || eyeThrown)
                    {
                        GameObject eyeModel = null;
                        if (eyeOnWall)
                        {
                            eyeModel = onWallEyeModel;
                        }
                        else
                        {
                            eyeModel = throwingEyeModel;
                        }
                        Vector3 fromControllerToEyeOnWall = eyeModel.transform.position - dominantController.transform.position;

                        Vector3 openHandVector = -dominantController.transform.up;
                        if (Vector3.Angle(openHandVector, fromControllerToEyeOnWall) < GameVariables.instance.recallAngle)
                        {
                            recallingEye = true;
                            if (eyeOnWall)
                            {
                                throwingEyeModel.transform.position = onWallEyeModel.transform.position;
                                onWallEyeModel.SetActive(false);
                                throwingEyeModel.SetActive(true);
                                eyeOnWall = false;
                            }
                        }
                    }
                }
                if (!eyeThrown && !eyeOnWall && !clutchingEye)
                {
                    bool controllerInArea = false;

                    Collider[] colliders = Physics.OverlapCapsule(chestModel.transform.position + new Vector3(0, 0.15f, 0), chestModel.transform.position + new Vector3(0, -0.2f, 0), GameVariables.instance.chestAreaRadius);
                    foreach (Collider collider in colliders)
                    {
                        if (GameVariables.instance.leftHandMode)
                        {
                            if (collider.tag == "LeftController")
                            {
                                controllerInArea = true;
                                break;
                            }
                        }
                        else
                        {
                            if (collider.tag == "RightController")
                            {
                                controllerInArea = true;
                                break;
                            }
                        }
                    }

                    if (controllerInArea)
                    {
                        Vector3 fromControllerToEyeInChest = chestModel.transform.position - dominantController.transform.position;

                        Vector3 openHandVector = -dominantController.transform.up;

                        if (!recallingEye)
                        {
                            eyeThrown = true;
                            throwingEyeModel.transform.parent = null;
                            throwingEyeModel.transform.position = chestModel.transform.position;
                            onWallEyeModel.SetActive(false);
                            clutchingEyeModel.SetActive(false);
                            throwingEyeModel.SetActive(true);
                            eyeOnWall = false;
                            recallingEye = true;

                            eyeFromChest = true;

                            SoundManager.instance.TriggerClip(3);

                            
                            TutorialManager.instance.SetTutorial(1);
                            
                        }

                    }
                }
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(dominantController.transform.position, dominantController.transform.forward, out hit, 100f, cantMoveRobotThroughLayerMask))
                {
                    if (validFloors == (validFloors | (1 << hit.collider.gameObject.layer)))
                    {
                        MoveRobot();
                    }
                }
            }
        }
    }

    public void ButtonBoop()
    {
        if (!dead)
        {
            if (currentMode == CameraMode.Wall)
            {
                if (!GameVariables.instance.useTriggerForRobotMovement)
                {
                    MoveRobot();
                }
                else
                {
                    Recall();
                }
            }
            else
            {
                if (eyeOnWall)
                {
                    ShootToTarget(onWallEyeModel.transform.position);
                }
                else if (eyeThrown)
                {
                    RecallEye();
                }
            }
        }
    }

    public void MoveRobot(bool canVibrate = false)
    {
        if (!MovingToPoint)
        {
            if (GameVariables.instance.freeMovement)
            {
                RaycastHit hit;
                if (GameVariables.instance.useHeadForAimingToMoveRobot)
                {
                    if (Physics.Raycast(head.transform.position, head.transform.forward, out hit, 100f, validFloors))
                    {
                        
                        robotAgent.destination = hit.point;
                        if (canVibrate)
                        {
                            newRobotDestination = true;
                            vibrate(true, 0.06f, 750);
                        }
                    }
                }
                else
                {
                    if (Physics.Raycast(dominantController.transform.position, dominantController.transform.forward, out hit, 100f, validFloors))
                    {
                        robotAgent.destination = hit.point;
                        if (canVibrate)
                        {
                            newRobotDestination = true;
                            vibrate(true, 0.06f, 750);
                        }
                    }
                }
            }
            else
            {
                RaycastHit hit;
                if (GameVariables.instance.useHeadForAimingToMoveRobot)
                {
                    if (Physics.Raycast(head.transform.position, head.transform.forward, out hit, 100f, validMovePoint))
                    {
                        if (currentMovePoint.connections.Contains(hit.collider.GetComponent<MovePoint>()))
                        {
                            currentMovePoint = hit.collider.GetComponent<MovePoint>();
                            
                            robotAgent.destination = hit.collider.transform.position;
                            if (canVibrate)
                            {
                                newRobotDestination = true;
                                vibrate(true, 0.06f, 750);
                            }
                            MovingToPoint = true;
                        }
                    }
                }
                else
                {
                    if (Physics.Raycast(dominantController.transform.position, dominantController.transform.forward, out hit, 100f, validMovePoint))
                    {
                        if (currentMovePoint.connections.Contains(hit.collider.GetComponent<MovePoint>()))
                        {
                            currentMovePoint = hit.collider.GetComponent<MovePoint>();
                            
                            robotAgent.destination = hit.collider.transform.position;
                            if (canVibrate)
                            {
                                newRobotDestination = true;
                                vibrate(true, 0.06f, 750);
                            }
                            MovingToPoint = true;
                        }
                    }
                }
            }
        }
    }

    public void ShootToWall()
    {
        RaycastHit hit = new RaycastHit();
        bool validShot = false;

        if (GameVariables.instance.useHeadForAimingToWall)
        {
            if (Physics.Raycast(head.transform.position, head.transform.forward, out hit, 100f, validWalls))
            {
                validShot = true;
            }
        }
        else
        {
            if (Physics.Raycast(dominantController.transform.position, dominantController.transform.forward, out hit, 100f, validWalls))
            {
                validShot = true;
            }
        }
        if (validShot)
        {
            ShootToTarget(hit.point);
        }
    }

    public void ShootToTarget(Vector3 hitPoint)
    {

        robotAgent.isStopped = false;

        currentMode = CameraMode.Wall;

        Vector3 newRigPos = cameraRig.transform.position;
        Quaternion newRigRot = cameraRig.transform.rotation;

        if (GameVariables.instance.rotatePlayerWhenRobotToWall)
        {
            Quaternion headRotOffset = head.transform.localRotation;
            //Rotate camera relative to head position;
            newRigRot = Quaternion.LookRotation(head.transform.position - hitPoint);
            newRigRot *= Quaternion.Inverse(headRotOffset);
            newRigRot = Quaternion.Euler(0, newRigRot.eulerAngles.y, 0);
        }
        Quaternion oldRigRot = cameraRig.transform.rotation;
        cameraRig.transform.rotation = newRigRot;

        Vector3 vectorNeededToMoveHeadToPoint = hitPoint - head.transform.position;
        newRigPos += vectorNeededToMoveHeadToPoint;

        cameraRig.transform.rotation = oldRigRot;

        FadeAndMove(newRigPos, newRigRot);
    }

    public void Recall()
    {
        if (!MovingToPoint)
        {
            Debug.Log("Recall");
            eyeOnWall = false;

            robotAgent.destination = robotAgent.transform.position;
            robotAgent.isStopped = true;// = true;


            Vector3 newRigPos = cameraRig.transform.position;
            Quaternion newRigRot = cameraRig.transform.rotation;

            if (GameVariables.instance.rotatePlayerWhenWallToRobot)
            {
                Quaternion headRotOffset = head.transform.localRotation;
                //Rotate camera relative to head position;
                newRigRot = robotAgent.transform.rotation;
                newRigRot *= Quaternion.Inverse(headRotOffset);
                newRigRot = Quaternion.Euler(0, newRigRot.eulerAngles.y, 0);
            }
            Quaternion oldRigRot = cameraRig.transform.rotation;
            cameraRig.transform.rotation = newRigRot;

            Vector3 vectorNeededToMoveHeadToPoint = robotAgent.transform.position - head.transform.position;
            newRigPos += vectorNeededToMoveHeadToPoint;

            cameraRig.transform.rotation = oldRigRot;

            currentMode = CameraMode.Robot;
            newRigPos = new Vector3(newRigPos.x, 0f, newRigPos.z);

            FadeAndMove(newRigPos, newRigRot);

            RecallEye();
        }
    }

    public void eyeCollidedWithValidWall()
    {
        if (!recallingEye)
        {
            throwingEyeModel.SetActive(false);
            onWallEyeModel.SetActive(true);
            onWallEyeModel.transform.position = throwingEyeModel.transform.position;
            eyeOnWall = true;

            TutorialManager.instance.SetTutorial(4);
        }
        vibrate(true, 0.06f, 3000);
    }

    public void eyeCollidedWithNonValidWall()
    {
        vibrate(true, 0.01f, 750);
    }

    public void vibrate(bool dominantController, float time, float strength)
    {
        if (dominantController && GameVariables.instance.leftHandMode || !dominantController && !GameVariables.instance.leftHandMode)
        {
            if (rightHandInputController != null)
            {
                StartCoroutine(LongVibration(SteamVR_Controller.Input((int)leftHandInputController.controller.controllerIndex), time, strength));
            }
        }
        else
        {
            if (leftHandInputController != null)
            {
                StartCoroutine(LongVibration(SteamVR_Controller.Input((int)rightHandInputController.controller.controllerIndex), time, strength));
            }
        }
    }

    public void RecallEye()
    {
        throwingEyeModel.transform.parent = dominantStuff.transform;
        throwingEyeModel.SetActive(false);

        clutchingEye = false;
        clutchingEyeModel.SetActive(false);
        eyeThrown = false;
        throwingEyeModel.transform.localPosition = Vector3.zero;

        onWallEyeModel.SetActive(false);
        eyeOnWall = false;
    }

    private float _fadeDuration = 0.5f;

    private void FadeAndMove(Vector3 newRigPos, Quaternion newRigRot)
    {
        transitioning = true;
        FadeToWhite();
        StartCoroutine(FadeFromWhite(newRigPos, newRigRot,_fadeDuration));
    }
    private void FadeToWhite()
    {
        //set start color
        SteamVR_Fade.Start(Color.clear, 0f);
        //set and start fade to
        SteamVR_Fade.Start(Color.black, _fadeDuration);
    }
    public IEnumerator FadeFromWhite(Vector3 newRigPos, Quaternion newRigRotation, float delay)
    {
        yield return new WaitForSeconds(delay);

        cameraRig.transform.rotation = newRigRotation;
        cameraRig.transform.position = newRigPos;

        //set start color
        SteamVR_Fade.Start(Color.black, 0f);
        //set and start fade to

        SteamVR_Fade.Start(Color.clear, _fadeDuration);

        transitioning = false;


        if (currentMode == CameraMode.Wall)
        {
            onWallEyeModel.SetActive(false);

            robotModel.SetActive(true);
        }
        else
        {
            robotModel.SetActive(false);
        }
    }

    public void inSlingShot()
    {
        isInSlingShot = true;
        //clutchingEyeModel.transform.parent = leftController.transform;
    }

    public void PlayerKilled()
    {
        if (!dead)
        {
            SoundManager.instance.TriggerClip(10);
            robotAnimator.SetBool("Dead", true);
            deathText.SetActive(true);
            dead = true;
            robotAgent.isStopped = true;
            StartCoroutine(respawnTimer());
        }
    }

    public IEnumerator respawnTimer()
    {
        yield return new WaitForSeconds(3f);

        foreach (Enemy enemy in enemies)
        {
            enemy.Respawn();
        }
        cameraRig.transform.position = rigCheckpointPos;
        cameraRig.transform.rotation = rigCheckpointRot;
        dead = false;
        deathText.SetActive(false);
        robotAgent.isStopped = false;

        robotAgent.Warp(rigCheckpointPos);
        robotAgent.transform.rotation = rigCheckpointRot;
        robotModel.SetActive(false);

        currentMode = CameraMode.Robot;

        isInSlingShot = false;
        clutchingEye = false;
        eyeOnWall = false;
        eyeThrown = false;
        recallingEye = false;
        MovingToPoint = false;
        eyeFromChest = false;

        RecallEye();
    }

    IEnumerator LongVibration(SteamVR_Controller.Device device, float length, float strength)
    {
        for (float i = 0; i < length; i += Time.deltaTime)
        {
            device.TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
            yield return null;
        }
    }
}