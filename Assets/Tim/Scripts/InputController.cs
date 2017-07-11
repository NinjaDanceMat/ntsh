using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InputController : MonoBehaviour
{

    public bool isInSlingShot;

    public LayerMask validWalls;
    public LayerMask validFloors;
    public LayerMask validMovePoint;
    public GameObject head;
    public GameObject cameraRig;
    public GameObject robotModel;
    public GameObject rightController;
    public GameObject leftController;
    public NavMeshAgent robotAgent;

    public GameObject optic;

    public bool clutchingEye;
    public GameObject throwingEyeModel;
    public GameObject clutchingEyeModel;
    public GameObject onWallEyeModel;

    //public Vector3 robotPosition;
    public Quaternion robotRotation;

    public bool eyeThrown;

    public float eyeRecallTimer;

    public List<Vector3> rightHandTransforms = new List<Vector3>();

    public MovePoint currentMovePoint;
    public bool MovingToPoint;

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
        }
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (MovingToPoint)
        {
            if (Vector3.Distance(robotAgent.transform.position,currentMovePoint.transform.position) < 0.1)
            {
                MovingToPoint = false;
            }
        }


        if (GameVariables.instance.throwEye)
        {
            if (eyeThrown)
            {
                if (!eyeOnWall)
                {
                    if (eyeRecallTimer < 3)
                    {
                        eyeRecallTimer += Time.deltaTime;
                    }
                    else
                    {
                        RecallEye();
                    }
                }
            }
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
        if (GameVariables.instance.useHeadForAimingToWall)
        {
            if (currentMode == CameraMode.Robot)
            {
                if (Physics.Raycast(head.transform.position, head.transform.forward, out hit, 100f, validWalls) && !GameVariables.instance.throwEye)
                {
                    optic.SetActive(true);
                    optic.transform.position = hit.point;
                }
                else
                {
                    optic.SetActive(false);
                }
            }
            else
            {
                if (GameVariables.instance.freeMovement)
                {
                    if (Physics.Raycast(head.transform.position, head.transform.forward, out hit, 100f, validFloors))
                    {
                        optic.SetActive(true);
                        optic.transform.position = hit.point;
                    }
                    else
                    {
                        optic.SetActive(false);
                    }
                }
                else
                {
                   
                    if (Physics.Raycast(head.transform.position, head.transform.forward, out hit, 100f, validMovePoint))
                    {
                        //Highlight Point
                    }
                    Physics.Raycast(head.transform.position, head.transform.forward, out hit, 100f, validFloors);
                    optic.transform.position = hit.point;
                    optic.SetActive(true);

                }
            }
        }
        else
        {
            if (currentMode == CameraMode.Robot)
            {
                if (Physics.Raycast(rightController.transform.position, rightController.transform.forward, out hit, 100f, validWalls) && !GameVariables.instance.throwEye)
                {
                    optic.SetActive(true);
                    optic.transform.position = hit.point;
                }
                else
                {
                    optic.SetActive(false);
                }
            }
            else
            {
                if (GameVariables.instance.freeMovement)
                {
                    if (Physics.Raycast(rightController.transform.position, rightController.transform.forward, out hit, 100f, validFloors))
                    {
                        optic.SetActive(true);
                        optic.transform.position = hit.point;
                    }
                    else
                    {
                        optic.SetActive(false);
                    }
                }
                else
                {
                    if (Physics.Raycast(rightController.transform.position, rightController.transform.forward, out hit, 100f, validMovePoint))
                    {
                        //Highlight Point
                    }
                    Physics.Raycast(rightController.transform.position, rightController.transform.forward, out hit, 100f, validFloors);
                    optic.transform.position = hit.point;
                    optic.SetActive(true);

                }
            }
        }
    }

    public void TriggerReleased()
    {
        if (GameVariables.instance.throwEye)
        {
            if (clutchingEye)
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
                    throwingEyeModel.GetComponent<Rigidbody>().velocity =  (leftController.transform.position- rightController.transform.position) * GameVariables.instance.slingshotVelocity;
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
                if (eyeOnWall)
                {
                    ShootToTarget(onWallEyeModel.transform.position);
                }
                else
                {
                    RecallEye();
                    clutchingEyeModel.SetActive(true);
                    clutchingEye = true;
                    rightHandTransforms.Clear();
                }
                
            }
        }
        else
        {
            if (!GameVariables.instance.useTriggerForRobotMovement)
            {
                Recall();
            }
            else
            {
                MoveRobot();
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

        Vector3 vectorNeededToMoveHeadToPoint = hitPoint - head.transform.position;
        newRigPos += vectorNeededToMoveHeadToPoint;

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
        eyeThrown = false;
        throwingEyeModel.transform.localPosition = Vector3.zero;
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
