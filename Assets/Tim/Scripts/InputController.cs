using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InputController : MonoBehaviour
{
    public Vector3 chestOffset;

    public bool isInSlingShot;

    public LayerMask validWalls;
    public LayerMask validFloors;
    public LayerMask validMovePoint;
    public GameObject head;
    public GameObject cameraRig;
    public GameObject robotModel;
    public Animator robotAnimator;
    public GameObject rightController;
    public GameObject leftController;
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

    public List<Vector3> rightHandTransforms = new List<Vector3>();

    public MovePoint currentMovePoint;
    public bool MovingToPoint;

    public GameObject currentLeftHandModel;
    public GameObject currentRightHandModel;

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

    public enum CameraMode
    {
        Robot,
        Wall,
    }
    public CameraMode currentMode = CameraMode.Robot;

    public static InputController instance;
    public bool eyeOnWall;

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
    }

    private void Update()
    {
        if (recallingEye)
        {
            if (Vector3.Distance(throwingEyeModel.transform.position, rightController.transform.position) < 0.5f)
            {
                eyeThrown = false;
                clutchingEyeModel.SetActive(true);
                clutchingEye = true;
                recallingEye = false;
                rightHandTransforms.Clear();

                throwingEyeModel.transform.parent = rightController.transform;
                throwingEyeModel.SetActive(false);
                throwingEyeModel.transform.localPosition = Vector3.zero;
            }
            else
            {
                Vector3 fromThrowingBallToController = rightController.transform.position - throwingEyeModel.transform.position;
                throwingEyeModel.GetComponent<Rigidbody>().velocity = (fromThrowingBallToController.normalized * GameVariables.instance.recallSpeed);
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
                rightHandTransforms.Add(rightController.transform.position);
                if (rightHandTransforms.Count > GameVariables.instance.numberOfFramesForVelocity)
                {
                    rightHandTransforms.RemoveAt(0);
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
                position = rightController.transform.position;
                forward = rightController.transform.forward;
            }
            if (Physics.Raycast(position, forward, out hit, 100f) && !GameVariables.instance.throwEye)
            {
                if (validWalls == (validWalls | (1 << hit.collider.gameObject.layer)))
                {
                    optic.SetActive(true);
                    optic.transform.position = hit.point;
                }
            }
            Destroy(currentRightHandModel);
            bool hasChangedModel = false;

            if (clutchingEye)
            {
                hasChangedModel = true;
                currentRightHandModel = Instantiate(rHandHold, rightHandModelSpawnPoint.transform);
            }
            if (!hasChangedModel)
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
                    Vector3 fromControllerToEyeOnWall = eyeModel.transform.position - rightController.transform.position;

                    Vector3 openHandVector = -rightController.transform.up;
                    if (Vector3.Angle(openHandVector, fromControllerToEyeOnWall) < GameVariables.instance.recallAngle)
                    {
                        hasChangedModel = true;
                        currentRightHandModel = Instantiate(rHandRecall, rightHandModelSpawnPoint.transform);
                    }
                }
            }
            if (!hasChangedModel && eyeOnWall)
            {
                
                if (Vector3.Distance(rightController.transform.position,armButton.transform.position) < 0.1f)
                {
                    hasChangedModel = true;
                    currentRightHandModel = Instantiate(rHandPoint, rightHandModelSpawnPoint.transform);
                }
            }

            if (!hasChangedModel && !eyeOnWall && !eyeThrown)
            {
                bool controllerInArea = false;

                Collider[] colliders = Physics.OverlapCapsule(chestModel.transform.position + new Vector3(0, 0.15f, 0), chestModel.transform.position + new Vector3(0, -0.2f, 0), 0.1f);
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
                    hasChangedModel = true;
                    currentRightHandModel = Instantiate(rHandGrab, rightHandModelSpawnPoint.transform);
                }
            }
            
            if (!hasChangedModel)
            {
                hasChangedModel = true;
                currentRightHandModel = Instantiate(rHandDefault, rightHandModelSpawnPoint.transform);
            }

            Destroy(currentLeftHandModel);
            if (isInSlingShot && !eyeThrown)
            {
                currentLeftHandModel = Instantiate(lHandRecall, leftHandModelSpawnPoint.transform);
            }
            else
            {
                currentLeftHandModel = Instantiate(lHandDefault, leftHandModelSpawnPoint.transform);
            }
        }
        else
        {
            robotAnimator.SetFloat("WalkingBlend",robotAgent.velocity.magnitude/robotAgent.speed);


            Destroy(currentRightHandModel);
            bool pointingAtButton = false;

            if (Vector3.Distance(rightController.transform.position, armButton.transform.position) < 0.1f)
            {
                pointingAtButton = true;
                currentRightHandModel = Instantiate(rHandPoint, rightHandModelSpawnPoint.transform);
            }

            else
            {
                currentRightHandModel = Instantiate(rHandDefault, rightHandModelSpawnPoint.transform);
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
                    position = rightController.transform.position;
                    forward = rightController.transform.forward;
                }
                if (GameVariables.instance.freeMovement)
                {
                    if (Physics.Raycast(position, forward, out hit, 100f))
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
                    Physics.Raycast(position, forward, out hit, 100f);
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

        chestModel.transform.Translate(new Vector3(0,chestOffset.y,0));
        //chestModel.transform.parent = null;

        //chestModel.transform.position
    }

    public void TriggerReleased()
    {
        recallingEye = false;
        if (GameVariables.instance.throwEye)
        {
            if (clutchingEye)
            {

                bool controllerInArea = false;
                Collider[] colliders = Physics.OverlapCapsule(chestModel.transform.position + new Vector3(0, 0.15f, 0), chestModel.transform.position + new Vector3(0, -0.2f, 0), 0.1f);
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

                else
                {
                    throwingEyeModel.transform.parent = null;
                    throwingEyeModel.SetActive(true);
                    clutchingEye = false;
                    clutchingEyeModel.SetActive(false);
                    eyeThrown = true;
                    eyeRecallTimer = 0;
                    eyeOnWall = false;

                    if (GameVariables.instance.canSlingShotEye && isInSlingShot)
                    {
                        isInSlingShot = false;
                        throwingEyeModel.GetComponent<Rigidbody>().velocity = (leftController.transform.position - rightController.transform.position) * GameVariables.instance.slingshotVelocity;
                    }
                    else
                    {
                        Vector3 average = new Vector3();
                        for (int i = 1; i < rightHandTransforms.Count; i++)
                        {
                            average += (rightHandTransforms[i] - rightHandTransforms[i - 1]);
                        }
                        average /= rightHandTransforms.Count - 1;
                        throwingEyeModel.GetComponent<Rigidbody>().velocity = average * GameVariables.instance.throwingVelocity;
                    }
                }
            }
        }
    }

    public void TriggerShoot()
    {
        if (currentMode == CameraMode.Robot)
        {
            if (!GameVariables.instance.throwEye)
            {
                ShootToWall();
            }
            else
            {
                if (!eyeThrown && !eyeOnWall)
                {
                    bool controllerInArea = false;

                    Collider[] colliders = Physics.OverlapCapsule(chestModel.transform.position + new Vector3(0, 0.15f, 0), chestModel.transform.position + new Vector3(0, -0.2f, 0), 0.1f);
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
                        clutchingEyeModel.SetActive(true);
                        clutchingEye = true;
                        rightHandTransforms.Clear();
                    }
                }
                else if (eyeOnWall)
                {
                    if (Vector3.Distance(rightController.transform.position, armButton.transform.position) < 0.1f)
                    {
                        ShootToTarget(onWallEyeModel.transform.position);
                    }
                }
            }
        }
        else
        {

            if (Vector3.Distance(rightController.transform.position, armButton.transform.position) < 0.1f)
            {
                Recall();
            }
            else
            {
                MoveRobot();
            }
        }
    }

    public void TriggerHeld()
    {
        if (currentMode == CameraMode.Robot)
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
                    Vector3 fromControllerToEyeOnWall = eyeModel.transform.position - rightController.transform.position;

                    Vector3 openHandVector = -rightController.transform.up;
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
        }
    }

    public void ButtonBoop()
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

    public void MoveRobot()
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
                    }
                }
                else
                {
                    if (Physics.Raycast(rightController.transform.position, rightController.transform.forward, out hit, 100f, validFloors))
                    {
                        robotAgent.destination = hit.point;
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
                            MovingToPoint = true;
                        }
                    }
                }
                else
                {
                    if (Physics.Raycast(rightController.transform.position, rightController.transform.forward, out hit, 100f, validMovePoint))
                    {
                        if (currentMovePoint.connections.Contains(hit.collider.GetComponent<MovePoint>()))
                        {
                            currentMovePoint = hit.collider.GetComponent<MovePoint>();
                            robotAgent.destination = hit.collider.transform.position;
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
            if (Physics.Raycast(rightController.transform.position, rightController.transform.forward, out hit, 100f, validWalls))
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
        onWallEyeModel.SetActive(false);

        robotModel.SetActive(true);
        robotAgent.Resume();// = false;

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

            robotAgent.Resume();// = true;
            robotModel.SetActive(false);

            Vector3 newRigPos = cameraRig.transform.position;
            Quaternion newRigRot = cameraRig.transform.rotation;
            Vector3 vectorNeededToMoveHeadToPoint = robotAgent.transform.position - head.transform.position;
            newRigPos += vectorNeededToMoveHeadToPoint;
            if (GameVariables.instance.rotatePlayerWhenWallToRobot)
            {
                newRigRot = robotRotation;
            }

            currentMode = CameraMode.Robot;
            newRigPos = new Vector3(newRigPos.x, 0f, newRigPos.z);
            FadeAndMove(newRigPos, newRigRot);

            RecallEye();
        }
    }

    public void eyeCollidedWithValidWall()
    {
        throwingEyeModel.SetActive(false);
        onWallEyeModel.SetActive(true);
        onWallEyeModel.transform.position = throwingEyeModel.transform.position;
        eyeOnWall = true;
    }

    public void RecallEye()
    {
        throwingEyeModel.transform.parent = rightController.transform;
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
    }

    public void inSlingShot()
    {
        isInSlingShot = true;
        //clutchingEyeModel.transform.parent = leftController.transform;
    }
}
