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

    void Start()
    {

    }

    void Update()
    {
        ShootAllRays();
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
        }*/
    }

    void ShootAllRays()
    {
        var angle = Mathf.Acos(1 - (radiusOfTarget * radiusOfTarget) / (2 * maxDistance * maxDistance));
        int numOfTurns = (int)Mathf.Ceil(2 * Mathf.PI / angle);

        var rotation = Quaternion.Euler(0, 360f / numOfTurns, 0);
        var direction = Vector3.right;

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

    void ShootOneRay(Vector3 direction)
    {
        var hits = Physics.RaycastAll(origin.position, direction, maxDistance)
            .OrderBy(v => origin.position.ManhattanDist(v.point));

        var currentRadiationLevel = basicRadiationLevel;

        foreach (var hit in hits)
        {
            // debug
            debugHitPoints.Add(hit.point);
            // debug

            var wall = hit.transform.gameObject.GetComponent<Wall>();
            if (wall != null)
            {
                currentRadiationLevel -= wall.radiationDecrement;
                continue;
            }

            var affected = hit.transform.gameObject.GetComponent<RadiationAffected>();
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

    void OnDrawGizmos()
    {
        foreach (var vec in debugDirections)
        {
            Gizmos.DrawLine(debugOrigin, debugOrigin + vec.normalized * debugMaxDistance);
        }

        foreach (var vec in debugHitPoints)
        {
            Gizmos.DrawSphere(vec, 0.05f);
        }
    }
}
