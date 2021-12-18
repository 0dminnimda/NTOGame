using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Extensions;
using static Consts;

public class RadiationEmitter : MonoBehaviour
{
    //[SerializeField]
    public float maxDistance = 10;
    [SerializeField]
    float radiusOfTarget = 1;

    [SerializeField]
    Transform origin;

    [SerializeField]
    float basicRadiationLevel;

    float debugMaxDistance = 10;
    List<Vector3> debugDirections = new List<Vector3>();
    Vector3 debugOrigin = new Vector3();
    List<Vector3> debugHitPoints = new List<Vector3>();

    public List<RayData> data = new List<RayData>();
    public List<Point> pointData = new List<Point>();
    public bool dataHasChanged;

    private void Start()
    {

    }

    private void Update()
    {
        //Control();

        dataHasChanged = false;

        ShootAllRays();
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("");
            ShootAllRays();
        }*/
    }

    [SerializeField]
    Transform controllable;

    void Control()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.x -= Screen.width / 2f;
        mousePos.y -= Screen.height / 2f;
        mousePos.z = mousePos.y;
        mousePos.y = controllable.position.z;

        controllable.SetPositionAndRotation(mousePos / 100, Quaternion.identity);
        //controllable.position = mousePos / 100;
    }

    private void ShootAllRays()
    {
        dataHasChanged = true;
        data.Clear();
        pointData.Clear();

        float angle = Mathf.Acos(1 - (radiusOfTarget * radiusOfTarget) / (2 * maxDistance * maxDistance));
        var numOfTurns = (int)Mathf.Ceil(2 * Mathf.PI / angle);

        Quaternion rotation = Quaternion.Euler(0, 0, 360f / numOfTurns);
        var direction = Vector2.right;

        // Debug.Log(numOfTurns);

        // debug
        debugMaxDistance = maxDistance;
        debugOrigin = origin.position;
        debugDirections.Clear();
        debugHitPoints.Clear();
        // debug

        for (var i = 0; i < numOfTurns; i++)
        {
            ShootOneRay(direction);
            direction = rotation * direction;
        }

        // so the circle would be completed
        data.Add(data[0]);
        pointData.Add(pointData[0]);
        pointData.Add(pointData[1]);
        pointData.Add(pointData[2]);
        pointData.Add(pointData[3]);
    }

    private void ShootOneRay(Vector2 direction2D)
    {
        Vector3 direction = transform.InverseTransformPoint(direction2D);

        RaycastHit[] hits = Physics.RaycastAll(origin.position, direction, maxDistance);

        float currentRadiationLevel = basicRadiationLevel;

        List<Point> dataItem = new List<Point>();
        foreach (RaycastHit hit in hits.OrderBy(v => origin.position.ManhattanDist(v.point)))
        {
            // debug
            debugHitPoints.Add(hit.point);
            // debug

            Wall wall = hit.transform.gameObject.GetComponent<Wall>();
            if (wall != null)
            {
                currentRadiationLevel -= wall.radiationDecrement;
            }
            else
            {

                RadiationAffected affected = hit.transform.gameObject.GetComponent<RadiationAffected>();
                if (affected != null)
                {
                    affected.AffectByRadiation(currentRadiationLevel);
                }
                else
                {
                    Debug.LogError("Should not be reachable, check your layer mask");
                }
            }

            dataItem.Add(new Point(new Vector2(hit.point.x, hit.point.z), currentRadiationLevel));
            //data.Add(new Vector2(hit.point.x, hit.point.z));
        }

        data.Add(new RayData(direction2D, dataItem.Count));
        int used = data[data.Count - 1].ptsUsed;

        for (var i = 0; i < used; i++)
            pointData.Add(dataItem[i]);

        for (var i = used; i < LEN; i++)
            pointData.Add(new Point(new Vector2(), 0f));

        // debug
        debugDirections.Add(direction);
        // debug
    }

    private void OnDrawGizmos()
    {
        foreach (Vector3 vec in debugDirections)
        {
            Gizmos.DrawLine(debugOrigin, debugOrigin + vec.normalized * debugMaxDistance);
        }

        foreach (Vector3 vec in debugHitPoints)
        {
            Gizmos.DrawSphere(vec, 0.05f);
        }
    }
}
