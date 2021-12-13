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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootAllRays();
        }
    }

    void ShootAllRays()
    {
        var radius = radiusOfTarget * 1.75f;
        var angle = Mathf.Acos(1 - (radius * radius) / (2 * maxDistance * maxDistance));
        int numOfTurns = (int)Mathf.Ceil(2 * Mathf.PI / angle);


        var rotation = Quaternion.Euler(0, 360 / numOfTurns, 0);
        var direction = Vector3.right;

        Debug.Log(numOfTurns);

        // debug
        debugMaxDistance = maxDistance;
        debugOrigin = origin.position;
        debugDirections.Clear();
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
            .OrderBy(v => origin.position.ManhattanDist(v.transform.position));

        var currentRadiationLevel = basicRadiationLevel;

        foreach (var hit in hits)
        {
            Debug.Log(hit.transform.gameObject.name);
            /*Renderer rend = hit.transform.GetComponent<Renderer>();

            if (rend)
            {
                // Change the material of all hit colliders
                // to use a transparent shader.
                rend.material.shader = Shader.Find("Transparent/Diffuse");
                Color tempColor = rend.material.color;
                tempColor.a = 0.3F;
                rend.material.color = tempColor;
            }*/
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
    }

    /*RaycastHit[] GetRaycasts(Vector3 direction)
    {
        return Physics.RaycastAll(origin.position, direction, maxDistance)
            .OrderBy(v => origin.position.ManhattanDist(v.transform.position));
    }*/
}
