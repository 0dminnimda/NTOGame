using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Extensions;

public class RadiationEmitter : MonoBehaviour
{
    [SerializeField]
    float maxDistance = 10;
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

    public List<(Vector2, Vector2[])> data = new List<(Vector2, Vector2[])>();
    public bool dataHasChanged;

    private void Start()
    {

    }

    private void Update()
    {
        dataHasChanged = false;

        ShootAllRays();
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("");
            ShootAllRays();
        }*/
    }

    private void ShootAllRays()
    {
        dataHasChanged = true;
        data.Clear();

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

    }

    private void ShootOneRay(Vector2 direction2D)
    {
        Vector3 direction = transform.InverseTransformPoint(direction2D);

        RaycastHit[] hits = Physics.RaycastAll(origin.position, direction, maxDistance);

        float currentRadiationLevel = basicRadiationLevel;

        List<Vector2> dataItem = new List<Vector2>();
        foreach (RaycastHit hit in hits.OrderBy(v => origin.position.ManhattanDist(v.point)))
        {
            // debug
            debugHitPoints.Add(hit.point);
            // debug

            dataItem.Add(new Vector2(hit.point.x, hit.point.z));

            Wall wall = hit.transform.gameObject.GetComponent<Wall>();
            if (wall != null)
            {
                currentRadiationLevel -= wall.radiationDecrement;
                continue;
            }

            RadiationAffected affected = hit.transform.gameObject.GetComponent<RadiationAffected>();
            if (affected != null)
            {
                affected.AffectByRadiation(currentRadiationLevel);
                continue;
            }

            Debug.LogError("Should not be reachable, check your layer mask");
        }

        data.Add((direction2D, dataItem.ToArray()));

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
