using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIMode
{
    Patroling,
    Chasing,
    Searching,
}

public class Enemy : MonoBehaviour {

    public List<GameObject> path = new List<GameObject>();

    public int currentPathIndex;

    public bool goingForwardsAlongPath = true;

    public NavMeshAgent agent;

    public bool loop;

    public AIMode currentAIMode;

    public GameObject playerRobot;

    public LayerMask blockLineOfSightLayerMask;

    public Vector3 lastKnownPlyerLocation;

    // Use this for initialization
    void Start () {
        agent.destination = path[currentPathIndex].transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        if (currentAIMode == AIMode.Patroling)
        {
            bool spotted = false;
            if (!Physics.Linecast(transform.position + new Vector3(0, 0.5f, 0), playerRobot.transform.position + new Vector3(0, 0.5f, 0), blockLineOfSightLayerMask))
            {
                if (Vector3.Angle(transform.forward, playerRobot.transform.position - transform.position) < GameVariables.instance.AISightAngle)
                {
                    if (Vector3.Distance(playerRobot.transform.position, transform.position) < GameVariables.instance.AISightDistance)
                    {
                        currentAIMode = AIMode.Chasing;
                        lastKnownPlyerLocation = playerRobot.transform.position;
                        agent.destination = lastKnownPlyerLocation;
                    }
                }
            }
            if (!spotted)
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
        }
        else if (currentAIMode == AIMode.Chasing)
        {
            if (!Physics.Linecast(transform.position + new Vector3(0, 0.5f, 0), playerRobot.transform.position + new Vector3(0, 0.5f, 0), blockLineOfSightLayerMask))
            {
                if (Vector3.Angle(transform.forward, playerRobot.transform.position - transform.position) < GameVariables.instance.AISightAngle)
                {
                    if (Vector3.Distance(playerRobot.transform.position, transform.position) < GameVariables.instance.AISightDistance)
                    {
                        lastKnownPlyerLocation = playerRobot.transform.position;
                        agent.destination = lastKnownPlyerLocation;
                    }
                }
            }
        }
    }
}
