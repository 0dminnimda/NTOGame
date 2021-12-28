using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TurretMovementController
public class TurretController : MonoBehaviour
{
    // Angular speed in radians per sec.
    public float turningSpeed = 1.0f;
    public Transform point;

    [Header("Vertical")]
    // public Transform basePart;
    public Transform verticalMovingPart;

    private float verticalAngle;
    private Vector3 originalVerticalEulerAngles;
    public float minVerticalTurnAngle = 0.02f;
    private static readonly Vector3 verticalMask = new Vector3(1, 0, 0);

    [Header("Horisontal")] public Transform horisontalMovingPart;
    private float horisontalAngle;
    private Vector3 originalHorisontalEulerAngles;
    public float minHorisontalTurnAngle = 0.02f;
    private static readonly Vector3 horisontalMask = new Vector3(0, 1, 1);


    private void Awake()
    {
        RecordAngles();
    }

    private void Update()
    {
        PointAt(point.position);
    }

    public void RecordAngles()
    {
        originalVerticalEulerAngles = verticalMovingPart.localRotation.eulerAngles;
        originalHorisontalEulerAngles = horisontalMovingPart.localRotation.eulerAngles;
    }

    private void TurnOneThing_(Vector3 point, Transform transform, float turningSpeed,
        float minTurnAngle, Vector3 originalEulerAngles,
        Vector3 rotationMask)
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = transform.parent.InverseTransformDirection(point) - transform.localPosition;
        //point - transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = turningSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(
            transform.parent.InverseTransformDirection(transform.forward), targetDirection,
            singleStep, minTurnAngle);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        Vector3 angles = Quaternion.LookRotation(newDirection).eulerAngles;

        // angles.x = originalEulerAngles.x;

        // angles.x = rotationMask.x * angles.x + (1 - rotationMask.x) * originalEulerAngles.x;
        // angles.y = rotationMask.y * angles.y + (1 - rotationMask.y) * originalEulerAngles.y;
        // angles.z = rotationMask.z * angles.z + (1 - rotationMask.z) * originalEulerAngles.z;

        originalEulerAngles = transform.parent.rotation.eulerAngles;

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.localRotation = Quaternion.Euler(new Vector3(
            rotationMask.x * angles.x + (1 - rotationMask.x) * originalEulerAngles.x,
            rotationMask.y * angles.y + (1 - rotationMask.y) * originalEulerAngles.y,
            rotationMask.z * angles.z + (1 - rotationMask.z) * originalEulerAngles.z));
    }

    private void TurnOneThing_bad(Vector3 point, Transform transform, float turningSpeed,
        float minTurnAngle, Vector3 originalEulerAngles,
        Vector3 normal)
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = transform.parent.InverseTransformDirection(Vector3.ProjectOnPlane(point - transform.position, normal));

        // The step size is equal to speed times frame time.
        float singleStep = turningSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(
            transform.parent.InverseTransformDirection(Vector3.ProjectOnPlane(transform.forward, normal)),
            targetDirection, 
            //transform.parent.InverseTransformDirection(transform.forward),
            singleStep, minTurnAngle);

        Debug.DrawRay(transform.position, targetDirection / 2, Color.green);
        Debug.DrawRay(transform.position, Vector3.ProjectOnPlane(transform.forward, normal) * 4, Color.gray);

        // var angle = Vector3.SignedAngle(targetDirection, Vector3.ProjectOnPlane(transform.forward, normal), normal);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection * 2, Color.red);

        // Vector3 angles = Quaternion.LookRotation(newDirection).eulerAngles;

        // angles.x = originalEulerAngles.x;

        // angles.x = rotationMask.x * angles.x + (1 - rotationMask.x) * originalEulerAngles.x;
        // angles.y = rotationMask.y * angles.y + (1 - rotationMask.y) * originalEulerAngles.y;
        // angles.z = rotationMask.z * angles.z + (1 - rotationMask.z) * originalEulerAngles.z;

        transform.rotation = horisontalMovingPart.parent.rotation * Quaternion.LookRotation(newDirection);

        // Debug.Log(angle);
        //
        // if (Math.Abs(angle) <= minTurnAngle * 57)
        //     angle = 0;
        // else if (Math.Abs(angle) > singleStep * 57)
        //     angle = singleStep * 57;
        // else
        //     angle = Math.Sign(angle) * Vector3.Angle(targetDirection, Vector3.ProjectOnPlane(transform.forward, normal));
        //
        // if (angle > 0)
        //     normal *= -1;
        // else
        // {
        // }

        // transform.Rotate(normal, angle);

        // originalEulerAngles = transform.parent.rotation.eulerAngles;
        //
        // // Calculate a rotation a step closer to the target and applies rotation to this object
        // transform.localRotation = Quaternion.Euler(new Vector3(
        //     rotationMask.x * angles.x + (1 - rotationMask.x) * originalEulerAngles.x,
        //     rotationMask.y * angles.y + (1 - rotationMask.y) * originalEulerAngles.y,
        //     rotationMask.z * angles.z + (1 - rotationMask.z) * originalEulerAngles.z));
    }

    private void TurnOneThing(
        Vector3 point, Transform transform, float turningSpeed, float minTurnAngle, Vector3 normal)
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = transform.parent.InverseTransformDirection(
            Vector3.ProjectOnPlane(point - transform.position, normal));
        Vector3 forward =         transform.parent.InverseTransformDirection(
            Vector3.ProjectOnPlane(transform.forward,          normal));
        
        // The step size is equal to speed times frame time.
        float singleStep = turningSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(
            forward, targetDirection, singleStep, minTurnAngle);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.localRotation = Quaternion.LookRotation(newDirection);
    }
    
    public void PointAt(Vector3 point)
    {
        // Debug.DrawRay(Vector3.zero, horisontalMovingPart.parent.InverseTransformDirection(point) * 5, Color.red);
        //Debug.DrawRay(Vector3.zero, horisontalMovingPart.parent.InverseTransformDirection(horisontalMovingPart.forward) * 5, Color.green);
        // Debug.DrawRay(Vector3.zero, - horisontalMovingPart.localPosition * 5, Color.red);
        //Debug.DrawRay(Vector3.zero, horisontalMovingPart.InverseTransformPoint(point), Color.red);
        //Debug.DrawRay(horisontalMovingPart.position, horisontalMovingPart.forward * 5, Color.green);
        Debug.DrawRay(Vector3.zero, point, Color.blue);

        TurnOneThing(point, horisontalMovingPart, turningSpeed, minHorisontalTurnAngle,
               horisontalMovingPart.parent.up); //horisontalMask);

        TurnOneThing(point, verticalMovingPart, turningSpeed, minVerticalTurnAngle,
            verticalMovingPart.parent.right);

        // // Determine which direction to rotate towards
        // Vector3 targetDirection = point - horisontalMovingPart.position;
        //
        // // The step size is equal to speed times frame time.
        // float singleStep = turningSpeed * Time.deltaTime;
        //
        // // Rotate the forward vector towards the target direction by one step
        // Vector3 newDirection = Vector3.RotateTowards(
        //     horisontalMovingPart.forward, targetDirection, 
        //     singleStep, minHorisontalTurnAngle);
        //
        // // Draw a ray pointing at our target in
        // // Debug.DrawRay(transform.position, newDirection, Color.red);
        //
        // Vector3 angles = Quaternion.LookRotation(newDirection).eulerAngles;
        //
        // angles.x = originalHorisontalEulerAngles.x;
        //
        // // angles.x = 1 * angles.x + 0 * originalHorisontalEulerAngles.x;
        // // angles.y = 0 * angles.y + 1 * originalHorisontalEulerAngles.y;
        // // angles.z = 0 * angles.z + 1 * originalHorisontalEulerAngles.z;
        //
        // // Calculate a rotation a step closer to the target and applies rotation to this object
        // horisontalMovingPart.rotation = Quaternion.Euler(angles);


        // // Determine which direction to rotate towards
        // Vector3 targetDirection = point - verticalMovingPart.position;

        // // The step size is equal to speed times frame time.
        // float singleStep = turningSpeed * Time.deltaTime;

        // // Rotate the forward vector towards the target direction by one step
        // Vector3 newDirection = Vector3.RotateTowards(
        //     verticalMovingPart.forward, targetDirection, 
        //     singleStep, minVerticalTurnAngle);

        // // Draw a ray pointing at our target in
        // // Debug.DrawRay(transform.position, newDirection, Color.red);

        // Vector3 angles = Quaternion.LookRotation(newDirection).eulerAngles;
        // 
        // angles.y = originalEulerAngles.y;
        // angles.z = originalEulerAngles.z;

        // var diff = angles.x - originalEulerAngles.x;
        // if (diff != 0)
        //     Debug.Log(diff.ToString());
        // 
        // // angles.x = 1 * angles.x + 0 * originalEulerAngles.x;
        // // angles.y = 0 * angles.y + 1 * originalEulerAngles.y;
        // // angles.z = 0 * angles.z + 1 * originalEulerAngles.z;
        // 
        // // Calculate a rotation a step closer to the target and applies rotation to this object
        // verticalMovingPart.rotation = Quaternion.Euler(angles);


        // Vector3 relPos;// = horisontalMovingPart.parent.InverseTransformVector(point);

        // Vector3 relativeToTankPosition = horisontalMovingPart.InverseTransformVector(point);
        // relativeToTankPosition.y = 0f;
        // horisontalMovingPart.rotation = Quaternion.LookRotation(
        //     relativeToTankPosition, Vector3.up);

        // Vector3 newDirection;

        // relPos = horisontalMovingPart.parent.InverseTransformPoint(point);
        //
        // Vector3 horisontalDirection = new Vector3(relPos.x, 0, relPos.z);
        // newDirection = Vector3.RotateTowards(
        //     horisontalMovingPart.forward, horisontalDirection, 360f, 0.0f);
        // horisontalMovingPart.localRotation = Quaternion.LookRotation(newDirection);


        // relPos = verticalMovingPart.parent.InverseTransformVector(point);
        //
        // var lookPos = point - verticalMovingPart.position;
        // lookPos.Normalize();
        // Vector3 verticalDirection = new Vector3(0, lookPos.y, 0);
        // Debug.DrawRay(verticalMovingPart.position, lookPos * 5, Color.red);
        // //Debug.DrawRay(verticalMovingPart.position, verticalDirection * 5, Color.red);
        // Debug.DrawRay(verticalMovingPart.position, (verticalMovingPart.forward + verticalDirection) * 5, Color.red);
        // //Debug.DrawRay(verticalMovingPart.position, (verticalMovingPart.forward) * 5, Color.red);
        // //Debug.DrawRay(verticalMovingPart.position, (verticalMovingPart.up) * 5, Color.red);
        // // Debug.DrawRay(verticalMovingPart.position, (verticalMovingPart.right) * 5, Color.red);

        //newDirection = Vector3.RotateTowards(
        //    verticalMovingPart.forward, verticalMovingPart.forward + verticalDirection, 360f, 0.0f);
        //verticalMovingPart.rotation = Quaternion.LookRotation(newDirection);

        // newDirection = Vector3.RotateTowards(
        //     verticalMovingPart.up, verticalDirection,360f, 0.0f);
        // verticalMovingPart.localRotation = Quaternion.LookRotation(newDirection);

        // var lookPos = point - verticalMovingPart.position;
        // lookPos.x = 0;
        // lookPos.z = 0;
        // verticalMovingPart.rotation = Quaternion.LookRotation(lookPos);
        // var rotation = Quaternion.LookRotation(lookPos);
        // transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }
}