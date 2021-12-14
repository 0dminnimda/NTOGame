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

    private void Start()
    {

    }

    private void Update()
    {
        ShootAllRays();
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
        }*/
    }

    private void ShootAllRays()
    {
        float angle = Mathf.Acos(1 - (radiusOfTarget * radiusOfTarget) / (2 * maxDistance * maxDistance));
        int numOfTurns = (int)Mathf.Ceil(2 * Mathf.PI / angle);

        float rotation = Quaternion.Euler(0, 360f / numOfTurns, 0);
        Vector3 direction = Vector3.right;

        // Debug.Log(numOfTurns);

        // debug
        debugMaxDistance = maxDistance;
        debugOrigin = origin.position;
        debugDirections.Clear();
        debugHitPoints.Clear();
        // debug

        for (int i = 0; i < numOfTurns; i++)
        {
            ShootOneRay(direction);
            direction = rotation * direction;
        }

    }

    private void ShootOneRay(Vector3 direction)
    {
        var hits = Physics.RaycastAll(origin.position, direction, maxDistance)
            .OrderBy(v => origin.position.ManhattanDist(v.point));

        float currentRadiationLevel = basicRadiationLevel;

        foreach (var hit in hits)
        {
            // debug
            debugHitPoints.Add(hit.point);
            // debug

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
