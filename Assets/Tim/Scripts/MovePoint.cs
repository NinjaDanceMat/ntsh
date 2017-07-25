using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePoint : MonoBehaviour {

    public List<MovePoint> connections = new List<MovePoint>();


    public List<GameObject> connectionLines = new List<GameObject>();

    public GameObject conenctionLinePrefab;


    public void Start()
    {
        foreach (GameObject line in connectionLines)
        {
            DestroyImmediate(line);
        }
        connectionLines.Clear();

        foreach (MovePoint connection in connections)
        {
            GameObject newConnectionLine = Instantiate(conenctionLinePrefab);
            newConnectionLine.GetComponent<LineConnection>().from = this;
            newConnectionLine.GetComponent<LineConnection>().to = connection;
            connectionLines.Add(newConnectionLine);

        }
    }

    public void Update()
    {
        if (!Application.isPlaying)
        {
            foreach (GameObject line in connectionLines)
            {
                DestroyImmediate(line);
            }
            connectionLines.Clear();

            foreach (MovePoint connection in connections)
            {
                GameObject newConnectionLine = Instantiate(conenctionLinePrefab);
                newConnectionLine.GetComponent<LineConnection>().from = this;
                newConnectionLine.GetComponent<LineConnection>().to = connection;
                connectionLines.Add(newConnectionLine);

            }
        }
    }

}