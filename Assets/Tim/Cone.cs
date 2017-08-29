using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cone : MonoBehaviour
{
    public MeshFilter meshRender;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        List<Vector3> vetices = new List<Vector3>();

        float angle = -50;
        int RaysToShoot = 100;

        for (int i = 0; i < RaysToShoot; i++)
        {
            Vector3 direction = Quaternion.AngleAxis(angle, transform.up) * transform.forward;

            angle += 100 / RaysToShoot;

            Vector3 dir = new Vector3(direction.x, 0, direction.y);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(transform.position, dir, out hit, 20.1f))
            {
                vetices.Add(hit.point);
            }
            else
            {
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vetices.ToArray();

        meshRender.mesh = mesh;
    }
}

