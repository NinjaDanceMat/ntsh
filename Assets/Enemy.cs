using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIMode
{
    Patroling,
    Chasing,
    Waiting,
    Searching,
    Returning,
}

public class Enemy : MonoBehaviour {

    public List<GameObject> path = new List<GameObject>();

    public int currentPathIndex;

    public bool goingForwardsAlongPath = true;

    public NavMeshAgent agent;
    public NavMeshAgent playerRobotAgent;
    public bool loop;

    public AIMode currentAIMode;

    public GameObject playerRobot;

    public LayerMask blockLineOfSightLayerMask;

    public Vector3 lastKnownPlyerLocation;
    public Vector3 lastKnownPlyerVector;

    public float waitTimer;

    public int searchIndex = 0;

    public Vector3 lastKnownPlayerThisFoward;

    public bool justWaited;

    public Vector3 spawnPoint;
    public Quaternion spawnRot;

    // Use this for initialization
    void Start () {
        playerRobotAgent = InputController.instance.robotAgent;
        playerRobot = InputController.instance.robotAgent.gameObject;
        InputController.instance.enemies.Add(this);
        agent = GetComponent<NavMeshAgent>();

        agent.destination = path[currentPathIndex].transform.position;

        spawnPoint = transform.position;
        spawnRot = transform.rotation;

    }

    public void Respawn()
    {
        agent.Warp (spawnPoint);
        transform.rotation = spawnRot;
        currentAIMode = AIMode.Patroling;
        currentPathIndex = 0;
        agent.destination = path[currentPathIndex].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        agent.speed = GameVariables.instance.AIChasingSpeed;
        bool spotted = false;
        if (currentAIMode != AIMode.Chasing)
        {
            agent.speed = GameVariables.instance.AIDefaultSpeed;

            if (!Physics.Linecast(transform.position + new Vector3(0, 0.5f, 0), playerRobot.transform.position + new Vector3(0, 0.5f, 0), blockLineOfSightLayerMask))
            {
                if (Vector3.Distance(playerRobot.transform.position, transform.position) < 1f || Vector3.Angle(transform.forward, playerRobot.transform.position - transform.position) < GameVariables.instance.AISightAngle)
                {
                    if (Vector3.Distance(playerRobot.transform.position, transform.position) < GameVariables.instance.AISightDistance)
                    {
                        agent.isStopped = false;

                        currentAIMode = AIMode.Chasing;
                        lastKnownPlyerLocation = playerRobot.transform.position;
                        lastKnownPlyerVector = playerRobotAgent.desiredVelocity;
                        agent.destination = lastKnownPlyerLocation;

                        lastKnownPlayerThisFoward = transform.forward;
                    }
                }
            }
        }
        if (!spotted && currentAIMode == AIMode.Patroling)
        {
            if (Vector3.Distance(transform.position, path[currentPathIndex].transform.position) < 0.1f)
            {
                if (goingForwardsAlongPath)
                {
                    if (currentPathIndex + 1 < path.Count)
                    {
                        currentPathIndex += 1;
                    }
                    else
                    {
                        if (loop)
                        {
                            currentPathIndex = 0;
                        }
                        else
                        {
                            goingForwardsAlongPath = false;
                            currentPathIndex -= 1;
                        }
                    }
                }
                else
                {
                    if (currentPathIndex - 1 >= 0)
                    {
                        currentPathIndex -= 1;
                    }
                    else
                    {
                        if (loop)
                        {
                            currentPathIndex = path.Count - 1;
                        }
                        else
                        {
                            goingForwardsAlongPath = true;
                            currentPathIndex += 1;
                        }
                    }
                }
                agent.destination = path[currentPathIndex].transform.position;
            }
        }

        if (!spotted && currentAIMode == AIMode.Chasing)
        {
            if (Vector3.Distance(transform.position, lastKnownPlyerLocation) < 0.1f)
            {
                searchIndex = 0;
                currentAIMode = AIMode.Waiting;
                lastKnownPlayerThisFoward = transform.forward;
                waitTimer = 0;
            }
            if (!Physics.Linecast(transform.position + new Vector3(0, 0.5f, 0), playerRobot.transform.position + new Vector3(0, 0.5f, 0), blockLineOfSightLayerMask))
            {
                if (Vector3.Distance(playerRobot.transform.position, transform.position) < 1f || Vector3.Angle(transform.forward, playerRobot.transform.position - transform.position) < GameVariables.instance.AISightAngle)
                {
                    if (Vector3.Distance(playerRobot.transform.position, transform.position) < GameVariables.instance.AISightDistance)
                    {
                        lastKnownPlyerLocation = playerRobot.transform.position;
                        lastKnownPlyerVector = playerRobot.transform.forward;
                        agent.destination = lastKnownPlyerLocation;

                        if (Vector3.Distance(playerRobot.transform.position, transform.position) < GameVariables.instance.AIMeleeRange)
                        {
                            InputController.instance.PlayerKilled();
                        }
                    }
                }
            }
        }
        if (!spotted && currentAIMode == AIMode.Waiting)
        {
            if (waitTimer >= GameVariables.instance.AIWaitTime)
            {
                agent.isStopped = false;
                agent.destination = transform.position;
                currentAIMode = AIMode.Searching;
            }
            else
            {
                if (waitTimer == 0)
                {
                    agent.isStopped = true;
                   
                }
                if (waitTimer <= GameVariables.instance.AIWaitTime/2)
                {
                    transform.Rotate(0.0f, -70.0f*Time.deltaTime, 0.0f);
                }
                else
                {
                    transform.Rotate(0.0f, 140.0f * Time.deltaTime, 0.0f);
                }
                waitTimer += Time.deltaTime;
            }
        }
        if (!spotted && currentAIMode == AIMode.Searching)
        {
            if (searchIndex == 0)
            {
                searchIndex += 1;
                agent.destination = lastKnownPlyerLocation + (lastKnownPlyerVector * 3) ;
 
            }

            else if (Vector3.Distance(agent.destination, transform.position) < 0.1f)
            {
                if (!justWaited)
                {
                    currentAIMode = AIMode.Waiting;
                    waitTimer = 0;
                    justWaited = true;
                }
                else
                {

                    if (searchIndex == 1)
                    {
                        float angle = Vector3.Angle(lastKnownPlayerThisFoward, lastKnownPlyerVector);
                        Vector3 cross = Vector3.Cross(lastKnownPlayerThisFoward, lastKnownPlyerVector);
                        if (cross.y < 0)
                        {
                            angle = -angle;
                        }
                        if (angle > 0)
                        {
                            agent.destination = transform.position + (transform.right * 2f);
                            justWaited = false;
                            searchIndex += 1;
                        }
                        else
                        {
                            agent.destination = transform.position + (-transform.right * 2f);
                            justWaited = false;
                            searchIndex += 1;
                        }
                    }
                    else if (searchIndex == 2)
                    {
                        agent.destination = transform.position + (-transform.forward * 2f);
                        justWaited = false;
                        searchIndex += 1;
                    }
                    else if (searchIndex == 3)
                    {
                        currentAIMode = AIMode.Patroling;
                        agent.destination = path[currentPathIndex].transform.position;
                    }
                }
            }
        }
    }
}
