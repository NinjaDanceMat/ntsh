using UnityEngine;
using System.Collections;

public class LightningBoltUnity : MonoBehaviour {

    private LineRenderer lRend;
    private Vector3[] points = new Vector3[5];

    private readonly int point_Begin = 0;
    private readonly int point_Middle_Left = 1;
    private readonly int point_Center = 2;
    private readonly int point_Middle_Right = 3;
    private readonly int point_End = 4;

    public Transform line;

    public float randomPosOffset = 0.3f;
    public float randomWithOffsetMax = 2f;
    public  float randomWithOffsetMin = 1f;

    private readonly WaitForSeconds customFrame = new WaitForSeconds(0.001f);


    void Start () {
        lRend = GetComponent<LineRenderer>();
	}

    void Update()
    {

        points[point_Begin] = transform.position;
        points[point_End] = line.position;
        CalculateMiddle();
        lRend.SetPositions(points);
        lRend.startWidth = RandomWidthOffset();
        lRend.endWidth = RandomWidthOffset();
    }

    private float RandomWidthOffset()
    {
        return Random.Range(randomWithOffsetMin, randomWithOffsetMax);
    }

    private void CalculateMiddle()
    {
        Vector3 center = GetMiddleWithRandomness(transform.position, line.position);

        points[point_Center] = center;
        points[point_Middle_Left] = GetMiddleWithRandomness(transform.position, center);
        points[point_Middle_Right] = GetMiddleWithRandomness(center, line.position);
    }

    private Vector3 GetMiddleWithRandomness (Vector3 point1, Vector3 point2)
    {
        float x = (point1.x + point2.x) / point_Center;
        float finalX = Random.Range(x - randomPosOffset, x + randomPosOffset);
        float y = (point1.y + point2.y) / point_Center;
        float finalY = Random.Range(y - randomPosOffset, y + randomPosOffset);
        float z = (point1.z + point2.z) / point_Center;
        float finalZ = Random.Range(z - randomPosOffset, z + randomPosOffset);

        return new Vector3(finalX, finalY, finalZ);
    }
}
